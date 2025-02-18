using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class UserFeedbackQueryViewModel
    { 
            //public int feedbackId { get; set; }
            //public string name { get; set; }
            public string email { get; set; }
           public string phone { get; set; }
            public string replyText { get; set; }
            public string query { get; set; }
            public string connectionId { get; set; }
            public int replied { get; set; } = 0;
            public int queryId { get; set; }
            public string uhid { get; set; }
            public string querydate { get; set; }
            public string querytime { get; set; }
            public int querystatus { get; set; }
            public string querystatusType { get; set; }
            public string botKey { get; set; }
            //public IEnumerable<UserFeedback> Feedbacks { get; set; }
            //public IEnumerable<UserQuery> Queries { get; set; }
           
    }
    public class QueryViewModel
    {
        public IEnumerable<UserFeedbackQueryViewModel> userFeedbackQueryViewModels { get; set; }
    }
 }
