using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class ServiceFlow
    {

        public string Id { get; set; }

        [Required]
        public string InfoType { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        public DateTime DateTime { get; set; }

        public int StepNo { get; set; }
        public int MenuId { get; set; }

        public int Attempt { get; set; }
        public string botKey { get; set; }
        public string connectionId { get; set; }
        public int status { get; set; } = 0; //1=passed
        public string questionText { get; set; }
        public string answerText { get; set; }

        public int? keyWordQuesAnsId { get; set; }
        public KeyWordQuesAns keyWordQuesAns { get; set; }

        public int? branchInfoId { get; set; }
        public BranchInfo branchInfo { get; set; }
    }
}
