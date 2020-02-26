using System;

using RulesLibrary;
using BittrexData;
using BittrexCore;
using BittrexCore.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using System.Collections.Generic;

namespace BittrexConsole
{
    class Program
    {
		static void Main(string[] args)
		{
			Console.WriteLine("!! Hello, start initialization...");

			IRuleLibrary r = new RuleLibrary12Hour();

			var dataManager = new DataManager();
					   
			var factory = new ActorFactory(dataManager.ActorProvider);

			var actorManager = new ActorManager();

			actorManager.Initiate(dataManager.CurrencyProvider, dataManager.ActorProvider);

			// Console.WriteLine("Finished!!");
			Console.ReadKey();
		}
    }
}
