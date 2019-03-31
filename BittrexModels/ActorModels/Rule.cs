using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public delegate int Condition(Observation[] observations);

    public enum RuleType
    {
        ForBuy,
        ForSell
    }

    public class Rule
    {
        
        public List<Condition> Conditions { get; }
        public double Rating = 1;
        public TimeSpan TickAim;
        public RuleType Type;

       
        public Rule(RuleType ruleType)
        {
            this .Type = ruleType;
            Conditions = new List<Condition>();
        }
        public double RuleRecomendation(Observation[] observations)
        {            
            return Conditions.Sum(x => x(observations))*Rating;
        }
    }
    

}
