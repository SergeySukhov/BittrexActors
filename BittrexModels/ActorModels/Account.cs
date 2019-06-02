using BittrexModels.Interfaces;


namespace BittrexModels.ActorModels
{
    public class Account: IAccount
    {
        public string CurrencyName { get; set; }

        public decimal BtcCount { get; set; }
        public decimal CurrencyCount { get; set; }
        
    }
}
