using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class OTPCode:BotBase
    {
        public string otpCode { get; set; }
        public DateTime? expireTime { get; set; } = DateTime.Now.AddMinutes(5);
        public string connectionId { get; set; }
        public string parameterName { get; set; }

        public int? refMenuId { get; set; }
        public KeyWordQuesAns refMenu { get; set; }
    }
}
