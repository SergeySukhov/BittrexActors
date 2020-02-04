using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BittrexData.Models
{
	public class BalancedRule
	{
		[Key]
		public Guid Guid { get; set; }

		public virtual ActorData ActorData { get; set; } 

		public string RuleName { get; set; }
		public OperationType Type { get; set; }
	}
}
