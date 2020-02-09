using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BittrexData.Interfaces;

namespace RulesLibrary
{
    public class RuleLibrary
    {

        public delegate double Rule(ICurrencyProvider provider, DateTime currentTime);

        public readonly Dictionary<string, Rule> RulesSellDictionary = new Dictionary<string, Rule>();
        public readonly Dictionary<string, Rule> RulesBuyDictionary = new Dictionary<string, Rule>();

        public RuleLibrary()
        {
            RulesBuyDictionary.Add("rule0", rule0);
        }

        Rule rule0 = (ICurrencyProvider x, DateTime currentTime) => 
        {
            
            return 0;
        };



    }
}
