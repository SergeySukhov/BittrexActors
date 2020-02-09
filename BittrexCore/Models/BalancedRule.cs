using BittrexData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class BalancedRule
    {
        public Guid Guid;
        public string RuleName;
        public double Coefficient;
        public OperationType Type;

        public BalancedRule(string ruleName, OperationType operationType)
        {
            Guid = Guid.NewGuid();

            RuleName = ruleName;
            Type = operationType;
        }

        
    }
}
