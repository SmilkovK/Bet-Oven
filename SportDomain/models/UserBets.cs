using SportDomain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    class UserBets
    {
        enum BetStatus
        {
            Active,
            Finished
        }
        public List<BetValue> BetValues { get; set; }
        public BetUser User { get; set; }
        public string UserId { get; set; }

    }
}
