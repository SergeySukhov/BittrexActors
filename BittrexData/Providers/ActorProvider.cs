using System;
using System.Collections.Generic;
using System.Linq;

using BittrexData.Contexts;
using BittrexData.Models; // !!

namespace BittrexData.Providers
{
    public class ActorProvider
    {
		// private readonly BittrexActorsDbContext context;
		public ActorProvider()
		{
			//this.context = new BittrexActorsDbContext();
		}

		public void TestAddingData()
		{
			var context = new BittrexActorsDbContext();

			var a = new ActorData();
			a.Guid = Guid.NewGuid();
			a.HesitationToBuy = 1;
			a.HesitationToSell = 1.011111d;

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
	}
}
