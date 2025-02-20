using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.Identity
{
    public class BetUser : IdentityUser
    {
        public string? Name { get; set; }
        public DateOnly Date { get; set; }
    }
}
