using Bittrex.Api.Client;
using Bittrex.Api.Client.Models;
using BittrexModels.ActorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BittrexModels
{
    public class BittrexApiManager
    {
        private BittrexClient BittrexClient;
        // private Queue<Task> BittrexTasks;

        private int ProcessedTask = 0;
        private DateTime LastRequestTime;
        private readonly TimeSpan oneMinuteSpan = new TimeSpan(0, 1, 0);
        public BittrexApiManager()
        {
            BittrexClient = new BittrexClient("6c58ca3f387b4581ab2ba324b7a78dd5", "");
        }

        public enum BittrexRequestType { CHECK_CONECTION, GET_TICKER, GET_SELL_BOOK, GET_BUY_BOOK }

        public Task ApiRequest(BittrexRequestType requestType, string marketName)
        {
            Task task = null;
            switch (requestType)
            {
                case BittrexRequestType.CHECK_CONECTION:
                    return new Task(() => { BittrexClient.GetOrderBook("BTC-LTC", BookOrderType.Buy, 1); });
                    break;
                case BittrexRequestType.GET_BUY_BOOK:
                    //var ordersBid = await BittrexClient.GetOrderBook(marketName, BookOrderType.Buy, 10); // !!

                    break;
                case BittrexRequestType.GET_SELL_BOOK:
                    break;

            }
            return task;
        }

        private void WaitBittrexPermission(int requestAmount = 1)
        {
            if (ProcessedTask + requestAmount >= 60)
            {
                var deltaTime = LastRequestTime - DateTime.Now;
                if (deltaTime < oneMinuteSpan)
                {
                    Thread.Sleep(oneMinuteSpan - deltaTime);
                    this.LastRequestTime = DateTime.Now;

                }
                ProcessedTask = 0;
            }
            ProcessedTask += requestAmount;
        }
        /// <summary>
        /// Выполнение транзакции (запрос последней цены для покупаемой валюты для иммитации покупки/продажи)
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<Ticker>> CommitTransaction(string MarketName)
        {

            WaitBittrexPermission();


            var startTransact = DateTime.Now;
            ApiResult<Ticker> apiResult = null;
            try
            {
                apiResult = await BittrexClient.GetTicker(MarketName);
                return apiResult;
            }
            catch (Exception e)
            {
                // todo: logger
                return null;
            }
        }

        public async Task<Observation> MakeObservation(string targetMarket)
        {
            WaitBittrexPermission(4);
            try
            {
                BittrexClient client = new BittrexClient("", "");
                var ordersBid = await client.GetOrderBook(targetMarket, BookOrderType.Buy, 10); // !!
                var ordersAsk = await client.GetOrderBook(targetMarket, BookOrderType.Sell, 10);
                var bidPrice = await client.GetMarketSummary(targetMarket);

                var sumBid = ordersBid.Result.Sum(x => x.Quantity);
                var sumAsk = ordersAsk.Result.Sum(x => x.Quantity);
                var obs = new Observation(bidPrice.Result.Bid, bidPrice.Result.Ask, sumAsk, sumBid);
                return obs;
            }
            catch (Exception e)
            {
                // logger
                return null;
            }
        }

    }
}
