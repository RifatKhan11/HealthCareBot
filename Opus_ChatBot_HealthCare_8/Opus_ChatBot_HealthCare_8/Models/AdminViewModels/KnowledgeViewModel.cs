using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class KnowledgeViewModel
    {
        public int Id { get; set; }
        public string nodeName { get; set; }
        public int facebookPageId { get; set; }
        public string question { get; set; }
        public int? type { get; set; }
        public string answer { get; set; }
        public string questionkey { get; set; }
        public string keyword { get; set; }
        public string more { get; set; }
        public int? questioncategoryId { get; set; }
        public int? status { get; set; }
        public int? isquestion { get; set; }
        public int? priority { get; set; }
        public int? IsLoop { get; set; }
        public int questionOrder { get; set; }
        public int? keyWordQuesAnsId { get; set; }
        public int? nextNodeId { get; set; }
        public string[] nodes { get; set; }
        public string responseApi { get; set; }
        public int? smsOtp { get; set; } = 0;
        public string refName { get; set; }
        public int? departmentId { get; set; }
        public int? doctorId { get; set; }

        public IEnumerable<KeyWordQuesAns> keyWordQuesAns { get; set; }
        public IEnumerable<questionCategory> questionCategories { get; set; } 
        
    }
}
