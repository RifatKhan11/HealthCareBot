namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class ConnectionInfo:BotBase
    {
        public string userId { get; set; }
        public string connectionId { get; set; }
        public int? wrapperDetailsId { get; set; }
        public BotRackInfoDetail wrapperDetails { get; set; }

        public int? isTextMsg { get; set; } = 0; //0=text, 1=auto
    }
}
