using BittrexData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class Transaction
    {
        public Guid Guid;
    
        public OperationType Type;

        public DateTime Time;

        public decimal CurrencyPrice;

        public decimal BtcCount;

        public TransactionResult TransactionResult;

        public Transaction()
        {
            Guid = Guid.NewGuid();
        }

        public void CommitTransaction(Account account, decimal sum, OperationType operationType, DateTime transactionTime)
        {

        }
    }
}
