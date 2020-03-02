using BittrexData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BittrexCore.Models
{
	public class Currency
	{
		public Guid Guid { get; set; }

		public string CurrencyName { get; set; }

		public decimal SellPrice { get; set; }

		public decimal BuyPrice { get; set; }

		public decimal VolumeBtc { get; set; }

		public decimal VolumeCurrency { get; set; }
		
		public decimal High { get; set; }

		public decimal Low { get; set; }

		public DateTime DateTime { get; set; }

	}
}
