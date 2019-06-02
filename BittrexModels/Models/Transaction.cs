using BittrexModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Models
{
    public class Transaction : ITransaction
    {
        
        public Guid Guid { get; }

        public OperationType Type { get; }

        // public decimal BtcSum { get; set; }
        public decimal CurrencySum { get; set; }

        public string MarketName { get; set; }

        public IAccount Account { get; }

        public TransactionResult TransactionResult { get; set; }

        public Transaction(OperationType type, decimal sum, string marketName, IAccount account)
        {
            this.Guid = Guid.NewGuid();
            this.Type = type;
            //if (Type == OperationType.Buy) this.BtcSum = sum;
           // if (Type == OperationType.Sell)
                this.CurrencySum = sum;
            this.MarketName = marketName;
            this.Account = account;
            TransactionResult = TransactionResult.Awaiting;

        }
               
    }
}
