using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class TimePeriod:BotBase
    {
        public string shiftPeriod { get; set; } //Morning,Evening
        public string timeSlot { get; set; }
        public string timeSlotBn { get; set; }
        public int? sortOrder { get; set; }
        public int? isActive { get; set; }
    }
}
