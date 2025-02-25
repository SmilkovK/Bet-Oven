using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class AllLeagues
    {
        public LeagueInfo League { get; set; }
        public CountryInfo Country { get; set; }
        public List<SeasonInfo> Seasons { get; set; }
    }

    public class LeagueInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Logo { get; set; }
    }

    public class CountryInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Flag { get; set; }
    }
    public class SeasonInfo
    {
        public int Year { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool Current { get; set; }
    }
    public class Fixture
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public Teams Teams { get; set; }
        public Goals Goals { get; set; }
    }

    public class Teams
    {
        public TeamInfo Home { get; set; }
        public TeamInfo Away { get; set; }
    }

    public class TeamInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
    }

    public class Goals
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }
}
