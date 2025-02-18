using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class EvercareToken:BotBase
    {
        public string token { get; set; }
        public int isActive { get; set; } = 1;
        public string phone { get; set; }
        public DateTime expiryDate { get; set; }
        public int sentAlert { get; set; } = 0;
    }
}
