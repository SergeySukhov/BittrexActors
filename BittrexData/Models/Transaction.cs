using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BittrexData.Models
{
    public class Transaction
    {
		[Key]
        public Guid Guid { get; set; }

		[Required]
        public OperationType Type { get; set; }

		[Required]
        public DateTime Time { get; set; }

		[Required]
		public decimal CurrencyPrice { get; set; }

		[Required]
		public decimal BtcCount { get; set; }

		[Required]
		public TransactionResult TransactionResult { get; set; }
    }
}
