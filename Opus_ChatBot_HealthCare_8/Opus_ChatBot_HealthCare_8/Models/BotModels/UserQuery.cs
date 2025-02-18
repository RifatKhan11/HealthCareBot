using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class UserQuery : BotBase
    {
        public string uhid { get; set; }
        public string email { get; set; }
        public string querytime { get; set; }
        public string querydate { get; set; }
        public string query { get; set; }
        public string phone { get; set; }
        public DateTime effectiveDate { get; set; } = DateTime.Now;
        public string connectionId { get; set; }
        public int replied { get; set; } = 0;
        public DateTime? repliedDate { get; set; }
        public string replyText { get; set; }
        public int status { get; set; }
    }
}
