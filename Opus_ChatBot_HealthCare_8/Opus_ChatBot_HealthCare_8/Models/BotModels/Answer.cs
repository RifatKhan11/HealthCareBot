using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Answer : BotBase
    {
        [Required]
        public string AnswerText { get; set; }

        [Required]
        public string AnswerTextEN { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        [Required]
        public int AnswerTypeId { get; set; }

        public AnswerType AnswerType { get; set; }
    }
}
