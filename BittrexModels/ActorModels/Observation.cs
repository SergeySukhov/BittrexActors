using BittrexModels.Interfaces;
using System;

namespace BittrexModels.ActorModels
{
    public class Observation : IObservation
    {
       public DateTime ObservationTime { get; }
       
       public decimal BidPrice { get; }
       public decimal AskPrice { get; }
       public decimal OrderAskSum { get; }
       public decimal OrderBidSum { get; }

        public Observation(decimal bid, decimal ask, decimal orderAskSum, decimal orderBidSum)
        {
            BidPrice = bid;
            AskPrice = ask;
            OrderAskSum = orderAskSum;
            OrderBidSum = orderBidSum;
            this.ObservationTime = DateTime.Now;
        }

       
    }
}
