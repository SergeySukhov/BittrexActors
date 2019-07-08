using DataManager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Interfaces
{
    public interface ITransactionManager
    {
        List<Transaction> AllTransactions { get; }

        Queue<Transaction> AwaitingTransactions { get; }

        /// <summary>
        /// Создание новой транзакции с замораживанием счета
        /// </summary>
        Transaction CreateTransaction(OperationType operationType, decimal sum, string marketName, Account account);

        void ProcessTransaction(object sender, EventArgs e);
        /// <summary>
        /// Выполнение транзакции (запрос последней цены для покупаемой валюты для иммитации покупки/продажи)
        /// </summary>
        /// <returns></returns>
        Task<TransactionResult> CommitTransaction(Transaction transaction);

        /// <summary>
        /// Перезапускает загруженные невыполненные транзакции
        /// </summary>
        /// <param name="transactions"></param>
        void InitiateLoadTransactions(Transaction[] transactions);

        /// <summary>
        /// Проверка выполненых транзакций </summary>
        /// <param name="transactions"></param>
        /// <returns>"ок" or "Erorr in AccountGuid, error message|..."</returns>
        string ValidateTransactions(Transaction[] transactions);

    }
}