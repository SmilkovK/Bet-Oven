using SportDomain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.DTO
{
    public class BetValueDTO
    {
        public string Value { get; set; }
        public string Odd { get; set; }
    }

    public class BetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BetValueDTO> Values { get; set; }
    }

    public class BookmakerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BetDTO> Bets { get; set; }
    }

    public class OddsInfoDTO
    {
        public LeagueInfo League { get; set; }
        public Fixture Fixture { get; set; }
        public string Update { get; set; }
        public List<BookmakerDTO> Bookmakers { get; set; }
    }

    public class ApiFootballOddsResponseDTO
    {
        public List<OddsInfoDTO> Response { get; set; }
    }

}
