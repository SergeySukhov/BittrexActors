using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models
{
    public enum RuleType
    {
        ForBuy = 0,
        ForSell = 1,
    }

    public class Rule
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public string ConditionSplitedNames { get; set; }

        [Required]
        public int MinuteInterval { get; set; }

        [Required]
        public RuleType Type { get; set; }

        [Required]
        public double Rating { get; set; }

        [Required]
        public Guid ActorGuid { get; set; }

        [ForeignKey("ActorGuid")]
        public virtual Actor Actor { get; set; }
    }

    
    

}
