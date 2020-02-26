using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BittrexCore.Models;
using BittrexData;
using BittrexData.Interfaces;
using RulesLibrary;

namespace BittrexCore
{
	public class ActorFactory
	{
		private IActorProvider actorProvider;


		public ActorFactory(IActorProvider actorProvider)
		{
			this.actorProvider = actorProvider;
		}

		public Actor CreateActor(ICurrencyProvider currencyProvider, IRuleLibrary ruleLibrary, ActorType actorType, string currencyName, string[] rulesForBuy, string[] rulesForSell)
		{
			var actor = new Actor(currencyProvider, ruleLibrary);

			actor.Data.CurrentTime = Const.StartActorTime;

			actor.Data.Account.BtcCount = 0.001m;
			actor.Data.Account.CurrencyName = currencyName;
			actor.Data.Account.CurrencyCount = 0m;

			actor.Data.ActorType = actorType;
			actor.Data.ChangeCoefficient = 0.01;
			actor.Data.HesitationToSell = 0.5;
			actor.Data.HesitationToBuy = 0.5;
			actor.Data.IsAlive = true;
			actor.Data.LastActionTime = new DateTime(2000, 1, 1);
			actor.Data.Generation = -1;

			// TODO: доп  проверки
			if (rulesForBuy == null && rulesForSell == null || rulesForBuy.Length == 0 && rulesForSell.Length == 0)
			{
				rulesForBuy = ruleLibrary.RulesBuyDictionary.Select(x => x.Key).ToArray();
				rulesForSell = ruleLibrary.RulesSellDictionary.Select(x => x.Key).ToArray();
			}
			 
			foreach (var rule in rulesForBuy) 
			{
				actor.Data.Rules.Add(new BalancedRule(rule, OperationType.Buy));
			}

			foreach (var rule in rulesForSell)
			{
				actor.Data.Rules.Add(new BalancedRule(rule, OperationType.Sell) { Guid = Guid.NewGuid(), Coefficient = 0.4 });
			}

			return actor;
		}

		public async Task<List<Actor>> LoadAliveActors(ICurrencyProvider currencyProvider, IRuleLibrary ruleLibrary)
		{
			var aliveActors = new List<Actor>();

			var dtoActorData = await actorProvider.LoadAliveActors();

			foreach(var dtoData in dtoActorData)
			{
				var actor = new Actor(currencyProvider, ruleLibrary) { Data = DbActorToModel(dtoData) };
				aliveActors.Add(actor);
			}

			return aliveActors;
		}

		public async Task SaveActor(Actor actor)
		{
			var dtoData = ActorToDbModel(actor.Data);

			await actorProvider.SaveOrUpdateActor(dtoData);
		}

		private ActorData DbActorToModel(BittrexData.Models.ActorDataDto actorData)
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AllowNullDestinationValues = true;
				cfg.AllowNullCollections = true;

				cfg.CreateMap<BittrexData.Models.ActorDataDto, ActorData>()
				.ForMember(x => x.Account, x => x.MapFrom(m => m.Account))
				.ForMember(x => x.Transactions, x => x.MapFrom(m => m.Transactions))
				.ForMember(x => x.Predictions, x => x.MapFrom(m => m.Predictions))
				.ForMember(x => x.Rules, x => x.MapFrom(m => m.Rules));

				cfg.CreateMap<BittrexData.Models.AccountDto, Account>();

				cfg.CreateMap<BittrexData.Models.TransactionDto, Transaction>();

				cfg.CreateMap<BittrexData.Models.BalancedRuleDto, BalancedRule>()
					.ConstructUsing(x => new BalancedRule(x.RuleName, x.Type));

				cfg.CreateMap<BittrexData.Models.PredictionDto, Prediction>()
					.ForMember(x => x.RulePredictions, x => x.MapFrom(m => m.RulePredictions));

				cfg.CreateMap<BittrexData.Models.PredictionUnitDto, KeyValuePair<string, double>>()
					.ConstructUsing(x => new KeyValuePair<string, double>(x.RuleName, x.Coefficient));

			});

			var mapper = new Mapper(config);

			var data = mapper.Map<ActorData>(actorData);

			return data;
		}

		private BittrexData.Models.ActorDataDto ActorToDbModel(ActorData actorData)
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ActorData, BittrexData.Models.ActorDataDto>()
				.ForMember(x => x.Account, x => x.MapFrom(m => m.Account))
				.ForMember(x => x.Transactions, x => x.MapFrom(m => m.Transactions))
				.ForMember(x => x.Predictions, x => x.MapFrom(m => m.Predictions))
				.ForMember(x => x.Rules, x => x.MapFrom(m => m.Rules))
									.AfterMap((src, dest) =>
									{
										foreach (var t in dest.Transactions) { t.ActorData = dest; t.ActorDataGuid = dest.Guid; }
										foreach (var t in dest.Rules) { t.ActorData = dest; t.ActorDataGuid = dest.Guid; }
										foreach (var t in dest.Predictions) { t.ActorData = dest; t.ActorDataGuid = dest.Guid; }
										dest.Account.ActorData = dest;
										dest.Account.ActorDataGuid = dest.Guid;

									});

				cfg.CreateMap<Account, BittrexData.Models.AccountDto>();

				cfg.CreateMap<Transaction, BittrexData.Models.TransactionDto>();

				cfg.CreateMap<BalancedRule, BittrexData.Models.BalancedRuleDto>();

				cfg.CreateMap<Prediction, BittrexData.Models.PredictionDto>()
					.ForMember(x => x.RulePredictions, x => x.MapFrom(m => m.RulePredictions))
					.AfterMap((src, dest) =>
						{
							foreach (var s in dest.RulePredictions)
							{
								s.PredictionGuid = src.Guid;
								s.Prediction = dest;
							}
						});

				cfg.CreateMap<KeyValuePair<string, double>, BittrexData.Models.PredictionUnitDto>()
					.ForMember(x => x.RuleName, x => x.MapFrom(m => m.Key))
					.ForMember(x => x.Coefficient, x => x.MapFrom(m => m.Value))
					.ForMember(x => x.Guid, x => x.MapFrom(m => Guid.NewGuid()));

			});

			var mapper = new Mapper(config);

			var dataDto = mapper.Map<BittrexData.Models.ActorDataDto>(actorData);
			return dataDto;
		}
	}
}
