using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BittrexData.Models
{
	public class PredictionUnit
	{
		[Key]
		public Guid Guid { get; set; }

		[Required]
		public string RuleName { get; set; }

		[Required]
		public double Coefficient { get; set; }

        public virtual Prediction Prediction { get; set; }

    }
}
