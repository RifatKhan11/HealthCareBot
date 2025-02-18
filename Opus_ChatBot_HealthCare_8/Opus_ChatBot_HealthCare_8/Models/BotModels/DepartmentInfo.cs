namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class DepartmentInfo:BotBase
    {
        public string departmentCode { get; set; }
        public string departmentName { get; set; }
        public string shortName { get; set; }
        public string thumbUrl { get; set; }
        public int status { get; set; } = 1;
        public string location { get; set; }
    }
}
