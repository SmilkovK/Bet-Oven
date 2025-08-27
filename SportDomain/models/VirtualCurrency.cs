using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class VirtualCurrency
    {
        [Key]
        public int Id { get; set; }
        public float CurrencyAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? BetUserId { get; set; }
        public BetUser? BetUser { get; set; }
        public bool IsBalanceRecord { get; set; } = false;
    }
}
