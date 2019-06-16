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
            db.Actors.Add(actor);
            db.SaveChanges();
        }
        public void SaveObservation(Observation obs)
        {
            db.Observations.Add(obs);
            db.SaveChanges();
        }

        public void SaveTransaction(Transaction transaction, bool isNew = false)
        {
            if (isNew)
            {
                db.Transactions.Add(transaction);
            } else
            {
                var t = db.Transactions.Where(x => transaction.Guid == x.Guid).FirstOrDefault();
                if (t == null) db.Transactions.Add(transaction);
                else
                {
                    t = transaction;
                }
            }

            db.SaveChanges();
        }

        public void SaveRule(Rule rule)
        {
            db.Rules.Add(rule);
            db.SaveChanges();
        }

        public void EditActor(Actor actor)
        {
            var eActor = db.Actors.Where(x => x.Guid == actor.Guid).Include(x => x.CountVolume).FirstOrDefault();
            eActor.HesitationToBuy = actor.HesitationToBuy;
            eActor.HesitationToSell = actor.HesitationToSell;
            eActor.LastActionTime = actor.LastActionTime;
            eActor.OperationPercent = actor.OperationPercent;
            eActor.TargetMarket = actor.TargetMarket;
            eActor.ActivationSpan = actor.ActivationSpan;
            eActor.CountVolume.BtcCount = actor.CountVolume.BtcCount;
            eActor.CountVolume.CurrencyCount = actor.CountVolume.CurrencyCount;
            eActor.CountVolume.CurrencyName = actor.CountVolume.CurrencyName;
            db.SaveChanges();
        }

    }
}
