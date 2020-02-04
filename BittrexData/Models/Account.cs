using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BittrexData.Models
{
    public class Account
    {
		[Key]
		public Guid Guid { get; set; }

        [ForeignKey("ActorDataGuid")]
        public Guid ActorDataGuid { get; set; }

		[Required]
		public string CurrencyName { get; set; }

		[Required]
		public decimal BtcCount { get; set; }

		[Required]
		public decimal CurrencyCount { get; set; }

        public virtual ActorData ActorData { get; set; }
	}
}
