using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BittrexData.Models
{
	public class ActorDataDto
	{
		[Key]
		public Guid Guid { get; set; }

		[Required]
		public double HesitationToBuy { get; set; }

		[Required]
		public double HesitationToSell { get; set; }

		[Required]
		public double ChangeCoefficient { get; set; }

		[Required]
		public int Generation { get; set; }

		[Required]
		public bool IsAlive { get; set; }

		[Required]
		public DateTime LastActionTime { get; set; }

		[Required]
		public DateTime CurrentTime { get; set; }

		[Required]
		public ActorType ActorType { get; set; }

        public virtual AccountDto Account { get; set; }

		public virtual ICollection<BalancedRuleDto> Rules { get; set; }
		public virtual ICollection<PredictionDto> Predictions { get; set; }
		public virtual ICollection<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
	}
}
