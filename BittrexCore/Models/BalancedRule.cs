using BittrexData;
using RulesLibrary;
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
		/// <summary>
		/// (0,1)
		/// </summary>
        public double Coefficient;
        public OperationType Type;

        public BalancedRule(string ruleName, OperationType operationType)
        {
            Guid = Guid.NewGuid();
			Coefficient = 0.5;

            RuleName = ruleName;
            Type = operationType;
        }

        
    }
}
