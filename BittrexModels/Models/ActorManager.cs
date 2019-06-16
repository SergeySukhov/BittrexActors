using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BittrexModels.Interfaces;
using DataManager.Models;
using DataManager.Providers;

namespace BittrexModels.ActorModels
{
    public class ActorManager
    {        
        private IBittrexApi BittrexApi { get; }
        private ITransactionManager TransactionManager { get; }
        private BittrexDbProvider BittrexDbProvider { get; }
        List<Actor> ActiveActors;

        public ActorManager(IBittrexApi bittrexApi, BittrexDbProvider bittrexDbProvider, ITransactionManager transactionManager)
        {
            this.TransactionManager = transactionManager;
            this.BittrexApi = bittrexApi;
            this.BittrexDbProvider = bittrexDbProvider;
            ActiveActors = new List<Actor>();
        }

        public Actor CreateActor(string targetMarket, TimeSpan tickSpan, decimal startCountVol = 0.5m)
        {
            var actor = new Actor()
            {
                Guid = Guid.NewGuid(),
                CountVolume = new Account() { CurrencyName = targetMarket, BtcCount = startCountVol, CurrencyCount = 0m },
                TargetMarket = targetMarket,
                ActivationSpan = tickSpan,
                Rules = new List<Rule>(),
                Observations = new List<Observation>(),
                Transactions = new List<Transaction>(),
                HesitationToBuy = Consts.StartHesitationToBuy,
                HesitationToSell = Consts.StartHesitationToSell,
                OperationPercent = Consts.OperationPercent,
                LastActionTime = DateTime.Now
            };
            BittrexDbProvider.SaveActor(actor);
            return actor;
        }

        public void SetupActor(Actor actor, Rule rule)
        {
            actor.Rules.Add(rule);
            this.ActiveActors.Add(actor);
        }

        public void ActorsTimerAction(object sender, EventArgs e)
        {
            foreach (var actor in ActiveActors)
            {
                Task.Factory.StartNew(() =>
                {
                    if (DateTime.Now - actor.LastActionTime < actor.ActivationSpan) return;
                    else { actor.LastActionTime = DateTime.Now; }

                    Task.Factory.StartNew(() =>
                    Observe(actor).Start());

                    if (actor.Rules.Count == 0) return;

                    if (actor.Rules.Any(x => x.Type == RuleType.ForBuy))
                    {
                        CheckBuyRules(actor);
                    }

                    actor.HesitationToBuy += 0.01;


                });
                }
            
        }

        public Task Observe(Actor actor)
        {
            return new Task(async() =>
            {
                var obs = await BittrexApi.GetObservation(actor.TargetMarket);
                if (obs != null)
                {
                    actor.Observations.Add(obs);
                    BittrexDbProvider.SaveObservation(obs);
                }
                else
                {
                    // TODO notifications
                }
            });
            
        }

        public void CheckBuyRules(Actor actor)
        {
            var persuasiveness = 10.0;
            foreach (var s in actor.Rules)
            {
                persuasiveness += RuleRecomendation(s, actor.Observations.ToArray());
            }
            persuasiveness /= actor.Rules.Count;
            if (persuasiveness > actor.HesitationToBuy)
                Task.Factory.StartNew(() =>
                {
                    var transaction = TransactionManager.CreateTransaction(OperationType.Buy,
                     100m * (decimal)(actor.OperationPercent),
                         actor.TargetMarket, actor.CountVolume);
                     
                });
        }

        public double RuleRecomendation(Rule rule, Observation[] observations)
        {
            var ConditionNames = rule.ConditionSplitedNames.Split('|');

            return ConditionNames.Sum(x => Models.RulesLibrary.AllConditions[(Models.RulesLibrary.RuleNames)int.Parse(x)](observations));
        }

        /*public Actor toDbActor()
        {
            var dbActor = new Actor();
            dbActor.Observations = new List<DataManager.Models.Observation>();
            dbActor.Rules = new List<DataManager.Models.Rule>();
            dbActor.Transactions = new List<DataManager.Models.Transaction>();

            dbActor.Guid = Guid;
            dbActor.ActivationSpan = ActivationSpan;
            dbActor.TargetMarket = TargetMarket;
            dbActor.HesitationToBuy = HesitationToBuy;
            dbActor.HesitationToSell = HesitationToSell;
            dbActor.LastActionTime = LastActionTime;

            dbActor.CountVolume = new DataManager.Models.Account()
            {
                Guid = this.Guid,
                BtcCount = CountVolume.BtcCount,
                CurrencyCount = CountVolume.CurrencyCount,
                CurrencyName = CountVolume.CurrencyName,
                Actor = dbActor
            };

            Observations.ForEach(x => dbActor.Observations.Add(new DataManager.Models.Observation()
            {
                Guid = x.Guid,
                ActorGuid = this.Guid,
                ObservationTime = x.ObservationTime,
                AskPrice = x.AskPrice,
                BidPrice = x.BidPrice,
                OrderAskSum = x.OrderAskSum,
                OrderBidSum = x.OrderBidSum,
                MarketName = x.MarketName,
                Actor = dbActor

            }));

            Transactions.ForEach(x => dbActor.Transactions.Add(new DataManager.Models.Transaction()
            {
                Guid = x.Guid,
                ActorGuid = this.Guid,
                CreationTime = x.CreationTime,
                ReleaseTime = x.ReleaseTime,
                CurrencySum = x.CurrencySum,
                MarketName = x.MarketName,
                TransactionResult = (int)x.TransactionResult,
                Account = dbActor

            }));
            Rules.ForEach(x => {
                var temp = new DataManager.Models.Rule()
                {
                    Guid = x.Guid,
                    ActorGuid = this.Guid,
                    MinuteInterval = x.MinuteInterval,
                    Rating = x.Rating,
                    Type = (int)x.Type,
                    Actor = dbActor
                };

                var str = "";

                x.ConditionNames.ForEach(n => str += n.ToString() + "|");
                str.Trim('|');
                temp.ConditionSplitedNames = str;
                dbActor.Rules.Add(temp);
            });


            

            return dbActor;
        }
        */
    }

     
}
