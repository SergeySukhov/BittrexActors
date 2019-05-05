using Bittrex.Api.Client;
using Bittrex.Api.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public class Observation
    {
        // todo : добавить проверку на 60 запросов в мин
        public static async Task<Observation> MakeObservation(string targetMarket)
        {
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
        public Guid Guid; 

        public readonly DateTime ObservationTime;

        public readonly decimal BidPrice = 0;
        public readonly decimal AskPrice = 0;
        public readonly decimal OrderAskSum = 0;
        public readonly decimal OrderBidSum = 0;

        public Observation(decimal bid, decimal ask, decimal orderAskSum, decimal orderBidSum)
        {
            this.Guid = Guid.NewGuid();
            BidPrice = bid;
            AskPrice = ask;
            OrderAskSum = orderAskSum;
            OrderBidSum = orderBidSum;
            this.ObservationTime = DateTime.Now;
        }

    }
}
