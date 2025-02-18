using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels.Sub;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class QuestionReplayViewModel
    {
        public int QuestionId { get; set; }

        public int FbPageId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionTextEN { get; set; }

        public int AnswerId { get; set; }

        public string AnswereText  { get; set; }

        public string AnswereTextEN { get; set; }

        public int MenuId { get; set; }

        public int AnswerTypeId { get; set; }

        public List<Buttons>TextWithButton  { get; set; }
        public List<GenericTemplate> GenericTemplates { get; set; }


        public IEnumerable<MenuQuestionAnswer> menuQuestionAnswers { get; set; }
        public IEnumerable<AnswerType> AnswerTypes { get; set; }

    }
}
