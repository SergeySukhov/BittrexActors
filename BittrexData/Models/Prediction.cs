﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace BittrexData.Models
{
    public class Prediction
    {
		[Key]
        public Guid Guid { get; set; }

		[ForeignKey("ActorDataGuid")]
		public Guid ActorDataGuid { get; set; }

		[Required]
        public DateTime ForTime { get; set; }

		[Required]
		public decimal OldPrice { get; set; }

		public virtual ActorData ActorData { get; set; }

        public virtual ICollection<PredictionUnit> RulePredictions { get; set; }

    }
}
