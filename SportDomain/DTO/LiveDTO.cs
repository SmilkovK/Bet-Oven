using SportDomain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.DTO
{
    public class ApiFootballResponse
    {
        public List<ApiFixtureWrapper> Response { get; set; }
    }
    public class ApiFixtureWrapper
    {
        public ApiFixture Fixture { get; set; }
        public LeagueInfo League { get; set; }
        public Teams Teams { get; set; }
        public Goals Goals { get; set; }
        public bool Popular { get; set; }
    }
    public class ApiFixture
    {
        public int Id { get; set; }
        public int Timestamp { get; set; }
        public DateTime? Date { get; set; }
        public ApiStatus Status { get; set; }
    }
    public class ApiStatus
    {
        public string Long { get; set; }
        public string Short { get; set; }
        public int? Elapsed { get; set; }
    }
}
