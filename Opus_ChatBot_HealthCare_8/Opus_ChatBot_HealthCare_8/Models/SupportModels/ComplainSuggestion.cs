using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.SupportModels
{
    public class ComplainSuggestion:BotBase
    {
        public int? type { get; set; } //1=complain,2=suggestion

        public string text { get; set; }

        public string passportNumber { get; set; }
        public DateTime? dateTime { get; set; }
    }
}
