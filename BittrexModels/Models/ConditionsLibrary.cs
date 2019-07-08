using System;
using System.Collections.Generic;
using System.Linq;

using DataManager.Models;


namespace BittrexModels.Models
{
    public class ConditionsLibrary
    {
        public delegate double Condition(Observation[] observations);


        public static readonly Dictionary<ConditionsNames, Condition> AllConditions = new Dictionary<ConditionsNames, Condition>();

        public enum ConditionsNames
        {
            FirstRuleBuy30min,
            ExtrimelyRuleSell30min,
        }

        public ConditionsLibrary()
        {
            InitFirstRuleBuy30min();
            InitExtrimelyRuleSell30min();
        }

        private void InitFirstRuleBuy30min()
        {
            Condition t = (obs) => {
                var ObsList = obs.ToList();
                int obsCount = ObsList.Count;

                int watchObsIndex = 5;

                if (obsCount < watchObsIndex || !CheckIntervalOfTwo(ObsList, 30)) return 0;

                var positive_rel = 0;

                for (int i = watchObsIndex - 1; i > 0; i--)
                {
                    if (ObsList[obsCount - watchObsIndex].BidPrice / ObsList[obsCount - i].BidPrice < 1.0m) positive_rel++;
                }

                return positive_rel / (watchObsIndex - 1);
            };
            AllConditions.Add(ConditionsNames.FirstRuleBuy30min, t);

        }

        private void InitExtrimelyRuleSell30min()
        {
            //var extrimelyRuleSell30min = new Rule()
            //{

            //    Guid = Guid.NewGuid(),
            //    Rating = 1,
            //    Type = RuleType.ForSell,
            //};
            //extrimelyRuleSell30min.MinuteInterval = 30;
            //extrimelyRuleSell30min.ConditionSplitedNames += ConditionsNames.ExtrimelyRuleSell30min;

            Condition t = (obs) => {
                var ObsList = obs.ToList();
                int obsCount = ObsList.Count;
                int positive_rel = 0;
                int watchObsIndex = 3;
                if (obsCount < watchObsIndex || !CheckIntervalOfTwo(ObsList, 30)) return 0;

                for (int i = watchObsIndex - 1; i > 0; i--)
                {
                    if (ObsList[obsCount - i].BidPrice / ObsList[obsCount - watchObsIndex].BidPrice < 0.9m) positive_rel++;
                }

                return positive_rel / (watchObsIndex - 1);
            };
            AllConditions.Add(ConditionsNames.ExtrimelyRuleSell30min, t);
        }


        private static bool CheckIntervalOfTwo(List<Observation> lsObs, int requiredSpanMinutes)
        {
            if (lsObs.Count < 2 ||
                (lsObs[lsObs.Count - 2].ObservationTime - lsObs[lsObs.Count - 1].ObservationTime).Minutes < requiredSpanMinutes - Consts.ObservationTimeInaccuracy)
                return false;
            return true;
        }
    }
}