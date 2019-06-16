using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models
{
    public class Observation
    {
        [Key]
        public Guid Guid { get; set; }

        [Required]
        public string MarketName { get; set; }

        [Required]
        public DateTime ObservationTime { get; set; }

        [Required]
        public decimal BidPrice { get; set; }

        [Required]
        public decimal AskPrice { get; set; }

        [Required]
        public decimal OrderAskSum { get; set; }

        [Required]
        public decimal OrderBidSum { get; set; }

        [Required]
        public Guid ActorGuid { get; set; }

        [ForeignKey("ActorGuid")]
        public virtual Actor Actor { get; set; }

    }
}
