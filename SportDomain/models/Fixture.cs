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
        public string DisplayDate => Date.HasValue
        ? Date.Value.ToString("MMM d, HH:mm UTC")
        : "Time TBD";
        public int Timestamp { get; set; } 
        public LeagueInfo League { get; set; }
        public string Status { get; set; }
        public Teams Teams { get; set; }
        public Goals Goals { get; set; }
        public Odds Odds { get; set; }
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
    public class MatchStats
    {
        public TeamStatistics Home { get; set; }
        public TeamStatistics Away { get; set; }
    }
    public class TeamStatistics
    {
        public int ShotsOnGoal { get; set; }
        public int ShotsOffGoal { get; set; }
        public int TotalShots { get; set; }
        public int Fouls { get; set; }
        public int CornerKicks { get; set; }
        public int Offsides { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
    public class LeagueResponse
    {
        public LeagueStandings League { get; set; }
    }
    public class LeagueStandings
    {
        public List<List<Standing>> Standings { get; set; }
    }
    public class Standing
    {
        public int Rank { get; set; }
        public TeamInfo Team { get; set; }
        public int Points { get; set; }
        public int GoalsDiff { get; set; }
        public AllStats All { get; set; }
    }

    public class AllStats
    {
        public int Played { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lose { get; set; }
    }
    public class StandingsViewModel
    {
        public List<Standing> Standings { get; set; }
        public List<int> AvailableSeasons { get; set; }
        public int SelectedSeason { get; set; }
        public int LeagueId { get; set; }
    }
    public class OddsResponse
    {
        public List<Bookmaker> Bookmakers { get; set; }
    }

    public class Odds
    {
        public List<Bookmaker> Bookmakers { get; set; } = new List<Bookmaker>();
    }

    public class Bookmaker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Bet> Bets { get; set; } = new List<Bet>();
    }

    public class Bet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BetValue> Values { get; set; } = new List<BetValue>();
    }

    public class BetValue
    {
        public string Value { get; set; }
        public double Odd { get; set; }
    }
}
