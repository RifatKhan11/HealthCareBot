namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class InputGroupDetail:BotBase
    {
        public int masterId { get; set; }
        public InputGroupMaster master { get; set; }

        public string inputName { get; set; }
        public string parameterName { get; set; }
        public string placeHolder { get; set; }
        public string inputType { get; set; }
        public int status { get; set; } = 1;
    }
}
