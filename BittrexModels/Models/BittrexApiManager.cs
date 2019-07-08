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
using DataManager.Models;

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
        }


        public void ProcessRequestTimerAction(object sender, EventArgs e)
        {
            if (Tasks.Count == 0) return;
            if (CheckRequestLimit())
            {
                var task = Tasks.Dequeue();
                if (task != null) task.Start();
                else { Console.WriteLine("!!! null task in BittrexApi"); }
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

        public async Task<decimal> GetPrice(Transaction transaction)
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




        public async Task<Observation> GetObservation(string TargetMarket)
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


                OperationJournal.Add(DateTime.Now);
                OperationJournal.Add(DateTime.Now);
                OperationJournal.Add(DateTime.Now);
                    var obs = new Observation()
                    {
                        Guid = Guid.NewGuid(),
                        ObservationTime = DateTime.Now,
                        MarketName = TargetMarket,
                        BidPrice = price.Result.Result.Bid,
                        AskPrice = price.Result.Result.Ask,
                        OrderBidSum = ordersBid.Result.Result.Sum(x => x.Quantity),
                        OrderAskSum = ordersAsk.Result.Result.Sum(x => x.Quantity)
                    };
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
