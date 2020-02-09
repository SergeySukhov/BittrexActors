using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class Prediction
    {
        public Guid Guid;
        public DateTime ForTime;
        public decimal OldPrice;
        public readonly Dictionary<string, double> RulePredictions;

        public Prediction()
        {
            Guid = Guid.NewGuid();

            RulePredictions = new Dictionary<string, double>();
        }
    }
}
