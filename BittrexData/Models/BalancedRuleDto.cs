using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BittrexData.Models
{
	public class BalancedRuleDto
	{
		[Key]
		public Guid Guid { get; set; }

		[ForeignKey("ActorDataGuid")]
		public Guid ActorDataGuid { get; set; }

		[Required]
		public string RuleName { get; set; }

        [Required]
        public double Coefficient { get; set; }

        [Required]
        public OperationType Type { get; set; }

        public virtual ActorDataDto ActorData { get; set; }

    }
}
