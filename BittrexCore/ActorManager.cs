using BittrexData.Interfaces;
using RulesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BittrexCore
{
    public class ActorManager
    {
		public readonly List<Actor> AllActors = new List<Actor>();
		public readonly List<Task> AllActorProcesses = new List<Task>();

		public ActorFactory ActorFactory;

		private bool IsInitiated = false;
		private ICurrencyProvider CurrencyProvider;
		private IActorProvider ActorProvider;

		private int LastGeneration = 0;

		public void Initiate(ICurrencyProvider currencyProvider, IActorProvider actorProvider)
		{

		}

		public void RunActor(Actor actor)
		{
			var task = new Task(async () =>
			{
				int i = 0;
				while (actor.Data.IsAlive)
				{
					try
					{
						actor.DoWork();

					} catch (Exception ex)
					{
						Console.WriteLine("!! error" + ex.Message);
					}

					if (i % 100 == 0) Console.WriteLine(actor.GetInfo());
					actor.Data.CurrentTime += new TimeSpan(1, 0, 0); // в обучающей модели

					//Thread.Sleep(500);
					i++;

					await ActorFactory.SaveActor(actor);
				}
			});

			AllActorProcesses.Add(task);
			task.Start();
		}

		public void SpawnGeneration()
		{
			Console.WriteLine($"!! Spawn generation: {LastGeneration}");

			if (LastGeneration == 0)
			{
				var actor = ActorFactory.CreateActor(CurrencyProvider, new RuleLibrary12Hour(), BittrexData.ActorType.HalfDaily, "ETH", null, null);
				AllActors.Add(actor);
				RunActor(actor);
			} else
			{
				
				foreach(var oldActor in AllActors)
				{
					var rulesAboveAverage = oldActor.Data.Rules.Where(x => x.Coefficient > 0.6);
					if (rulesAboveAverage != null && rulesAboveAverage.Count() > 0)
					{
						var actorNewGen = ActorFactory.CreateActor(CurrencyProvider, new RuleLibrary12Hour(),
							BittrexData.ActorType.HalfDaily, oldActor.Data.Account.CurrencyName,
							rulesAboveAverage.Where(x => x.Type == BittrexData.OperationType.Buy).Select(x => x.RuleName).ToArray(),
							rulesAboveAverage.Where(x => x.Type == BittrexData.OperationType.Sell).Select(x => x.RuleName).ToArray());

						AllActors.Add(actorNewGen);
						RunActor(actorNewGen);
					}
				}
			}

			LastGeneration++;
		}

		public void InspectGeneration()
		{

		}

    }
}
