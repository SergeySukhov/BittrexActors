﻿using System;

namespace BittrexModels.Interfaces
{
    public enum OperationType
    {
        Buy, // покупка целовой валюты - продажа базовой (btc)
        Sell // продажа целевой валюты - покупка базовой (btc)
    }
    public enum TransactionResult
    {
        Success = 0,
        WithWarnings = 1,
        Awaiting = 2,
        Error = 3,
        Canceled = 4,
    }

    public interface ITransaction
    {
        Guid Guid { get; }
        /// <summary>
        /// Тип операции продажа/покупка
        /// </summary>
        OperationType Type { get; }
        /// <summary>
        /// Сумма базовой валюты (btc)
        /// </summary>
        //decimal BtcSum { get; set; }
        /// <summary>
        /// Сумма целевой валюты
        /// </summary>
        decimal CurrencySum { get; }
        /// <summary>
        /// Наименование магазина для Bittrex.Api
        /// </summary>
        string MarketName { get; }
        /// <summary>
        /// Состояние транзакции
        /// </summary>
         
        DateTime CreationTime { get; }

        DateTime ReleaseTime { get; set; }

        TransactionResult TransactionResult { get; set; }

        IAccount Account { get; }        
    }
}
