using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class UserBet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public BetUser User { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string BetType { get; set; }
        public float Odds { get; set; }
        public float Stake { get; set; }
        public float PotentialWin { get; set; }

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    }
}
