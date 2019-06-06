using BittrexModels.Interfaces;
using System;

namespace BittrexModels.ActorModels
{
    public class Observation : IObservation
    {
       public DateTime ObservationTime { get; set; }
       
       public decimal BidPrice { get; set; }
       public decimal AskPrice { get; set; }
       public decimal OrderAskSum { get; set; }
       public decimal OrderBidSum { get; set; }
        public bool IsComplete { get; set; }

        public Observation(decimal bid, decimal ask, decimal orderAskSum, decimal orderBidSum)
        {
            BidPrice = bid;
            AskPrice = ask;
            OrderAskSum = orderAskSum;
            OrderBidSum = orderBidSum;
            this.ObservationTime = DateTime.Now;
            IsComplete = true;
        }

        public Observation()
        {
            IsComplete = false;
        }
       
    }
}
