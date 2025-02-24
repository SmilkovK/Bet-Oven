using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportDomain.models
{
    public class Fixture
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public Teams Teams { get; set; }
        public Goals Goals { get; set; }
    }

    public class Teams
    {
        public TeamInfo Home { get; set; }
        public TeamInfo Away { get; set; }
    }

    public class TeamInfo
    {
        public string Name { get; set; }
    }

    public class Goals
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }
}
