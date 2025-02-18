using Opus_ChatBot_HealthCare_8.Models.KeyWord;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class QuestionNavigation:BotBase
    {
        public int? requestQuestionId { get; set; }
        public KeyWordQuesAns requestQuestion { get; set; }

        public int? responseQuestionId { get; set; }
        public KeyWordQuesAns responseQuestion { get; set; }
    }
}
