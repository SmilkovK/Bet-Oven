using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class RewardsViewModel
    {
        public float CurrentBalance { get; set; }
        public float Reward1Threshold { get; set; }
        public float Reward2Threshold { get; set; }
        public float DefaultDailyLimit { get; set; }
        public float Reward1DailyLimit { get; set; }
        public bool HasReward1 { get; set; }
        public bool HasReward2 { get; set; }
        public string Reward1Description { get; set; }
        public string Reward2Description { get; set; }
    }
}
