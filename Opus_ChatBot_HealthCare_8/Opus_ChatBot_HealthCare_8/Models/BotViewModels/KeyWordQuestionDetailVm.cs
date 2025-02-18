namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class KeyWordQuestionDetailVm
    {
        public string question { get; set; }
        public string answer { get; set; }
        public int? priority { get; set; }
        public string questionKey { get; set; }
        public string type { get; set; }
        public int? orderNo { get; set; }
        public string status { get; set; }
        public string isQuestion { get; set; }
        public string nextNode { get; set; }
        public string SmsOTP { get; set; }
        public string Department { get; set; }
        public string Doctor { get; set; }
        public string Reference { get; set; }
    }
}
