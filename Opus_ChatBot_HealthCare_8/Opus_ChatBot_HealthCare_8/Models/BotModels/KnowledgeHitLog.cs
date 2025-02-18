using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class KnowledgeHitLog: BotBase
    {
        public int? keyWordQuesAnsId { get; set; }
        public KeyWordQuesAns keyWordQuesAns { get; set; }

        public int? facebookPageId { get; set; }
        public FacebookPage facebookPage { get; set; }
        public DateTime? dateTime { get; set; }

        public int? unKnownKeyWordQuestionId { get; set; }
        public UnKnownKeyWordQuestion unKnownKeyWordQuestion { get; set; }

        public string connectionId { get; set; }
    }
}
