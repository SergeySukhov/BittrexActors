﻿using Bittrex.Api.Client;
using BittrexModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.Interfaces
{
    public interface IBittrexApi
    {
        BittrexClient BittrexClient { get; }
        
        List<DateTime> OperationJournal { get; }

        //Task<TransactionResult> CommitTransaction(ITransaction transaction);
        Task<IObservation> GetObservation(string TargetMarket);
        Task<bool> CheckConnection();
        Task<decimal> GetPrice(ITransaction transaction);
    }
}