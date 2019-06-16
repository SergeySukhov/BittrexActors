using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManager.Models
{
    public class Account
    {
        [Key]
        [ForeignKey("Actor")]
        public Guid Guid { get; set; }

        [Required]
        public string CurrencyName { get; set; }
                
        [Required]
        public decimal BtcCount { get; set; }
        
        [Required]
        public decimal CurrencyCount { get; set; }

        public virtual Actor Actor { get; set; }
    }
}
