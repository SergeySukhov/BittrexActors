using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public class Observation
    {
        public readonly DateTime ObservationTime;

        public readonly decimal BidPrice = 0;
        public readonly decimal AskPrice = 0;
        public readonly decimal OrderAskSum = 0;
        public readonly decimal OrderBidSum = 0;
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
