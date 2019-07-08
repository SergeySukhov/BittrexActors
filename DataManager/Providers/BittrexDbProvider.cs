using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataManager.Models;

namespace DataManager.Providers
{
    public class BittrexDbProvider
    {
        private BittrexDbContext db;

        public BittrexDbProvider()
        {
            db = new BittrexDbContext();
        }

        public void SaveActor(Actor actor)
        {
            var context = new BittrexDbContext();

            context.Actors.Where(x => x.Guid == actor.Guid).Include(xx => xx.CountVolume).Load();

            var oldActor = context.Actors.Where(x => x.Guid == actor.Guid).FirstOrDefault();

            if (oldActor == null)
            {
                context.Actors.Add(actor);
            }
            else
            {
                oldActor.LastActionTime = actor.LastActionTime;
                oldActor.HesitationToBuy = actor.HesitationToBuy;
                oldActor.HesitationToSell = actor.HesitationToSell;
                oldActor.ActivationSpan = actor.ActivationSpan;
                oldActor.CountVolume.BtcCount = actor.CountVolume.BtcCount;
                oldActor.CountVolume.CurrencyCount = actor.CountVolume.CurrencyCount;
                oldActor.OperationPercent = actor.OperationPercent;

                context.Entry(oldActor).State = EntityState.Modified;
                context.Entry(oldActor.CountVolume).State = EntityState.Modified;
            }

            context.SaveChanges();
            context.Dispose();
        }
        public void SaveObservation(Observation obs)
        {
            var context = new BittrexDbContext();

            context.Observations.Add(obs);
            context.SaveChanges();

            context.Dispose();
        }

        public void SaveTransaction(Transaction transaction)
        {
            var context = new BittrexDbContext();
            context.Transactions.Where(x => transaction.Guid == x.Guid).Load();

            var oldTransaction = context.Transactions.Where(x => transaction.Guid == x.Guid).FirstOrDefault();

            if (oldTransaction == null) context.Transactions.Add(transaction);
            else
            {
                oldTransaction.ReleaseTime = transaction.ReleaseTime;
                oldTransaction.TransactionResult = transaction.TransactionResult;
                oldTransaction.ReleasePrice = transaction.ReleasePrice;

                context.Entry(oldTransaction).State = EntityState.Modified;
            }

            context.SaveChanges();

            context.Dispose();
        }

        public void SaveRule(Rule rule)
        {
            var context = new BittrexDbContext();

            context.Rules.Where(x => x.Guid == rule.Guid).Load();

            var oldRule = context.Rules.Where(x => x.Guid == rule.Guid).FirstOrDefault();
            if (oldRule == null) context.Rules.Add(rule);
            else
            {
                oldRule.MinuteInterval = rule.MinuteInterval;
                oldRule.Rating = rule.Rating;
                oldRule.ConditionSplitedNames = oldRule.ConditionSplitedNames;

                context.Entry(oldRule).State = EntityState.Modified;
            }
            context.SaveChanges();
            context.Dispose();
        }

        public Actor[] LoadActorModels()
        {
            var context = new BittrexDbContext();
            context.Actors.Include(xx => xx.CountVolume).Load();
            context.Transactions.Load();
            context.Observations.Load();
            context.Rules.Load();

            var actors = context.Actors.Local.ToArray();

            foreach (var actor in actors)
            {
                var transactions = context.Transactions.Local.Where(x => x.AccountGuid == actor.CountVolume.Guid).ToList();
                foreach (var transaction in transactions) transaction.Account = actor.CountVolume;
                actor.Transactions = transactions;

                var observations = context.Observations.Local.Where(x => x.ActorGuid == actor.Guid).ToList();
                actor.Observations = observations;

                var rules = context.Rules.Local.Where(x => x.ActorGuid == actor.Guid).ToList();
                actor.Rules = rules;
            }

            // context.Dispose();

            return actors.ToArray();
        }

        public Actor LoadActor(Guid actorGuid)
        {
            var context = new BittrexDbContext();

            context.Actors.Where(x => x.Guid == actorGuid).Include(xx => xx.CountVolume).Load();

            var actor = context.Actors.Local.FirstOrDefault();

            if (actor != null)
            {
                context.Transactions.Where(x => x.AccountGuid == actor.CountVolume.Guid).Load();
                var transactions = context.Transactions.Local.ToList();
                foreach (var transaction in transactions) transaction.Account = actor.CountVolume;
                actor.Transactions = transactions;

                context.Observations.Where(x => x.ActorGuid == actor.Guid).Load();
                var observations = context.Observations.Local.ToList();
                actor.Observations = observations;

                context.Rules.Where(x => x.ActorGuid == actor.Guid).Load();
                var rules = context.Rules.Local.ToList();
                actor.Rules = rules;
            }

            context.Dispose();

            return actor;
        }

    }
}