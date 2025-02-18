using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class UserQueriesViewModel
    {
   

        [Required]
        public string userId { get; set; }

        [Required]
        public string userName { get; set; }

        [Required]
        public int fbPageId { get; set; }

        [Required]
        public string userQuestion { get; set; }
        public QuizeLang quizeLang { get; set; }

    }
}
