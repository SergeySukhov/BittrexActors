using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels
{
    public static class Consts
    {
        public static readonly int BittrexRequestLimit = 55;

        // Actors consts

        public static readonly double StartHesitationToSell = 1.0;
        public static readonly double StartHesitationToBuy = 1.0;
        public static readonly double OperationPercent = 0.05;
        public static readonly decimal StartCountVolumeBtc = 0.5m;
        public static readonly int ObservationTimeInaccuracy = 5;
    }
}
