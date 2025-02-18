using Opus_ChatBot_HealthCare_8.Models.KeyWord;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class InputGroupMaster:BotBase
    {
        public string name { get; set; }
        public int status { get; set; } = 1;

        public int menuId { get; set; }

        public KeyWordQuesAns menu { get; set; }
    }
}
