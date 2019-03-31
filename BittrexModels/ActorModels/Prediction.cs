using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public enum PriceType { Ask, Bid }

    public class Prediction
    {
        public DateTime XTime { get; }
        public PriceType PriceType { get; }

        decimal PredictPrice;
        decimal Inaccuracy;
        public Prediction(decimal predictPrice, PriceType priceType, decimal inaccuracy = 0)
        {
            XTime = DateTime.Now;
            PriceType = priceType;
            this.PredictPrice = predictPrice;
            this.Inaccuracy = Math.Abs(inaccuracy);
        }
        public decimal PredictionDelta(decimal actualPrice)
        {

            return PredictPrice - actualPrice;
        }
        public bool IsPredictWell(decimal actualPrice)
        {
            return Math.Abs(PredictPrice - actualPrice) > Inaccuracy;
        }
    }
}
