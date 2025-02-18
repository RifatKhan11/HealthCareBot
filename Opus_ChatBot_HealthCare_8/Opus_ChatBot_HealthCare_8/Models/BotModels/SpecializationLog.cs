using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class SpecializationLog:BotBase
    {
        public DateTime? logDate { get; set; }

        public string code { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }
}
