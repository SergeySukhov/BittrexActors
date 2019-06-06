using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Interfaces
{
    public interface IObservation
    {
        DateTime ObservationTime { get; set; }

        decimal BidPrice { get; set; }
        decimal AskPrice { get; set; }
        decimal OrderAskSum { get; set; }
        decimal OrderBidSum { get; set; }
    }
}
