using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Question : BotBase
    {
        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionTextEN { get; set; }

        [Required]
        public int MenuId { get; set; }

        public Menu Menu { get; set; }

    }
}
