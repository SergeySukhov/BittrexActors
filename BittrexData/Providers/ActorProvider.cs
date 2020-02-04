using System;
using System.Collections.Generic;
using System.Linq;

using BittrexData.Contexts;
using BittrexData.Models; // !!
using Microsoft.EntityFrameworkCore;

namespace BittrexData.Providers
{
    public class ActorProvider
    {
		public ActorProvider()
		{
		}

		public void TestAddingData()
		{
			var context = new BittrexActorsDbContext();

			var a = new ActorData();
			a.Rules = new List<BalancedRule>();

			a.Guid = Guid.NewGuid();
			a.HesitationToBuy = 1;
			a.HesitationToSell = 1.011111d;
			a.IsAlive = true;

			a.Rules.Add(new BalancedRule() { Guid = Guid.NewGuid(), RuleName = "hui", Type = OperationType.Buy });
			context.ActorDatas.Add(a);

			context.SaveChanges();
			context.Dispose();
		}

		public void TestLoadingData()
		{
			var context = new BittrexActorsDbContext();
			var a = context.ActorDatas.Select(x => x.HesitationToBuy > 0);
			Console.WriteLine(a.Count());
			context.Dispose();

		}

		public void TestSaving()
		{
			var context = new BittrexActorsDbContext();

			var a = new ActorData();
			a.Rules = new List<BalancedRule>();
            a.Predictions = new List<Prediction>();
			a.Guid = Guid.Parse("23E43272-E7F3-4DAA-B9F3-2A682B7DF8A3");
			a.HesitationToBuy = 2;
			a.HesitationToSell = 2.0222222d;
			a.IsAlive = true;

            var rule = new BalancedRule() { Guid = Guid.Parse("23E43272-E7F3-4DAA-B9F3-2A682B7DF8A4"), RuleName = "hui333", Type = OperationType.Buy };
            var predU = new PredictionUnit() { Guid = Guid.NewGuid(), RuleName = rule.RuleName, Coefficient = 0.22d };
            var preds = new Prediction() { Guid = Guid.NewGuid(), RulePredictions = new List<PredictionUnit>() { predU } };

            a.Predictions.Add(preds);
			a.Rules.Add(rule);

			this.SaveOrUpdateActor(a);

			var alives = this.LoadAliveActors();

            context.Dispose();

			Console.WriteLine(alives.Count);
			// context.ActorDatas.Add(a);
		}

		private bool SaveOrUpdateActor(ActorData actorData)
		{
			var context = new BittrexActorsDbContext();

			try
			{

				var savedData = context.ActorDatas.Where(actor => actor.Guid == actorData.Guid)
                    .Include(x => x.Rules)
                    .Include(x => x.Transactions)
                    .Include(x => x.Account)
                    .Include(x => x.Predictions)
                    .FirstOrDefault();

				if (savedData == null)
				{
					context.ActorDatas.Add(actorData);
				} else
				{
                    
					context.ActorDatas.Remove(savedData);
					context.ActorDatas.Add(actorData);
				}
                context.SaveChanges();
                context.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Ошибка!", ex.Message);
				if (context != null) context.Dispose();
				return false;
			}

		} 

		private List<ActorData> LoadAliveActors()
		{
			var context = new BittrexActorsDbContext();

			var aliveActors = context.ActorDatas.Where(actor => actor.IsAlive)
                .Include(x => x.Rules)
                .Include(x => x.Account)
                .Include(x => x.Transactions)
                .Include(x => x.Predictions)
                .ToList();
			context.Dispose();

			return aliveActors;
		}
	}
}
