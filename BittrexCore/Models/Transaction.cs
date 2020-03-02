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
			// TODO: вынести в конструктор
			Time = transactionTime;
			Type = operationType;
			BtcCount = sumBtc;
			CurrencyPrice = provider.FindPriceByTime(transactionTime, account.CurrencyName);
			if (CurrencyPrice <= 0)
			{
				this.TransactionResult = TransactionResult.Failed; // TODO: Логгер
				return;
			} 
			if (operationType == OperationType.Buy)
			{
				if (account.BtcCount - sumBtc + Const.TransactionSumBtcCommision < 0)
				{
					TransactionResult = TransactionResult.Failed;
					return;
				}
				account.BtcCount -= sumBtc + Const.TransactionSumBtcCommision;
				account.CurrencyCount += sumBtc / CurrencyPrice;
			} else if (operationType == OperationType.Sell)
			{
				if (account.CurrencyCount - (sumBtc + Const.TransactionSumBtcCommision) / CurrencyPrice < 0)
				{
					TransactionResult = TransactionResult.Failed;
					return;
				}
				account.CurrencyCount -= sumBtc / CurrencyPrice;
				account.BtcCount += sumBtc - Const.TransactionSumBtcCommision;
			} else
			{
				this.TransactionResult = TransactionResult.Error;
			}
			TransactionResult = TransactionResult.Success;

        }
    }
}
