using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class BetConfirm
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public BetUser User { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserBet> Bets { get; set; }
        public string Status { get; set; } = "Pending";
        public bool IsPaidOut { get; set; } = false;
        public float CombinedPotentialWin { get; set; }

    }
}
