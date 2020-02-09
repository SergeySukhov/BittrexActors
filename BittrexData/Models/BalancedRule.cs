﻿using System;
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

        [Required]
		public string RuleName { get; set; }

        [Required]
        public double Coefficient { get; set; }

        [Required]
        public OperationType Type { get; set; }

        public virtual ActorData ActorData { get; set; }

    }
}
