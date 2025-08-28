using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.DTO
{
    public class UserBetDto
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Type { get; set; }
        public float Odds { get; set; }
        public int FixtureId { get; set; }
    }
}
