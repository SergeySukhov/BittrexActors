using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;
using BittrexCore.Models;

namespace BittrexModels.ActorModels
{
    public class Actor
    {
        public static List<Actor> AllActors = new List<Actor>();

        public Guid Guid { get; }
        public Account CountVolume { get; } // BTC - Currency
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

        private DateTime LastTick { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetMarket"> целевой магазин (валюта в которой идет торговля)</param>
        /// <param name="tickSpan">периодичность действий актора (сек.)</param>
        /// <param name="startCountVol">начальные объем валюты (BTC)</param>
        public Actor(string targetMarket, int tickSpan = 10, decimal startCountVol = 0.5m)
        {
            Guid = Guid.NewGuid();
            CountVolume = new Account() { TargetCurrencyName = "", BtcCount = startCountVol, TargetCurrencyCount = 0m };
            TargetMarket = targetMarket;
            TickSpan = new TimeSpan(tickSpan* 10000000);
            LastTick = DateTime.Now - TickSpan;
            Rules = new List<Rule>();
            InactiveTime = new List<DayBounds>();
            Observations = new List<Observation>();

            HesitationToBuy = Consts.StartActorBuyHesitation;
            HesitationToSell = Consts.StartActorSellHesitation;
            AllActors.Add(this);
        }

        /// <summary>
        /// Переодически вызываемая функция актора для всех операций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ActorTimer(object sender, EventArgs e)
        {
            // ассинхронность {}
            if (InactiveTime.Any(x => x.isInBounds(DateTime.Now)) || DateTime.Now - this.LastTick < this.TickSpan) return;
            LastTick = DateTime.Now;
            await Observe();

            if (Rules.Count == 0) return;

            if (Rules.Any(x => x.Type == RuleType.ForBuy))
            {
                CheckBuyRules();
            }


        }

        public async Task Observe()
        {
            var obs = await Observation.MakeObservation(this.TargetMarket);
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
            var persuasiveness = Rules.Sum(x => x.RuleRecomendation(this.Observations.ToArray())) / Rules.Count;
           
            if (persuasiveness > HesitationToBuy)
            {
                var transSum = CountVolume.BtcCount * (decimal)(Consts.OperationCommisionPercent * (persuasiveness - HesitationToBuy));
                if (transSum > Consts.MinimalOperationSum)
                    Transaction.CreateTransaction(OperationType.Buy, this,
                         CountVolume.BtcCount * (decimal)(Consts.OperationCommisionPercent * (persuasiveness - HesitationToBuy)));
                
            }
        }
    }


}
