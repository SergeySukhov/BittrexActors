using BittrexData;
using BittrexData.Interfaces;
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

        public void CommitTransaction(Account account, decimal sumBtc, OperationType operationType, DateTime transactionTime, ICurrencyProvider provider)
        {
			var curPrice = provider.FindPriceByTime(transactionTime, account.CurrencyName);
			
			if (operationType == OperationType.Buy)
			{
				if (account.BtcCount - sumBtc + Const.TransactionSumBtcCommision < 0)
				{
					TransactionResult = TransactionResult.Failed;
					return;
				}
				account.BtcCount -= sumBtc + Const.TransactionSumBtcCommision;
				account.CurrencyCount += sumBtc / curPrice;
			} else if (operationType == OperationType.Sell)
			{
				if (account.CurrencyCount - (sumBtc + Const.TransactionSumBtcCommision) / curPrice < 0)
				{
					TransactionResult = TransactionResult.Failed;
					return;
				}
				account.CurrencyCount -= sumBtc / curPrice;
				account.BtcCount += sumBtc - Const.TransactionSumBtcCommision;
			} else
			{
				throw new Exception("Непонятная операция!");
			}
			

        }
    }
}
