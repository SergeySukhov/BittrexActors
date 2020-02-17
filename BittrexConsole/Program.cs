using System;

using RulesLibrary;
using BittrexData;
using BittrexCore;
using BittrexCore.Models;
using System.Threading.Tasks;
using System.Threading;

namespace BittrexConsole
{
    class Program
    {
		static void Main(string[] args)
		{
			Console.WriteLine("!! Hello, start initialization...");

			RuleLibrary r = new RuleLibrary();

			var dataManager = new DataManager();

			//dataManager.actorProvider.TestAddingData();
			//dataManager.actorProvider.TestLoadingData();

			//dataManager.ActorProvider.TestSaving();


			var actor = new Actor(dataManager.CurrencyProvider, r);

			actor.CurrentTime = new DateTime(2018, 1, 1, 1, 1, 1);

			var xx = actor.CurrentTime.Date.ToString();

			actor.Data.Account.BtcCount = 0.001m;
			actor.Data.Account.CurrencyName = "ETH";
			actor.Data.Account.CurrencyCount = 0m;

			actor.Data.ActorType = ActorType.HalfDaily;
			actor.Data.ChangeCoefficient = 0.01;
			actor.Data.HesitationToSell = 0.5;
			actor.Data.HesitationToBuy = 0.5;
			actor.Data.IsAlive = true;
			actor.Data.LastActionTime = actor.CurrentTime - new TimeSpan(6, 1, 0);

			actor.Data.Rules.Add(new BalancedRule("rule0", OperationType.Buy) { Guid = Guid.NewGuid(), Coefficient = 0.5 }); // поменять строки на enum
			actor.Data.Rules.Add(new BalancedRule("rule1", OperationType.Buy) { Guid = Guid.NewGuid(), Coefficient = 0.5 });
			actor.Data.Rules.Add(new BalancedRule("rule2", OperationType.Buy) { Guid = Guid.NewGuid(), Coefficient = 0.5 });

			actor.Data.Rules.Add(new BalancedRule("ruleSell0", OperationType.Sell) { Guid = Guid.NewGuid(), Coefficient = 0.4 });
			actor.Data.Rules.Add(new BalancedRule("ruleSell1", OperationType.Sell) { Guid = Guid.NewGuid(), Coefficient = 0.4 });
			actor.Data.Rules.Add(new BalancedRule("ruleSell2", OperationType.Sell) { Guid = Guid.NewGuid(), Coefficient = 0.4 });

			actor.Data.Generation = 0;

			// actor.Data.

			Task.Run(() => {
				int i = 0;
				while(actor.Data.IsAlive)
				{
					actor.DoWork();

					if (i%10 == 0) Console.WriteLine(actor.GetInfo());
					actor.CurrentTime += new TimeSpan(1, 0, 0); // в обучающей модели

					Thread.Sleep(600);
					i++;
				}
			});


			Console.WriteLine("Finished!!");
			Console.ReadKey();
        }
    }
}
