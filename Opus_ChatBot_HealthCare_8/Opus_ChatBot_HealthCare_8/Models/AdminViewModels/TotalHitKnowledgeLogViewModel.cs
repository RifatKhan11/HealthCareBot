using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class TotalHitKnowledgeLogViewModel
    {
        public string question { get; set; }
        //public string answer { get; set; }
        //public string categoryName { get; set; }

        //public DateTime? dateTime { get; set; }
        public int? totalHit { get; set; }

        public string uquestion { get; set; }
        public DateTime? hitDate { get; set; }

    }
}
