using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class Prediction
    {
        public Guid Guid;
        public List<KeyValuePair<BalancedRule, double>> RulePredictions;

    }
}
