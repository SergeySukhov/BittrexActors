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

			Console.WriteLine("!! Actor Manger initiate...");
			ActorProvider = actorProvider;
			CurrencyProvider = currencyProvider;
			ActorFactory = new ActorFactory(actorProvider);

			Console.WriteLine("!!Spawn first generation...");
			// Проверки систем
			// запуск акторов
			SpawnGeneration();

			Task.Run(() => {

				while (AllActors.Count > 0)
				{
					if (LastGeneration > 1 && AllActors.Count < 2 || AllActors.Any(x => x.Data.Generation == LastGeneration && x.Data.CurrentTime - Const.StartActorTime > Const.NewGenerationSpawnDelay))
					{
						var oldActorsCount = AllActors.Count;
						SpawnGeneration();
						Console.WriteLine($"!! New generation spawned; Added actors: {AllActors.Count - oldActorsCount}");
												
					}
				}
				Console.WriteLine("!! All actors are dead");
			});
		}

		public void RunActor(Actor actor)
		{
				
			if (!AllActors.Any(x => x.Guid == actor.Guid)) AllActors.Add(actor);
			
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

					if (i % 30 == 0) Console.WriteLine(actor.GetInfo());
					actor.Data.CurrentTime += new TimeSpan(1, 0, 0); // в обучающей модели

					//Thread.Sleep(100);
					i++;

					await ActorFactory.SaveActor(actor);
				}

			});

			AllActorProcesses.Add(task);
			task.Start();
		}

		public void SpawnGeneration()
		{
            LastGeneration++;

            Console.WriteLine($"!! Spawn generation: {LastGeneration}");

			if (LastGeneration == 1)
			{
				var actor = ActorFactory.CreateActor(CurrencyProvider, new RuleLibrary12Hour(), BittrexData.ActorType.HalfDaily, "ETH", null, null);
                actor.Data.Generation = LastGeneration;

				RunActor(actor);
			} else
			{
                var newActors = new List<Actor>();

				foreach(var oldActor in AllActors)
				{
					var rulesAboveAverage = oldActor.Data.Rules.Where(x => x.Coefficient > 0.1);
					if (rulesAboveAverage != null && rulesAboveAverage.Count() > 0)
					{
						var actorNewGen = ActorFactory.CreateActor(CurrencyProvider, new RuleLibrary12Hour(),
							BittrexData.ActorType.HalfDaily, oldActor.Data.Account.CurrencyName,
							rulesAboveAverage.Where(x => x.Type == BittrexData.OperationType.Buy).Select(x => x.RuleName).ToArray(),
							rulesAboveAverage.Where(x => x.Type == BittrexData.OperationType.Sell).Select(x => x.RuleName).ToArray());
                        actorNewGen.Data.Generation = LastGeneration;

                        newActors.Add(actorNewGen);
					}
				}

                foreach (var s in newActors) RunActor(s);

            }

        }

		public void InspectGeneration()
		{

		}

    }
}
