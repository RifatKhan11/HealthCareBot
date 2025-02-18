using Opus_ChatBot_HealthCare_8.Models.KeyWord;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class MessageLog : BotBase
    {
        public string message { get; set; }
        public string rawMessage { get; set; }
        public string connectionId { get; set; }

        public int? menuId { get; set; }
        public Menu menu { get; set; }

        public string Type { get; set; } //Menu, Input

        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public string postedApi { get; set; }
        public string response { get; set; }

        public int? KeyWordQuesAnsId { get; set; }
        public KeyWordQuesAns KeyWordQuesAns { get; set; }

        public int? nextNodeId { get; set; }
    }
}
