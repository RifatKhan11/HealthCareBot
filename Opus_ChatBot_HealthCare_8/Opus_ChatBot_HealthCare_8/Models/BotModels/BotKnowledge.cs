using Opus_ChatBot_HealthCare_8.Models.KeyWord;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BotKnowledge:BotBase
    {
        public string connectionId { get; set; }
        public string question { get; set; }
        public string textReply { get; set; }
        
        public int? keyWordQuesAnsId { get; set; }
        public KeyWordQuesAns keyWordQuesAns { get; set; }

        public int status { get; set; } = 1;
    }
}
