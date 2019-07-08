using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models
{
    public enum OperationType
    {
        Buy = 0, // покупка целовой валюты - продажа базовой (btc)
        Sell = 1 // продажа целевой валюты - покупка базовой (btc)
    }
    public enum TransactionResult
    {
        Success = 0,
        WithWarnings = 1,
        Awaiting = 2,
        Error = 3,
        Canceled = 4,
        Created = 5
    }

    public class Transaction
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public OperationType Type { get; set; }

        [Required]
        public decimal CurrencySum { get; set; }

        public decimal ReleasePrice { get; set; }

        [Required]
        public string MarketName { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        public DateTime ReleaseTime { get; set; }

        [Required]
        public TransactionResult TransactionResult { get; set; }

        [Required]
        public Guid AccountGuid { get; set; }

        [NotMapped]
        public Account Account { get; set; }

    }
}
