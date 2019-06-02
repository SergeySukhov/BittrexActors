using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Interfaces
{
    public interface IObservation
    {
        DateTime ObservationTime { get; }

        decimal BidPrice { get; }
        decimal AskPrice { get; }
        decimal OrderAskSum { get; }
        decimal OrderBidSum { get; }
    }
}
