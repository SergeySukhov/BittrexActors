using System;
using System.Collections.Generic;
using System.Linq;

using BittrexCore.Models;
using BittrexData;
using BittrexData.Interfaces;
using RulesLibrary;

namespace BittrexCore
{
    public class Actor
    {
        public readonly Guid Guid;
        public ActorData Data;
        public readonly ICurrencyProvider CurrencyProvider;

        public readonly IRuleLibrary RuleLibrary;

        public Actor(ICurrencyProvider currencyProvider, IRuleLibrary ruleLibrary)
        {
            Guid = Guid.NewGuid();
            Data = new ActorData();
            CurrencyProvider = currencyProvider;
            RuleLibrary = ruleLibrary;
        }

		public void DoWork()
		{
			// TODO: вынести
			if (!Data.IsAlive ||
				Data.CurrentTime - Data.LastActionTime < new TimeSpan(24, 0, 0) && Data.ActorType == ActorType.Daily ||
				Data.CurrentTime - Data.LastActionTime < new TimeSpan(12, 0, 0) && Data.ActorType == ActorType.HalfDaily ||
				Data.CurrentTime - Data.LastActionTime < new TimeSpan(24 * 7, 0, 0) && Data.ActorType == ActorType.Weekly)
				return;

			Data.LastActionTime = Data.CurrentTime;

			if (Data.HesitationToBuy < ShouldBuy())
			{
				CommitOperation(OperationType.Buy);
				Data.HesitationToBuy *= 1.1; // !!
			}
			else
			{
				Data.HesitationToBuy *= 0.9; // !!
			}

			if (Data.HesitationToBuy < ShouldSell())
			{
				CommitOperation(OperationType.Sell);
				Data.HesitationToSell *= 1.1; // !!
			}
			else
			{
				Data.HesitationToSell *= 0.9; // !!
			}

			Inspect();

			Save();
			

			if (Data.Account.CurrencyCount < 0.00005m && Data.Account.BtcCount < 0.00005m) Data.IsAlive = false;
		}

		public string GetInfo()
		{
			var price = CurrencyProvider.FindPriceByTime(Data.CurrentTime, Data.Account.CurrencyName);
			var estimatePrice = Data.Account.BtcCount + Data.Account.CurrencyCount * price;

			return $"Актер: {Data.Guid}" + Environment.NewLine +
					$"Счет: {Data.Account.BtcCount}BTC" + Environment.NewLine +
					$"Счет валюты: {Data.Account.CurrencyCount}{Data.Account.CurrencyName}" + Environment.NewLine +
					$"Последнее время действия: {Data.LastActionTime}" + Environment.NewLine +
					$"Текущее время: {Data.CurrentTime}" + Environment.NewLine +
					$"Сомнения покупки: {Data.HesitationToBuy}" + Environment.NewLine +
					$"Сомнения продажи: {Data.HesitationToSell}" + Environment.NewLine +
					$"Тип: {Data.ActorType}" + Environment.NewLine +
					$"Количество предсказаний: {Data.Predictions.Count}" + Environment.NewLine +
					$"Количество транзакций: {Data.Transactions.Count}" + Environment.NewLine +
					$"Оценочное состояние: {estimatePrice}" + Environment.NewLine;
		}

        private double ShouldBuy()
        {
            var result = 0d;

			// Вынести
            var prediction = new Prediction();

            prediction.ForTime = Data.CurrentTime;
            prediction.OldPrice = CurrencyProvider.FindPriceByTime(Data.CurrentTime, Data.Account.CurrencyName) + Const.TransactionSumBtcCommision; // цена + процент за проведение транзакции
			if (prediction.OldPrice <= 0) return 0;

			if (Data.ActorType == ActorType.HalfDaily) prediction.ForTime += new TimeSpan(12, 0, 0);
            else if (Data.ActorType == ActorType.Daily) prediction.ForTime += new TimeSpan(24, 0, 0);
            else if (Data.ActorType == ActorType.Weekly) prediction.ForTime += new TimeSpan(7, 0, 0, 0);


            foreach (var rule in Data.Rules)
            {
                
                if (!RuleLibrary.RulesBuyDictionary.ContainsKey(rule.RuleName)) continue; // TODO: логирование

                var ruleCoef = RuleLibrary.RulesBuyDictionary[rule.RuleName](CurrencyProvider, Data.CurrentTime);

                prediction.RulePredictions.Add(rule.RuleName, ruleCoef);
                result += rule.Coefficient * ruleCoef; // > 0 - рекомендует, < 0 - не рекомендует, = 0 - не смог дать результат
			}

            Data.Predictions.Add(prediction);

            return result;
        }

