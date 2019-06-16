using System;
using DataManager.Models;

namespace BittrexModels.Models
{
    public class Transaction : DataManager.Models.Transaction
    {        
        public Transaction(OperationType type, decimal sum, string marketName, Account account)
        {
            this.Guid = Guid.NewGuid();
            this.CreationTime = DateTime.Now;
            this.Type = type;
            this.CurrencySum = sum;
            this.MarketName = marketName;
            this.Account = account;
            TransactionResult = TransactionResult.Awaiting;
        }        
       
    }
}
