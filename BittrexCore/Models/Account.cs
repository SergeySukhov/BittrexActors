using BittrexData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BittrexCore.Models
{
    public class Account
    {
        public Guid Guid;
		public string CurrencyName;
        public decimal BtcCount;
        public decimal CurrencyCount;
    }
}
