using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Models.KeyWord
{
    public class KeyWordInfo:BotBase
    {
        public string nameEn { get; set; }
        public string nameBn { get; set; }
        public int status { get; set; } = 1;


        public int? KeyWordQuesAnsId { get; set; }
        public KeyWordQuesAns KeyWordQuesAns { get; set; }
    }
}
