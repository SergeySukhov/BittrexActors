using BittrexModels.Interfaces;
using BittrexModels.Models;
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

        public List<ITransaction> AllTransactions { get; }

        public Queue<ITransaction> AwaitingTransactions { get; }

        public TransactionManager(IBittrexApi bittrexApi)
        {
            this.BittrexApi = bittrexApi;
            this.AwaitingTransactions = new Queue<ITransaction>();
            this.AllTransactions = new List<ITransaction>();
        }

        // присоединить к главному таймеру
        public async void ProcessTransaction(object sender, EventArgs e)
        {
            if (AwaitingTransactions.Count == 0) return;

            var currentTransaction = AwaitingTransactions.Dequeue();

            currentTransaction.TransactionResult = await CommitTransaction(currentTransaction);
                        
        }

        public async Task<TransactionResult> CommitTransaction(ITransaction transaction)
        {
            decimal price = 0;
            try
            {
                // узнаем цену целевой валюты в btc
                price = await BittrexApi.GetPrice(transaction);
                if (price == 0) return TransactionResult.Error;

            } catch (Exception ex)
            {
                return TransactionResult.Error;
            }

            if (transaction.Type == OperationType.Sell)
                if (transaction.Account.CurrencyCount >= transaction.CurrencySum
                && transaction.CurrencySum*price > 0.0005m)
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
        /// <param name="CurrencySum">Сколько валюты покупать/продавать</param>
        /// <param name="marketName"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public ITransaction CreateTransaction(OperationType operationType, decimal CurrencySum, string marketName, IAccount account)
        {
            var newTransaction = new Transaction(operationType, CurrencySum, marketName, account);
            AllTransactions.Add(newTransaction);
                       
            // заносим в очередь на обработку
            AwaitingTransactions.Enqueue(newTransaction);
            return newTransaction;
        }

        public bool PrecheckTransaction(ITransaction transaction)
        {
            return true;
            //return (transaction.Type == OperationType.Buy && transaction.Account.BtcCount >= transaction.BtcSum && transaction.BtcSum > 0.0005m)
            //    || (transaction.Type == OperationType.Sell && transaction.Account.CurrencyCount >= transaction.CurrencySum && transaction.CurrencySum > 0.0005m);
        }


    }
}
