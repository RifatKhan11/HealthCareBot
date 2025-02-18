namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class WrapperHeader:BotBase
    {
        public string heading { get; set; }
        public string subHeading { get; set; }
        public string url { get; set; }
        public int isActive { get; set; } = 1;
    }
}
