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

			a.Guid = Guid.Parse("bcd02af5-620b-446a-87bb-7c4eea949bc7");
			a.HesitationToBuy = 2;
			a.HesitationToSell = 2.0222222d;
			a.IsAlive = true;

			a.Rules.Add(new BalancedRule() { Guid = Guid.NewGuid(), RuleName = "hui2", Type = OperationType.Buy });

			this.SaveOrUpdateActor(a);

			var alives = this.LoadAliveActors();
			Console.WriteLine(alives.Count);
			// context.ActorDatas.Add(a);
		}

		private bool SaveOrUpdateActor(ActorData actorData)
		{
			var context = new BittrexActorsDbContext();

			try
			{

				var savedData = context.ActorDatas.Where(actor => actor.Guid == actorData.Guid).Include(x => x.Rules).FirstOrDefault();

				if (savedData == null)
				{
					context.ActorDatas.Add(savedData);
					context.SaveChanges();
				} else
				{
					context.ActorDatas.Remove(savedData);
					context.ActorDatas.Add(actorData);
					context.SaveChanges();
				}

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

			var aliveActors = context.ActorDatas.Where(actor => actor.IsAlive).Include(x => x.Rules).ToList();
			context.Dispose();

			return aliveActors;
		}
	}
}
