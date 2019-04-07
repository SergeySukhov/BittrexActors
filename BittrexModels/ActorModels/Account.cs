using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public class Account
    {
        private decimal btcCount = 0, targetCurrencyCount;

        public string TargetCurrencyName { get; set; }

        public decimal BtcCount
        {
            get => btcCount; set
            {
                if (value < 0) throw new Exception("Невозможно установить отрицательный баланс!");
                btcCount = value;
            }
        }
        public decimal TargetCurrencyCount
        {
            get => targetCurrencyCount; set
            {
                if (value < 0) throw new Exception("Невозможно установить отрицательный баланс!");
                targetCurrencyCount = value;
            }
        }
        
    }
}
