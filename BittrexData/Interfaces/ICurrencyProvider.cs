using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexData.Interfaces
{
    public interface ICurrencyProvider
    {
        decimal FindPriceByTime(DateTime dateTime, string currencyName);
    }
}
