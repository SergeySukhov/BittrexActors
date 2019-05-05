using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;

namespace BittrexModels.ActorModels
{
    public enum OperationType
    {
        Buy, // покупка целовой валюты - продажа базовой (btc)
        Sell // продажа целевой валюты - покупка базовой (btc)
    }
    public enum TransactionResult
    {
        Success,
        WithWarnings,
        Awaiting,
        Error,
        Canceled
    }
    public class Transaction
    {
        public static Dictionary<string, Transaction> AllTransactions { get; }
        
        private static BittrexClient BittrexClient = new BittrexClient("6c58ca3f387b4581ab2ba324b7a78dd5", "");
        private static Queue<Transaction> AwaitingTransactions = new Queue<Transaction>();

        /// <summary>
        /// Обработка очередной транзакции
        /// todo: добавить параллельность для разных актеров и проверку на 60 транзакций в минуту
        /// </summary>
        private async static void ProcessTransactions()
        {
            if (AwaitingTransactions.Count == 0) return;

            var currentTransaction = AwaitingTransactions.Peek();
            currentTransaction.TransactionResult = await currentTransaction.CommitTransaction();
            // при успешном выполнении транзакции зачисляем валюту
            if (currentTransaction.TransactionResult == TransactionResult.Success)
            {
                if (currentTransaction.Type == OperationType.Buy)
                    currentTransaction.Actor.CountVolume.TargetCurrencyCount += currentTransaction.CurrencySum;
                if (currentTransaction.Type == OperationType.Sell)
                    currentTransaction.Actor.CountVolume.BtcCount += currentTransaction.BtcSum;
            }
            else // иначе возвращаем
            {
                if (currentTransaction.Type == OperationType.Sell)
                    currentTransaction.Actor.CountVolume.TargetCurrencyCount += currentTransaction.CurrencySum;
                if (currentTransaction.Type == OperationType.Buy)
                    currentTransaction.Actor.CountVolume.BtcCount += currentTransaction.BtcSum;
            }           
        }
        /// <summary>
        /// Создание новой транзакции с замораживанием счета
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="actor"></param>
        /// <param name="sum"></param>
        public static void CreateTransaction(OperationType operationType, Actor actor, decimal sum)
        {
            var newTransaction = new Transaction(operationType, actor, sum);
            if (!newTransaction.PrecheckTransaction())
            {
                newTransaction.TransactionResult = TransactionResult.Canceled;
                return;
            }
            // замораживаем счет
            newTransaction.Actor.CountVolume.TargetCurrencyCount -= newTransaction.CurrencySum;
            newTransaction.Actor.CountVolume.BtcCount -= newTransaction.BtcSum;
            // заносим в очередь на обработку
            AwaitingTransactions.Enqueue(newTransaction);
        }

        /// <summary>
        /// Тип операции продажа/покупка
        /// </summary>
        public OperationType Type { get; }
        /// <summary>
        /// Сумма базовой валюты (btc)
        /// </summary>
        public decimal BtcSum { get; private set; } = 0;
        /// <summary>
        /// Сумма целевой валюты
        /// </summary>
        public decimal CurrencySum { get; private set; } = 0;
        /// <summary>
        /// Наименование магазина для Bittrex.Api
        /// </summary>
        public string MarketName { get; }
        /// <summary>
        /// Состояние транзакции
        /// </summary>
        public TransactionResult TransactionResult { get; private set; }
        /// <summary>
        /// Актор, запросивший транзакцию
        /// </summary>
        private Actor Actor { get; }

        private Transaction(OperationType operationType, Actor actor, decimal sum)
        {
            AllTransactions.Add(actor.Guid.ToString(), this);
            this.Type = operationType;
            if (Type == OperationType.Buy) this.BtcSum = sum;
            if (Type == OperationType.Sell) this.CurrencySum = sum;
            this.MarketName = actor.TargetMarket;
            this.Actor = actor;
            TransactionResult = TransactionResult.Awaiting;            
        }

        /// <summary>
        /// Выполнение транзакции (запрос последней цены для покупаемой валюты для иммитации покупки/продажи)
        /// </summary>
        /// <returns></returns>
        private async Task<TransactionResult> CommitTransaction()
        {
            var startTransact = DateTime.Now;
            ApiResult<Ticker> apiResult = null;
            try
            {
                apiResult = await BittrexClient.GetTicker(MarketName);
                if (!apiResult.Success) throw new Exception(apiResult.Message); // todo: добавить проверку связанную с api и интернетом
                if (this.Type == OperationType.Buy) this.CurrencySum = apiResult.Result.Ask * this.BtcSum; // сколько куплено на указанное кол-во btc
                if (this.Type == OperationType.Sell) this.BtcSum = apiResult.Result.Bid * this.CurrencySum; // сколько btc куплено

                if (DateTime.Now - startTransact > new TimeSpan(0, 1, 0) && apiResult.Success) return TransactionResult.WithWarnings;
                return TransactionResult.Success;
            } catch (Exception e)
            {
                // todo: logger
                return TransactionResult.Error;
            }
        }

        /// <summary>
        /// Препроверка транзакции
        /// </summary>
        /// <returns></returns>
        private bool PrecheckTransaction()
        {
            return (this.Type == OperationType.Buy && Actor.CountVolume.BtcCount >= BtcSum) 
                || (this.Type == OperationType.Sell && Actor.CountVolume.TargetCurrencyCount >= CurrencySum);
        }

    }
}