        // TODO: это можно улучшить
        private double ShouldSell()
        {
            var result = 0d;
            var prediction = new Prediction();

            prediction.ForTime = Data.CurrentTime;
			prediction.OldPrice = CurrencyProvider.FindPriceByTime(Data.CurrentTime, Data.Account.CurrencyName) + Const.TransactionSumBtcCommision; // цена + процент за проведение транзакции

			if (prediction.OldPrice <= 0) return 0;

			if (Data.ActorType == ActorType.HalfDaily) prediction.ForTime += new TimeSpan(6, 0, 0);
            else if (Data.ActorType == ActorType.Daily) prediction.ForTime += new TimeSpan(12, 0, 0);
            else if (Data.ActorType == ActorType.Weekly) prediction.ForTime += new TimeSpan(7, 0, 0, 0);

            foreach (var rule in Data.Rules)
            {
                if (!RuleLibrary.RulesSellDictionary.ContainsKey(rule.RuleName)) continue; // TODO: логирование

                var ruleCoef = RuleLibrary.RulesSellDictionary[rule.RuleName](CurrencyProvider, Data.CurrentTime);

                prediction.RulePredictions.Add(rule.RuleName, ruleCoef);

                result += rule.Coefficient * ruleCoef; // > 0 - рекомендует, < 0 - не рекомендует, = 0 - не смог дать результат
			}

            Data.Predictions.Add(prediction);

            return result;
        }

        private void CommitOperation(OperationType operationType)
        {
            var transaction = new Transaction();
            transaction.CommitTransaction(Data.Account, Const.TransactionSumBtc, operationType, Data.CurrentTime, CurrencyProvider);
            Data.Transactions.Add(transaction);
        }

        private void Inspect()
        {
			var predictionToDelete = new List<Prediction>();
			// проверка предсказаний
			foreach (var predictionsForTime in Data.Predictions) // проходим по всем предсказаниям
			{
				if (Data.CurrentTime - predictionsForTime.ForTime > new TimeSpan(1, 30, 0) || predictionsForTime.ForTime - Data.CurrentTime > new TimeSpan(1, 30, 0)) continue;

				var curPrice = CurrencyProvider.FindPriceByTime(predictionsForTime.ForTime, Data.Account.CurrencyName);
				if (curPrice <= 0) continue;

				foreach (var unitPrediction in predictionsForTime.RulePredictions) // проходим по всем правилам в предсказании
				{
					var rule = Data.Rules.First(r => r.RuleName == unitPrediction.Key);
					if (rule == null || unitPrediction.Value == 0) continue;

					// добавить градации
					if (curPrice >= predictionsForTime.OldPrice && unitPrediction.Value > 0 ||
						curPrice <= predictionsForTime.OldPrice && unitPrediction.Value < 0)
					{
						rule.Coefficient += ChangeRuleCoef(rule.Coefficient, unitPrediction.Value);
					}

					if (curPrice < predictionsForTime.OldPrice && unitPrediction.Value > 0 ||
						curPrice > predictionsForTime.OldPrice && unitPrediction.Value < 0)
					{
						rule.Coefficient -= ChangeRuleCoef(rule.Coefficient, unitPrediction.Value);
					}

					rule.Coefficient = 1d / (1d + Math.Exp(-6d*rule.Coefficient)); // (0,1) // избегаем увеличивающего коэффициента, сохранем линейность
				}

				predictionToDelete.Add(predictionsForTime);
			}

			Data.Predictions.RemoveAll(x => predictionToDelete.Contains(x));
			predictionToDelete.Clear();

			// проверка транзакций
			//...

        }

        private void Save()
        {

        }

        ////////////////////////// вспомогательные функции

        private double Exponenta01(double x)
		{
			return Math.Pow(1d + Math.Exp(-x), -1d);
		}

		private double ExponentaMinus11(double x)
		{
			return (Math.Exp(3d*x) - 1d)/ (Math.Exp(3d*x) + 1d);
		}

		private double ChangeRuleCoef(double currentCoefValue, double predictionValue)
		{
			if (predictionValue == 0) return 0;

			return Const.RuleChangeCoef * predictionValue * ExponentaMinus11(currentCoefValue);
		}
	}
}
