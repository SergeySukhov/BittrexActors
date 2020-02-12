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

        public decimal FindPriceByTime(DateTime dateTime, string currencyName)
        {
            var context = new BittrexCurrencyDbContext();
			// TODO: выбрать лучший из вариантов
			var resultCur = context.CurrencyDatas
							.Where(x => x.CurrencyName == currencyName &&
										x.DateTime.Date == dateTime.Date &&
										x.DateTime.Hour == dateTime.Hour)
							.FirstOrDefault();

            if (resultCur == null) return -1.0m;
            else
				return resultCur.BuyPrice; // !!
        }
    }
}
