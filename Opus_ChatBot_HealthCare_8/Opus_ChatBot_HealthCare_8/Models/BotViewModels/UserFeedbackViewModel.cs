using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class UserFeedbackViewModel
    {
        public int feedbackId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string message { get; set; }
        public string query { get; set; }
        public string connectionId { get; set; }
        public int replied { get; set; } = 0;
        public int queryId { get; set; }
        public string uhid { get; set; }
        public string querydate { get; set; }
        public string querytime { get; set; }
        public int querystatus { get; set; }
        public string querystatusType { get; set; }
        public string botKeyNew { get; set; }
        public IEnumerable<UserFeedback> Feedbacks { get; set; }
        public IEnumerable<UserQuery> Queries { get; set; }
    }
}
