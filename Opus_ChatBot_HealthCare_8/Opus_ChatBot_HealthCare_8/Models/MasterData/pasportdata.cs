using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.MasterData
{
    public class pasportdata
    {
        public IEnumerable<items> items { get; set; }
        public IEnumerable<links> links { get; set; }
        public bool hasMore { get; set; }
        public string limit { get; set; }
        public string offset { get; set; }
        public string count { get; set; }
    }
}
