using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BittrexData.Models
{
	public class ActorData
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
		public ActorType ActorType { get; set; }

		public virtual ICollection<BalancedRule> Rules { get; set; }
		public virtual ICollection<Prediction> Predictions { get; set; }
		public virtual ICollection<Transaction> Transactions { get; set; }
	}
}
