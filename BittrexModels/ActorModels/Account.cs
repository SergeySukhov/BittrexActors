using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public class Account
    {
        public string TargetCurrencyName { get; set; }
        public decimal BtcCount
        {
            get { return BtcCount; }
            set
            {
                if (BtcCount < 0) throw new Exception("negative btc count");
                BtcCount = value;
            }
        }
        public decimal TargetCurrencyCount
        {
            get { return TargetCurrencyCount; }
            set
            {
                if (TargetCurrencyCount < 0) throw new Exception($"negative {TargetCurrencyName} count");
                BtcCount = value;
            }
        }
    }
}
