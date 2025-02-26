using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class Matches
    {
        public int Id { get; set; }
        public LeagueInfo League { get; set; }
        public DateTime? Date { get; set; }
        public Teams Teams { get; set; } = new Teams();
        public Odds Odds { get; set; } = new Odds();
    }
}
