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
        List<ITransaction> AllTransactions { get; }

        Queue<ITransaction> AwaitingTransactions { get; }

        /// <summary>
        /// Создание новой транзакции с замораживанием счета
        /// </summary>
        ITransaction CreateTransaction(OperationType operationType, decimal sum, string marketName, IAccount account);

        void ProcessTransaction(object sender, EventArgs e);
        /// <summary>
        /// Выполнение транзакции (запрос последней цены для покупаемой валюты для иммитации покупки/продажи)
        /// </summary>
        /// <returns></returns>
        Task<TransactionResult> CommitTransaction(ITransaction transaction);

        /// <summary>
        /// Препроверка транзакции
        /// </summary>
        /// <returns></returns>
        bool PrecheckTransaction(ITransaction transaction);

    }
}
