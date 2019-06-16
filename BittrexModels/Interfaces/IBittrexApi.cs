using Bittrex.Api.Client;
using BittrexModels.Interfaces;
using DataManager.Models;
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

        Task<Observation> GetObservation(string TargetMarket);
        // Task<bool> CheckConnection();
        Task<decimal> GetPrice(Transaction transaction);
    }
}
