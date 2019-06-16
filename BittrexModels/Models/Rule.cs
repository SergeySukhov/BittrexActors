using BittrexModels.Interfaces;
using BittrexModels.Models;
using DataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public delegate double Condition(Observation[] observations);

    public class Rule : DataManager.Models.Rule
    {
        private List<RulesLibrary.RuleNames> ConditionNames { get; set; }
        
        public Rule(RuleType ruleType)
        {
            Guid = Guid.NewGuid();
            Rating = 1;
            Type = ruleType;
            ConditionNames = new List<RulesLibrary.RuleNames>();
        }

        public void AddNewCondition(RulesLibrary.RuleNames ruleName)
        {
            ConditionNames.Add(ruleName);
            ConditionSplitedNames += ConditionSplitedNames == "" ? "" : "|" + ruleName.ToString();
        }

        public double RuleRecomendation(Observation[] observations)
        {
            return ConditionNames.Sum(x => RulesLibrary.AllConditions[x](observations));
        }

    }
}    


