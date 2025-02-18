using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.SupportModels
{
    public class PoliceClearenceLog: BotBase
    {
        public string passportNo { get; set; }
        public string refNo { get; set; }
        public string status { get; set; }
        public DateTime? date { get; set; }
    }
}
