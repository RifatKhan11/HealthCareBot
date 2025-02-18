using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class TotalHitInfotViewModel
    {
        public string name { get; set; }
        public string passportNo { get; set; }
        public string refNo { get; set; }
        //public DateTime? applyDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
    }
}
