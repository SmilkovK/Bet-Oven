using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.DTO
{
    public class ApiStatsResponse
    {
        public string Get { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<string> Errors { get; set; }
        public int Results { get; set; }
        public PagingInfo Paging { get; set; }
        public List<SportDomain.models.TeamStatResponse> Response { get; set; }
    }

    public class PagingInfo
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }
}
