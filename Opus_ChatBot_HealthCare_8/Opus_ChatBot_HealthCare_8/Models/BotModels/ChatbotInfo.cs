using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class ChatbotInfo:BotBase
    {
        public string BotName { get; set; }
        public string BotLogo { get; set; }


        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string scriptText { get; set; }
    }
}
