using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bittrex.Api.Client;
using Bittrex.Api.Client.Models;
using BittrexModels.Interfaces;
using System.Timers;

namespace BittrexModels.ActorModels
{
    public class BittrexApiManager : IBittrexApi
    {
        public bool ApiReady;

        public BittrexClient BittrexClient { get; }

        public List<DateTime> OperationJournal { get; }

        private Queue<Task> Tasks { get; }

        public BittrexApiManager(BittrexClient bittrexClient)
        {
            this.BittrexClient = bittrexClient;
            OperationJournal = new List<DateTime>();
            Tasks = new Queue<Task>();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += ProcessRequestTimerAction;
            timer.Start();
        }


        public void ProcessRequestTimerAction(object sender, EventArgs e)
        {
            if (Tasks.Count == 0) return;
            if (CheckRequestLimit())
            {
                var task = Tasks.Dequeue();
                task.Start();
            }
        }

        /// <summary>
        /// Валидация клиента
        /// </summary>
        /// <returns></returns>
        public bool CheckClient()
        {
            this.ApiReady = true;
            return true;
        }

        public async Task<decimal> GetPrice(ITransaction transaction)
        {
            Task<decimal> task = new Task<decimal>(() =>
            {
                try
                {
                    ApiResult<Ticker> apiResult = null;

                    var t = this.BittrexClient.GetTicker(transaction.MarketName);
                    Task.WaitAll(t);
                    apiResult = t.Result;
                    OperationJournal.Add(DateTime.Now);

                    if (transaction.Type == OperationType.Buy)
                        return apiResult.Result.Ask;
                    else
                        return apiResult.Result.Bid;
                } catch (Exception ex)
                {
                    return 0;
                }

            });
            Tasks.Enqueue(task);

            return await task;
        }

 
    

    public async Task<IObservation> GetObservation(string TargetMarket)
        {
            var cts = new CancellationTokenSource();


            Task<Observation> task = new Task<Observation>(() =>
            {
                try
                {
                    var ordersBid = BittrexClient.GetOrderBook(TargetMarket, BookOrderType.Buy, 10); // !!
                    var ordersAsk = BittrexClient.GetOrderBook(TargetMarket, BookOrderType.Sell, 10);
                    var price = BittrexClient.GetMarketSummary(TargetMarket);

                    Task.WaitAll(ordersBid, ordersAsk, price);

                    var obs = new Observation();

                    OperationJournal.Add(DateTime.Now);
                    OperationJournal.Add(DateTime.Now);
                    OperationJournal.Add(DateTime.Now);

                    var sumBid = ordersBid.Result.Result.Sum(x => x.Quantity);
                    var sumAsk = ordersAsk.Result.Result.Sum(x => x.Quantity);
                    obs.BidPrice = price.Result.Result.Bid;
                    obs.AskPrice = price.Result.Result.Ask;
                    obs.OrderAskSum = sumAsk;
                    obs.OrderBidSum = sumBid;
                    obs.ObservationTime = DateTime.Now;
                    obs.IsComplete = true;
                    return obs;
                }
                catch (Exception ex)
                {
                    // TODO: logger 
                    return null;
                }
            }, cts.Token);
                       
            Tasks.Enqueue(task);

            return await task;
        }

             

        private bool CheckRequestLimit()
        {
            if (OperationJournal.Count < Consts.BittrexRequestLimit) return true;

            var now = DateTime.Now;
            var minute = new TimeSpan(0, 1, 0);
            int count = 0;

            for(int i = OperationJournal.Count - 1; i >= 0; i--)
            {
                if (now - OperationJournal[i] > minute) break;
                count++;
            }
            OperationJournal.RemoveRange(0, OperationJournal.Count - count);

            return OperationJournal.Count < Consts.BittrexRequestLimit;
            
        }
    }
}
