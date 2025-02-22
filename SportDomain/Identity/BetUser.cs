using Microsoft.AspNetCore.Identity;
using SportDomain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.Identity
{
    public class BetUser : IdentityUser
    {
        public string? SiteName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public virtual ICollection<VirtualCurrency> Currencies { get; set; } = new List<VirtualCurrency>();
    }
}
