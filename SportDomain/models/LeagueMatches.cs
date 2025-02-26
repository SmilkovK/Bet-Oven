using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportDomain.models;
namespace SportDomain.models
{
    class LeagueMatches
    {
        public LeagueInfo League { get; set; }
        public List<Fixture> Fixtures { get; set; }

    }
}
