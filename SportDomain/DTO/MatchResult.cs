using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.DTO
{
    public class MatchResult
    {
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public bool Finished { get; set; }
    }
}
