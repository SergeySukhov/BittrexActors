using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex.Api.Client;
using Bittrex.Api.Client.Models;
using BittrexModels.Interfaces;

namespace BittrexModels.ActorModels
{
    public class BittrexApiManager : IBittrexApi
    {
        //private string ApiKey;

        public bool ApiReady;

        public BittrexClient BittrexClient { get; }

        public List<DateTime> OperationJournal { get; }

        public BittrexApiManager(BittrexClient bittrexClient)
        {
            this.BittrexClient = bittrexClient;
            // this.ApiKey = apiKey;
        }

        /// <summary>
        /// Валидация клиента
        /// </summary>
        /// <returns></returns>
        public bool CheckClient()
        {
            this.ApiReady = true;
            return true;
        }

        public async Task<decimal> GetPrice(ITransaction transaction)
        {
            ApiResult<Ticker> apiResult = null;

            try
            {
                apiResult = await this.BittrexClient.GetTicker(transaction.MarketName);

                if (transaction.Type == OperationType.Buy)
                    return apiResult.Result.Ask;
                else
                    return apiResult.Result.Bid;
            }
            catch (Exception ex)
            {
                // todo: logger
                return 0;
            }
        }

        public async Task<TransactionResult> CommitTransaction(ITransaction transaction)
        {
            var startTransact = DateTime.Now;
            ApiResult<Ticker> apiResult = null;
            try
            {
                apiResult = await this.BittrexClient.GetTicker(transaction.MarketName);
                if (!apiResult.Success) throw new Exception(apiResult.Message); // todo: добавить проверку связанную с api и интернетом
                //if (transaction.Type == OperationType.Buy)
                //    transaction.CurrencySum = transaction.BtcSum / apiResult.Result.Ask; // сколько куплено на указанное кол-во btc
                ////if (transaction.Type == OperationType.Sell)
                //    transaction.BtcSum = apiResult.Result.Bid * transaction.CurrencySum; // сколько btc куплено

                if (DateTime.Now - startTransact > new TimeSpan(0, 1, 0) && apiResult.Success) return TransactionResult.WithWarnings;
                return TransactionResult.Success;
            }
            catch (Exception e)
            {
                return TransactionResult.Error;
            }
        }

        public async Task<IObservation> GetObservation(string TargetMarket)
        {
            try
            {
                var ordersBid = await BittrexClient.GetOrderBook(TargetMarket, BookOrderType.Buy, 10); // !!
                var ordersAsk = await BittrexClient.GetOrderBook(TargetMarket, BookOrderType.Sell, 10);
                var bidPrice = await BittrexClient.GetMarketSummary(TargetMarket);

                var sumBid = ordersBid.Result.Sum(x => x.Quantity);
                var sumAsk = ordersAsk.Result.Sum(x => x.Quantity);
                var obs = new Observation(bidPrice.Result.Bid, bidPrice.Result.Ask, sumAsk, sumBid);
                return obs;
            } catch (Exception ex)
            {
                // TODO: logger
                return null;
            }
        }

        public async Task<bool> CheckConnection()
        {
            throw new NotImplementedException();
        }       
    }
}
