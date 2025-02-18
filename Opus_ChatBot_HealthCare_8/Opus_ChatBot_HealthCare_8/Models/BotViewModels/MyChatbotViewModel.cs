using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class MyChatbotViewModel
    {
        public ChatbotInfo ChatbotInfo { get; set; }
        public IEnumerable<BotRackInfoDetail> BotRackInfoDetails { get; set; }
        public IEnumerable<string> DoctorInfos { get; set; }
        public IEnumerable<string> DepartmentInfos { get; set; }
        public IEnumerable<WrapperHeaderImg> wrapperHeaderImgs { get; set; }
    }
}
