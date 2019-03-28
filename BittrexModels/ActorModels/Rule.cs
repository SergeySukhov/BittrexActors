using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public delegate int Condition(Observation[] observations);

    public class Rule
    {
        
        public List<Condition> Conditions { get; }
        public double Rating = 1;
        public TimeSpan TickAim;

       
        public Rule(TimeSpan tickAim)
        {
            TickAim = tickAim;
            Conditions = new List<Condition>();
        }
        public double RuleRecomendation(Observation[] observations, TimeSpan timeSpan)
        {
            if (timeSpan < TickAim) return 0;
            return Conditions.Sum(x => x(observations))*Rating;
        }
    }
    

}
