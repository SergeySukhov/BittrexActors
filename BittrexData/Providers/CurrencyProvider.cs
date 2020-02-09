using BittrexData.Contexts;
using BittrexData.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexData.Providers
{
    public class CurrencyProvider : ICurrencyProvider
    {

        public decimal FindClosetPrice(DateTime dateTime, string currencyName)
        {
            var context = new BittrexCurrencyDbContext();

            var resultCur = context.CurrencyDatas.Where(x => x.CurrencyName == currencyName && x.DateTime - dateTime < new TimeSpan(0, 30, 0)).FirstOrDefault();

            if (resultCur == null) return -1m;
            else return resultCur.BuyPrice;
        }
    }
}
