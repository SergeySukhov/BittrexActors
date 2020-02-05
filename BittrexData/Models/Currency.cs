using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BittrexData.Models
{
	public class Currency
	{
		[Key]
		public Guid Guid { get; set; }

		[Required]
		public string CurrencyName { get; set; }

		[Required]
		public decimal SellPrice { get; set; }

		[Required]
		public decimal BuyPrice { get; set; }

		[Required]
		public decimal VolumeBtc { get; set; }

		[Required]
		public decimal VolumeCurrency { get; set; }
		
		[Required]
		public decimal High { get; set; }

		[Required]
		public decimal Low { get; set; }

		[Required]
		public DateTime DateTime { get; set; }

	}
}
