using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BittrexData.Models
{
    public class Prediction
    {
		[Key]
        public Guid Guid { get; set; }

        [Required]
        public DateTime ForTime { get; set; }


        public virtual ActorData ActorData { get; set; }

        public virtual ICollection<PredictionUnit> RulePredictions { get; set; }

    }
}
