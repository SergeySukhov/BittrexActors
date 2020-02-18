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

			RuleLibrary r = new RuleLibrary();

			var dataManager = new DataManager();


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

			Task.Run(() =>
			{
				int i = 0;
				while (actor.Data.IsAlive)
				{
					actor.DoWork();

					if (i % 10 == 0) Console.WriteLine(actor.GetInfo());
					actor.CurrentTime += new TimeSpan(1, 0, 0); // в обучающей модели

					Thread.Sleep(600);
					i++;
				}
			});


			//var config = new MapperConfiguration(cfg => {
			//	cfg.CreateMap<Account, BittrexData.Models.Account>();
			//	cfg.CreateMap<Transaction, BittrexData.Models.Transaction>();

			//	cfg.CreateMap<ActorData, BittrexData.Models.ActorData>()
			//	.ForMember(x => x.Account, x => x.MapFrom(m => m.Account))
			//	.ForMember(x => x.Transactions, x => x.MapFrom(m => m.Transactions));


			//	});
			//var mapper = new Mapper(config);

			//var dataDto = mapper.Map<BittrexData.Models.ActorData>(actor.Data);

			// +
			//var config = new MapperConfiguration(cfg => { cfg.CreateMap<Account, BittrexData.Models.Account>(); });
			//var mapper = new Mapper(config);

			//var accDto = mapper.Map<Account, BittrexData.Models.Account>(actor.Data.Account);

			var config = new MapperConfiguration(cfg => { cfg.CreateMap<BalancedRule, BittrexData.Models.BalancedRule>(); });
			var mapper = new Mapper(config);

			var accDto = mapper.Map<List<BalancedRule>, List<BittrexData.Models.BalancedRule>>(actor.Data.Rules);


			Console.WriteLine("Finished!!");
			Console.ReadKey();
		}
    }
}
