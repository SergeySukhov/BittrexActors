using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bittrex.Api.Client.Models;
using Bittrex.Api.Client;

namespace BittrexModels.ActorModels
{
    public enum OperationType
    {
        Buy,
        Sell
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

        public async static void ProcessTransactions()
        {
            if (AwaitingTransactions.Count == 0) return;

            var currentTransaction = AwaitingTransactions.Peek();
            currentTransaction.TransactionResult = await currentTransaction.CommitTransaction();
            if (currentTransaction.TransactionResult == TransactionResult.Success)
            {
                if (currentTransaction.Type == OperationType.Buy)
                    currentTransaction.Actor.CountVolume.TargetCurrencyCount += currentTransaction.CurrencySum;
                if (currentTransaction.Type == OperationType.Sell)
                    currentTransaction.Actor.CountVolume.BtcCount += currentTransaction.BtcSum;
            }
            else
            {
                if (currentTransaction.Type == OperationType.Buy)
                    currentTransaction.Actor.CountVolume.TargetCurrencyCount += currentTransaction.CurrencySum;
                if (currentTransaction.Type == OperationType.Sell)
                    currentTransaction.Actor.CountVolume.BtcCount += currentTransaction.BtcSum;
            }           
        }

        public static void CreateTransaction(OperationType operationType, Actor actor, decimal sum)
        {
            var newTransaction = new Transaction(operationType, actor, sum);
            if (newTransaction.TransactionResult != TransactionResult.Awaiting) return; // todo: add logger
            actor.ChangeCount(newTransaction, sum);
            AwaitingTransactions.Enqueue(newTransaction);            
        }

        public OperationType Type { get; }
        public decimal BtcSum { get; private set; } = 0;
        public decimal CurrencySum { get; private set; } = 0;
        public string MarketName { get; }
        public TransactionResult TransactionResult { get; private set; }
        private Actor Actor { get; }
        private Transaction(OperationType operationType, Actor actor, decimal sum)
        {
            AllTransactions.Add(actor.Id.ToString(), this);
            this.Type = operationType;
            if (Type == OperationType.Buy) this.BtcSum = sum;
            if (Type == OperationType.Sell) this.CurrencySum = sum;
            this.MarketName = actor.TargetMarket;
            if (operationType == OperationType.Buy && actor.CountVolume.BtcCount < sum) this.TransactionResult = TransactionResult.Canceled;
            TransactionResult = TransactionResult.Awaiting;            
        }

        private async Task<TransactionResult> CommitTransaction()
        {
            var startTransact = DateTime.Now;
            ApiResult<Ticker> apiResult = null;
            try
            {
                apiResult = await BittrexClient.GetTicker(MarketName);
                if (!apiResult.Success) throw new Exception(apiResult.Message);
                if (this.Type == OperationType.Buy) this.CurrencySum = apiResult.Result.Ask * this.BtcSum; // сколько куплено на указанное кол-во btc
                if (this.Type == OperationType.Buy) this.BtcSum = apiResult.Result.Ask * this.CurrencySum; // сколько btc куплено

                    if (DateTime.Now - startTransact > new TimeSpan(0, 1, 0) && apiResult.Success) return TransactionResult.WithWarnings;
                return TransactionResult.Success;
            } catch (Exception e)
            {
                // todo: logger
                return TransactionResult.Error;
            }
        }

    }
}
