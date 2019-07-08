using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BittrexModels.Interfaces;
using BittrexModels.Models;
using DataManager.Models;
using DataManager.Providers;

namespace BittrexModels.ActorModels
{
    public class ActorManager
    {
        private IBittrexApi BittrexApi { get; }
        private ITransactionManager TransactionManager { get; }
        private BittrexDbProvider BittrexDbProvider { get; } // TODO: interface
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

        public void SetupActor(Actor actor, Rule[] rules)
        {
            foreach (var rule in rules)
            {
                actor.Rules.Add(rule);
                rule.ActorGuid = actor.Guid;
                BittrexDbProvider.SaveRule(rule);
            }
            this.ActiveActors.Add(actor);
        }

        public void LoadAllActors()
        {
            var allActors = BittrexDbProvider.LoadActorModels();
            foreach (var loadedActor in allActors)
            {
                if (loadedActor != null && !this.ActiveActors.Select(x => x.Guid).Contains(loadedActor.Guid))
                {
                    // TransactionManager.InitiateLoadTransactions(loadedActor.Transactions.ToArray());
                    this.ActiveActors.Add(loadedActor);
                }
            }


        }

        public Actor LoadActor(Guid guid)
        {
            var loadedActor = BittrexDbProvider.LoadActor(guid);
            if (loadedActor != null && !this.ActiveActors.Select(x => x.Guid).Contains(loadedActor.Guid))
            {
                this.ActiveActors.Add(loadedActor);
                TransactionManager.InitiateLoadTransactions(loadedActor.Transactions.ToArray());
            }

            return loadedActor;
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

                    BittrexDbProvider.SaveActor(actor);
                });
            }
        }

        public Task Observe(Actor actor)
        {
            return new Task(async () =>
            {
                var obs = await BittrexApi.GetObservation(actor.TargetMarket);
                if (obs != null)
                {
                    actor.Observations.Add(obs);
                    obs.ActorGuid = actor.Guid;
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
            {
                var transaction = TransactionManager
                    .CreateTransaction(OperationType.Buy, 100m * (decimal)(actor.OperationPercent), actor.TargetMarket, actor.CountVolume);
                actor.Transactions.Add(transaction);
            }
        }

        public double RuleRecomendation(Rule rule, Observation[] observations)
        {
            var ConditionNames = rule.ConditionSplitedNames.Split('|');

            return ConditionNames.Sum(x => ConditionsLibrary.AllConditions[(ConditionsLibrary.ConditionsNames)int.Parse(x)](observations));
        }
    }

}

