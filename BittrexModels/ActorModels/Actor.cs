using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;

namespace BittrexModels.ActorModels
{
    public class Actor
    {
        public Guid Id { get; }
        public decimal CountVolume { get; } // BTC
        public string TargetMarket { get; }
        List<Transaction> RealisedTransactions { get; }
        List<Transaction> AwaitingTransactions { get; }
        public List<Rule> Rules { get; }

        public TimeSpan TickSpan { get; }
        public List<DayBounds> InactiveTime { get; }
        public List<Observation> Observations { get; }
        // public List<Prediction> Predictions { get; }
        public Actor(string targetMarket, TimeSpan tickSpan, decimal startCountVol = 0.5m)
        {
            Id = Guid.NewGuid();
            CountVolume = startCountVol;
            TargetMarket = targetMarket;
            TickSpan = tickSpan;
            RealisedTransactions = new List<Transaction>();
            AwaitingTransactions = new List<Transaction>();
            Rules = new List<Rule>();
            InactiveTime = new List<DayBounds>();
            Observations = new List<Observation>();
             Predictions = new List<Prediction>();
        }

        public async void ActorTimer(object sender, EventArgs e)
        {
            
            if (Rules.Count == 0 || InactiveTime.Any(x => x.isInBounds(DateTime.Now))) return;
            await Observe();


        }

        public async Task Observe()
        {
            // TODO: добавить очередь запросов: не привышать 60 запросов в минуту
            // request
            BittrexClient client = new BittrexClient("", "");
            var ordersBid = await client.GetOrderBook(this.TargetMarket, BookOrderType.Buy, 10); // !!
            var ordersAsk = await client.GetOrderBook(this.TargetMarket, BookOrderType.Sell, 10);
            var bidPrice = await client.GetMarketSummary(this.TargetMarket);


            var sumBid = ordersBid.Result.Sum(x => x.Quantity);
            var sumAsk = ordersAsk.Result.Sum(x => x.Quantity);
            var obs = new Observation(bidPrice.Result.Bid, bidPrice.Result.Ask, sumAsk, sumBid);
            this.Observations.Add(obs);
            return;
        }

        public void MakePrediction()
        {
            //!!
            //Rules.ForEach(x => { x. });

        }
        public void CheckPredictions()
        {
            //var now = DateTime.Now;

            //var resultObservation = this.Observations.Last();

            //Predictions.ForEach(x => {
            //    if (x.XTime.Subtract(now) < new TimeSpan(0, 1, 1))
            //    {
            //        var predRes = x.IsPredictWell(x.PriceType == PriceType.Ask ? resultObservation.AskPrice : resultObservation.BidPrice);
            //    }
            //});
        }

        public void MakeBet()
        {

        }
    }

    public class Observation
    {
        public readonly DateTime ObservationTime;

        public readonly decimal BidPrice = 0;
        public readonly decimal AskPrice = 0;
        public readonly decimal OrderAskSum = 0;
        public readonly decimal OrderBidSum = 0;
        public Observation(decimal bid, decimal ask, decimal orderAskSum, decimal orderBidSum)
        {
            BidPrice = bid;
            AskPrice = ask;
            OrderAskSum = orderAskSum;
            OrderBidSum = orderBidSum;
            this.ObservationTime = DateTime.Now;
        }
    }

    public class DayBounds
    {
        DateTime StartBound { get; }
        TimeSpan Duration { get; }
        public readonly List<DayOfWeek> exceptionDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

        public DayBounds(DateTime startBound, TimeSpan duration)
        {
            this.StartBound = startBound;
            this.Duration = duration;
        }

        public bool isInBounds()
        {
            return DateTime.Now.Subtract(this.StartBound) <= this.Duration && !exceptionDays.Contains(DateTime.Now.DayOfWeek);
        }

        public bool isInBounds(DateTime dateTime)
        {
            return dateTime.Subtract(this.StartBound) <= this.Duration && !exceptionDays.Contains(dateTime.DayOfWeek);
        }

    }
    #region predictions
    //public enum PriceType { Ask, Bid }

    //public class Prediction
    //{
    //    public DateTime XTime { get; }
    //    public PriceType PriceType { get; }

    //    decimal PredictPrice;
    //    decimal Inaccuracy;
    //    public Prediction(decimal predictPrice, PriceType priceType, decimal inaccuracy = 0)
    //    {
    //        XTime = DateTime.Now;
    //        PriceType = priceType;
    //        this.PredictPrice = predictPrice;
    //        this.Inaccuracy = Math.Abs(inaccuracy);
    //    }
    //    public decimal PredictionDelta(decimal actualPrice)
    //    {

    //        return PredictPrice - actualPrice;
    //    }
    //    public bool IsPredictWell(decimal actualPrice)
    //    {
    //        return Math.Abs(PredictPrice - actualPrice) > Inaccuracy;
    //    }
    //}


    #endregion
}
