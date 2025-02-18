using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class BotKnowledgeViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Questiontext { get; set; }
        public string TextReply { get; set; }
        public int KeyWordQuesAnsId { get; set; }
        public int Status { get; set; }
        public string BotKey { get; set; }
        public IEnumerable<KeyWordQuesAns> KeyWordQuesAnsS { get; set; }
        public IEnumerable<BotKnowledge> BotKnowledgeS { get; set; }
        public IEnumerable<DepartmentInfo> DepartmentInfos { get; set; }
        public IEnumerable<DoctorInfo> DoctorInfos { get; set; }


        public int? id { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public int? priority { get; set; }
        public int? Type { get; set; }
        public int? Order { get; set; }
        public int? IsQuestion { get; set; }
        public int? NextNode { get; set; }
        public string ResponseApi { get; set; }
        public int? SMSOTP { get; set; }
        public string RefName { get; set; }
        public int? Department { get; set; }
        public int? Doctor { get; set; }

    }
}
