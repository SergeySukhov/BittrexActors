using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BittrexCore.Models;
using BittrexData.Interfaces;

namespace BittrexCore
{
	public class ActorFactory
	{
		private IActorProvider actorProvider;


		public ActorFactory(IActorProvider actorProvider)
		{
			this.actorProvider = actorProvider;
		}

		public Actor CreateActor(ICurrencyProvider currencyProvider, RulesLibrary.RuleLibrary ruleLibrary)
		{
			var actor = new Actor(currencyProvider, ruleLibrary);

			// ...

			return actor;
		}

		public async Task<List<Actor>> LoadAliveActors(ICurrencyProvider currencyProvider, RulesLibrary.RuleLibrary ruleLibrary)
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

		private ActorData DbActorToModel(BittrexData.Models.ActorData actorData)
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AllowNullDestinationValues = true;
				cfg.AllowNullCollections = true;

				cfg.CreateMap<BittrexData.Models.ActorData, ActorData>()
				.ForMember(x => x.Account, x => x.MapFrom(m => m.Account))
				.ForMember(x => x.Transactions, x => x.MapFrom(m => m.Transactions))
				.ForMember(x => x.Predictions, x => x.MapFrom(m => m.Predictions))
				.ForMember(x => x.Rules, x => x.MapFrom(m => m.Rules));

				cfg.CreateMap<BittrexData.Models.Account, Account>();

				cfg.CreateMap<BittrexData.Models.Transaction, Transaction>();

				cfg.CreateMap<BittrexData.Models.BalancedRule, BalancedRule>()
					.ConstructUsing(x => new BalancedRule(x.RuleName, x.Type));

				cfg.CreateMap<BittrexData.Models.Prediction, Prediction>()
					.ForMember(x => x.RulePredictions, x => x.MapFrom(m => m.RulePredictions));

				cfg.CreateMap<BittrexData.Models.PredictionUnit, KeyValuePair<string, double>>()
					.ConstructUsing(x => new KeyValuePair<string, double>(x.RuleName, x.Coefficient));

			});

			var mapper = new Mapper(config);

			var data = mapper.Map<ActorData>(actorData);

			return data;
		}

		private BittrexData.Models.ActorData ActorToDbModel(ActorData actorData)
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ActorData, BittrexData.Models.ActorData>()
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

				cfg.CreateMap<Account, BittrexData.Models.Account>();

				cfg.CreateMap<Transaction, BittrexData.Models.Transaction>();

				cfg.CreateMap<BalancedRule, BittrexData.Models.BalancedRule>();

				cfg.CreateMap<Prediction, BittrexData.Models.Prediction>()
					.ForMember(x => x.RulePredictions, x => x.MapFrom(m => m.RulePredictions))
					.AfterMap((src, dest) =>
						{
							foreach (var s in dest.RulePredictions)
							{
								s.PredictionGuid = src.Guid;
								s.Prediction = dest;
							}
						});

				cfg.CreateMap<KeyValuePair<string, double>, BittrexData.Models.PredictionUnit>()
					.ForMember(x => x.RuleName, x => x.MapFrom(m => m.Key))
					.ForMember(x => x.Coefficient, x => x.MapFrom(m => m.Value))
					.ForMember(x => x.Guid, x => x.MapFrom(m => Guid.NewGuid()));

			});

			var mapper = new Mapper(config);

			var dataDto = mapper.Map<BittrexData.Models.ActorData>(actorData);
			return dataDto;
		}
	}
}
