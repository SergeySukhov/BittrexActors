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
            RulesBuyDictionary.Add("rule1", rule1);
            RulesBuyDictionary.Add("rule2", rule2);

			RulesSellDictionary.Add("ruleSell0", ruleSell0);
			RulesSellDictionary.Add("ruleSell1", ruleSell1);
			RulesSellDictionary.Add("ruleSell2", ruleSell2);

		}

		// покупка валюты
		Rule rule0 = (ICurrencyProvider x, DateTime currentTime) => 
        {
            
            return 0;
        };

		Rule rule1 = (ICurrencyProvider x, DateTime currentTime) =>
		{

			return 1d;
		};

		Rule rule2 = (ICurrencyProvider x, DateTime currentTime) =>
		{

			return -0.5;
		};


		// продажа валюты

		Rule ruleSell0 = (ICurrencyProvider x, DateTime currentTime) =>
		{

			return 0;
		};

		Rule ruleSell1 = (ICurrencyProvider x, DateTime currentTime) =>
		{

			return 1d;
		};

		Rule ruleSell2 = (ICurrencyProvider x, DateTime currentTime) =>
		{

			return -0.5;
		};

	}
}
