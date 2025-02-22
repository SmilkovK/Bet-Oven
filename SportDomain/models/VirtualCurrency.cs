using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class VirtualCurrency : BaseEntity
    {
        public float currencyAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? BetUserId { get; set; }
        public BetUser? BetUser { get; set; }
    }
}
