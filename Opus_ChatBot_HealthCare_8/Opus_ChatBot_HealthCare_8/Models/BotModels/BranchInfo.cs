namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BranchInfo : BotBase
    {
        public string branchCode { get; set; }
        public string branchName { get; set; }
        public string address { get; set; }
        public string contactPerson { get; set; }
        public string mapUrl { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int isActive { get; set; } = 1;
    }
}
