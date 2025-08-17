using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class ApiResponse<T>
    {
        public string Get { get; set; }
        public List<T> Response { get; set; }
        public object Errors { get; set; }
        public object Paging { get; set; }
        public int Results { get; set; }
    }

    public class FixtureWrapper
    {
        public FixtureInfo Fixture { get; set; }
        public LeagueInfo League { get; set; }
        public Teams Teams { get; set; }
        public Goals Goals { get; set; }
    }

    public class FixtureInfo
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public StatusInfo Status { get; set; }
    }

    public class StatusInfo
    {
        public string Long { get; set; }
        public string Short { get; set; }
        public int? Elapsed { get; set; }
    }

    public class OddsWrapper
    {
        public FixtureInfo Fixture { get; set; }
        public LeagueInfo League { get; set; }
        public List<Bookmaker> Bookmakers { get; set; }
        public string Update { get; set; }
    }
}
