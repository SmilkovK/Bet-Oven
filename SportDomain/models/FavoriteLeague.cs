using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class FavoriteLeague
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public BetUser User { get; set; }

        public int LeagueId { get; set; }
    }
}