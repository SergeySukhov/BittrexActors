using BittrexData.Interfaces;
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
		}

		public void SpawnGeneration()
		{
			if (LastGeneration == 0)
			{
				Console.WriteLine("!! Spawn first generation");



				return;
			}
		}

		public void InspectGeneration()
		{

		}

    }
}
