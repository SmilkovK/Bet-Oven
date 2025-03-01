using SportDomain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SportDomain.models
{
    public class LeagueMatchesViewModel
    {
        public List<AllLeagues> Leagues { get; set; }
        public Dictionary<int, List<Fixture>> FixturesGroupedByLeague { get; set; }
    }
}