using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BittrexCore.Models;
using BittrexData;
using BittrexData.Interfaces;
using RulesLibrary;

namespace BittrexCore
{
    public class Actor
    {
        public readonly Guid Guid;
        public DateTime CurrentTime;
        public readonly ActorData Data;
        public readonly ICurrencyProvider CurrencyProvider;

        public RuleLibrary RuleLibrary;

        public Actor(ICurrencyProvider currencyProvider, RuleLibrary ruleLibrary)
        {
            Guid = Guid.NewGuid();
            Data = new ActorData();
            CurrencyProvider = currencyProvider;
            RuleLibrary = ruleLibrary;
        }

        public void DoWork()
        {
            if (Data.HesitationToBuy < ShouldBuy())
            {
                CommitOperation(OperationType.Buy);
            }

            if (Data.HesitationToBuy < ShouldSell())
            {
                CommitOperation(OperationType.Sell);
            }

            Inspect();

            Save();

            CurrentTime += new TimeSpan(1, 0, 0); // в обучающей модели
        }

        private double ShouldBuy()
        {
            var result = 1d;
            var prediction = new Prediction();

            prediction.ForTime = CurrentTime;
            // !! prediction.OldPrice = CurrencyProvider. + 0.00005m;
            if (Data.ActorType == ActorType.HalfDaily) prediction.ForTime += new TimeSpan(6, 0, 0);
            else if (Data.ActorType == ActorType.Daily) prediction.ForTime += new TimeSpan(12, 0, 0);
            else if (Data.ActorType == ActorType.Weekly) prediction.ForTime += new TimeSpan(7, 0, 0, 0);


            foreach (var rule in Data.Rules)
            {
                
                if (!RuleLibrary.RulesBuyDictionary.ContainsKey(rule.RuleName)) continue; // TODO: логирование

                var ruleCoef = RuleLibrary.RulesBuyDictionary[rule.RuleName](CurrencyProvider, CurrentTime);

                prediction.RulePredictions.Add(rule.RuleName, ruleCoef);
                result += rule.Coefficient * ruleCoef; //  > 0.5 - рекомендует, < 0.5 - не рекомендует, = 0 - не смог дать результат
			}

            Data.Predictions.Add(prediction);

            return result;
        }

        // TODO: это можно улучшить
        private double ShouldSell()
        {
            var result = 1d;
            var prediction = new Prediction();

            prediction.ForTime = CurrentTime;
            if (Data.ActorType == ActorType.HalfDaily) prediction.ForTime += new TimeSpan(6, 0, 0);
            else if (Data.ActorType == ActorType.Daily) prediction.ForTime += new TimeSpan(12, 0, 0);
            else if (Data.ActorType == ActorType.Weekly) prediction.ForTime += new TimeSpan(7, 0, 0, 0);

            foreach (var rule in Data.Rules)
            {

                if (!RuleLibrary.RulesSellDictionary.ContainsKey(rule.RuleName)) continue; // TODO: логирование

                var ruleCoef = RuleLibrary.RulesSellDictionary[rule.RuleName](CurrencyProvider, CurrentTime);
                prediction.RulePredictions.Add(rule.RuleName, ruleCoef);
                result += rule.Coefficient * ruleCoef; //  > 1 - рекомендует, < 1 - не рекомендует, = 0 - не смог дать результат
            }

            Data.Predictions.Add(prediction);

            return result;
        }

        private void CommitOperation(OperationType operationType)
        {
            var transaction = new Transaction();
            transaction.CommitTransaction(Data.Account, Const.TransactionSumBtc, operationType, CurrentTime);
            Data.Transactions.Add(transaction);
        }

        private void Inspect()
        {
			// проверка предсказаний
			foreach (var predictionsForTime in Data.Predictions) // проходим по всем предсказаниям
			{
				if (CurrentTime - predictionsForTime.ForTime > new TimeSpan(1, 30, 0) || predictionsForTime.ForTime - CurrentTime > new TimeSpan(1, 30, 0)) continue;

				var curPrice = CurrencyProvider.FindClosetPrice(predictionsForTime.ForTime, Data.Account.CurrencyName);
				if (curPrice <= 0) continue;

				foreach (var unitPrediction in predictionsForTime.RulePredictions) // проходим по всем правилам в предсказании
				{
					var rule = Data.Rules.First(r => r.RuleName == unitPrediction.Key);
					if (rule == null || unitPrediction.Value == 0) continue;

					// добавить градации
					if (curPrice >= predictionsForTime.OldPrice && unitPrediction.Value >= 1 ||
						curPrice <= predictionsForTime.OldPrice && unitPrediction.Value <= 1)
					{
						rule.Coefficient += rule.Coefficient * Const.RuleChangeCoef + Const.RuleChangeCoef;
					}

					if (curPrice < predictionsForTime.OldPrice && unitPrediction.Value > 1 ||
						curPrice < predictionsForTime.OldPrice && unitPrediction.Value > 1)
					{
						rule.Coefficient -= rule.Coefficient * Const.RuleChangeCoef + Const.RuleChangeCoef;
					}

					rule.Coefficient = 1d / (1d + Math.Exp(-rule.Coefficient)); // есть функции получше
				}
			}

			// проверка транзакций
			//...

        }

        private void Save()
        {

        }

        ////////////////////////// вспомогательные функции

        
    }
}
