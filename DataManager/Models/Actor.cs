using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Models
{
    public class Actor
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public string TargetMarket { get; set; }

        [Required]
        public double HesitationToBuy { get; set; }

        [Required]
        public double HesitationToSell { get; set; }

        [Required]
        public double OperationPercent { get; set; }
                
        [Required]
        public DateTime LastActionTime { get; set; }

        [Required]
        public TimeSpan ActivationSpan { get; set; }        
        
        
        public Account CountVolume { get; set; } // BTC

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Rule> Rules { get; set; }
        public virtual ICollection<Observation> Observations { get; set; }
        
    }


}
