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
        public Account CountVolume { get; } // BTC
        public string TargetMarket { get; }
        List<Transaction> RealisedTransactions { get; }
        List<Transaction> AwaitingTransactions { get; }
        public List<Rule> Rules { get; }

        public TimeSpan TickSpan { get; }
        public List<DayBounds> InactiveTime { get; }
        public List<Observation> Observations { get; }
        public List<Prediction> Predictions { get; }

        private double HesitationToBuy { get; set; }
        private double HesitationToSell { get; set; }
        private double OperationPercent { get; set; }

        public Actor(string targetMarket, TimeSpan tickSpan, decimal startCountVol = 0.5m)
        {
            Id = Guid.NewGuid();
            CountVolume = new Account() { TargetCurrencyName = "", BtcCount = startCountVol, TargetCurrencyCount = 0m };
            TargetMarket = targetMarket;
            TickSpan = tickSpan;
            Rules = new List<Rule>();
            InactiveTime = new List<DayBounds>();
            Observations = new List<Observation>();

            HesitationToBuy = 1.0;
            HesitationToSell = 1.0;
            OperationPercent = 0.1;

        }

        public async void ActorTimer(object sender, EventArgs e)
        {

            if (InactiveTime.Any(x => x.isInBounds(DateTime.Now))) return;
            await Observe();

            if (Rules.Count == 0) return;

            if (Rules.Any(x => x.Type == RuleType.ForBuy))
            {
                CheckBuyRules();
            }


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

        public void CheckBuyRules()
        {
            var persuasiveness = 0.0;
            foreach (var s in Rules)
            {
                persuasiveness += s.RuleRecomendation(this.Observations.ToArray());
            }
            persuasiveness /= Rules.Count;
            if (persuasiveness > HesitationToBuy)
               Transaction.CreateTransaction(OperationType.Buy, this,
                    CountVolume.BtcCount * (decimal)(OperationPercent * (persuasiveness - HesitationToBuy)));


        }

        public void ChangeCount(Transaction transaction, decimal sum)
        {
            if (transaction.Type == OperationType.Buy && this.CountVolume.BtcCount - sum >= 0)
                this.CountVolume.BtcCount -= sum;
            else if (this.CountVolume.BtcCount - sum < 0) throw new Exception("not enough money");
            else this.CountVolume.BtcCount += sum;
        }

    }

     
}
