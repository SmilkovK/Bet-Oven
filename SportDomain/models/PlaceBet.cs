using SportDomain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class PlaceBet
    {
        public List<UserBetDto> Bets { get; set; }
        public float TotalStake { get; set; }
    }
}
