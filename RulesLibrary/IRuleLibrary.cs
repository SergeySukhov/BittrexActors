using BittrexData.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RulesLibrary
{
	public delegate double Rule(ICurrencyProvider provider, DateTime currentTime);

	public interface IRuleLibrary
	{
		Dictionary<string, Rule> RulesSellDictionary { get; }
		Dictionary<string, Rule> RulesBuyDictionary { get; }
	}
}
