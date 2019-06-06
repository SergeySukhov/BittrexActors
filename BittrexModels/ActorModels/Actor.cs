using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;
using BittrexModels.Interfaces;
using BittrexModels.Models;

namespace BittrexModels.ActorModels
{
    public class Actor
    {
        public Guid Id { get; }
        public IAccount CountVolume { get; } // BTC
        public string TargetMarket { get; }
        List<ITransaction> RealisedTransactions { get; }
        List<ITransaction> AwaitingTransactions { get; }
        public List<Rule> Rules { get; }

        public TimeSpan ActivitySpan { get; }
        public List<DayBounds> InactiveTime { get; }
        public List<IObservation> Observations { get; }

        private double HesitationToBuy { get; set; }
        private double HesitationToSell { get; set; }
        private double OperationPercent { get; set; }
        private DateTime LastAction { get; set; }
        private IBittrexApi BittrexApi { get; }
        private ITransactionManager TransactionManager { get; }

        public Actor(string targetMarket, TimeSpan tickSpan, IBittrexApi bittrexApi, ITransactionManager transactionManager, decimal startCountVol = 0.5m)
        {
            Id = Guid.NewGuid();
            CountVolume = new Account() { CurrencyName = "", BtcCount = startCountVol, CurrencyCount = 0m };
            TargetMarket = targetMarket;
            ActivitySpan = tickSpan;
            Rules = new List<Rule>();
            InactiveTime = new List<DayBounds>();
            Observations = new List<IObservation>();

            HesitationToBuy = Consts.StartHesitationToBuy;
            HesitationToSell = Consts.StartHesitationToSell;
            OperationPercent = Consts.OperationPercent;

            this.TransactionManager = transactionManager;
            this.BittrexApi = bittrexApi;

        }

        public void ActorTimerAction(object sender, EventArgs e)
        {
            
            if (InactiveTime.Any(x => x.isInBounds(DateTime.Now))) return;
            if (DateTime.Now - LastAction < this.ActivitySpan) return;
            else { LastAction = DateTime.Now; }

            Task.Factory.StartNew(() => Observe().Start());            

            if (Rules.Count == 0) return;

            //if (Rules.Any(x => x.Type == RuleType.ForBuy))
            //{
            //    CheckBuyRules();
            //}
        }

        public Task Observe()
        {
            return new Task(async() =>
            {
                var obs = await BittrexApi.GetObservation(this.TargetMarket);
                if (obs != null) this.Observations.Add(obs);
                else
                {
                    // TODO notifications
                }
            });
            
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
                Task.Factory.StartNew(() =>
                {
                    TransactionManager.CreateTransaction(OperationType.Buy,
                     100m * (decimal)(OperationPercent),
                         this.TargetMarket, this.CountVolume);
                });
        }
    }

     
}
