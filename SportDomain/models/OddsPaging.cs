using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class ApiFootballOddsPagedResponse
    {
        public List<OddsInfo> Response { get; set; }
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

}
