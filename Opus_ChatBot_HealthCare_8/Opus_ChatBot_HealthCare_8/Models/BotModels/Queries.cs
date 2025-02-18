using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Queries: BotBase
    {
        [Required]
        public string QueriesText { get; set; }

        [Required]
        public string FbUserID { get; set; }

        [Required]
        public string FbUserName { get; set; }

        public string AnswerText { get; set; }

        public int FacebookPageId { get; set; }

        public FacebookPage FacebookPage { get; set; }
    }
}
