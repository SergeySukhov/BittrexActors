using BittrexModels.Interfaces;
using BittrexModels.Models;
using DataManager.Models;
using DataManager.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Models
{
    public class TransactionManager : ITransactionManager
    {
        private IBittrexApi BittrexApi;

        private BittrexDbProvider BittrexDbProvider;

        public List<Transaction> AllTransactions { get; }

        public Queue<Transaction> AwaitingTransactions { get; }

        public TransactionManager(IBittrexApi bittrexApi, BittrexDbProvider bittrexDbProvider)
        {
            this.BittrexDbProvider = bittrexDbProvider;
            this.BittrexApi = bittrexApi;
            this.AwaitingTransactions = new Queue<Transaction>();
            this.AllTransactions = new List<Transaction>();
        }

        // присоединить к главному таймеру
        public void ProcessTransaction(object sender, EventArgs e)
        {
            if (AwaitingTransactions.Count == 0) return;

            var currentTransaction = AwaitingTransactions.Dequeue();
            if (currentTransaction == null)
            {
                Console.WriteLine("!! error null TRANSACTION");
                return;
            }
            Task.Factory.StartNew(async () => {
                if (!PrecheckTransaction(currentTransaction))
                {
                    currentTransaction.TransactionResult = TransactionResult.Canceled;
                }
                else
                {
                    currentTransaction.TransactionResult = TransactionResult.Awaiting;
                    currentTransaction.TransactionResult = await CommitTransaction(currentTransaction);
                }

                currentTransaction.ReleaseTime = DateTime.Now;
                BittrexDbProvider.SaveTransaction(currentTransaction);
            });
            
        }

        public async Task<TransactionResult> CommitTransaction(Transaction transaction)
        {
            decimal price = 0;
            try
            {
                // узнаем цену целевой валюты в btc
                price = await BittrexApi.GetPrice(transaction);
                if (price == 0) return TransactionResult.Error;

            }
            catch (Exception ex)
            {
                return TransactionResult.Error;
            }

            transaction.ReleasePrice = price;

            if (transaction.Type == OperationType.Sell)
                if (transaction.Account.CurrencyCount >= transaction.CurrencySum
                && transaction.CurrencySum * price > 0.0005m)
                {
                    transaction.Account.CurrencyCount -= transaction.CurrencySum;
                    transaction.Account.BtcCount += transaction.CurrencySum * price;
                }
                else
                {
                    return TransactionResult.Canceled;
                }

            if (transaction.Type == OperationType.Buy)
                if (transaction.Account.BtcCount >= transaction.CurrencySum * price
                && transaction.CurrencySum * price > 0.0005m)
                {
                    transaction.Account.BtcCount -= transaction.CurrencySum * price;
                    transaction.Account.CurrencyCount += transaction.CurrencySum;
                }
                else
                {
                    return TransactionResult.Canceled;
                }



            return TransactionResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="currencySum">Сколько валюты покупать/продавать</param>
        /// <param name="marketName"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public Transaction CreateTransaction(OperationType operationType, decimal currencySum, string marketName, Account account)
        {
            var newTransaction = new Transaction
            {
                Guid = Guid.NewGuid(),
                Type = operationType,
                CurrencySum = currencySum,
                ReleasePrice = 0m,
                MarketName = marketName,
                Account = account,
                CreationTime = DateTime.Now,
                TransactionResult = TransactionResult.Created,
                ReleaseTime = DateTime.Now,
                AccountGuid = account.Guid
            };
            BittrexDbProvider.SaveTransaction(newTransaction);

            // заносим в очередь на обработку
            AwaitingTransactions.Enqueue(newTransaction);

            return newTransaction;
        }

        public void InitiateLoadTransactions(Transaction[] transactions)
        {
            foreach (var transaction in transactions)
            {
                if (transaction.Account != null
                    && (transaction.TransactionResult == TransactionResult.Created || transaction.TransactionResult == TransactionResult.Awaiting)
                    && AwaitingTransactions.Select(x => x.Guid).Contains(transaction.Guid)
                    )
                    AwaitingTransactions.Enqueue(transaction);
            }
        }

        public string ValidateTransactions(Transaction[] transactions)
        {

            return "ok";
        }

        private bool PrecheckTransaction(Transaction transaction)
        {
            return transaction.Type == OperationType.Sell && transaction.CurrencySum < transaction.Account.CurrencyCount || transaction.Type == OperationType.Buy && transaction.Account.BtcCount > 0.0005m;
        }


    }
}
