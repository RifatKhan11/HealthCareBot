using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    [NotMapped]
    public class MenuQuestionAnswer
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionTextEN { get; set; }
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string AnswerTextEN { get; set; }
        public bool? IsLast { get; set; }
    }
}
