namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BotRackInfoDetail:BotBase
    {
        public string thumb { get; set; }
        public string heading { get; set; }
        public string subHeading { get; set; }
        public int sortOrder { get; set; } = 1;
        public string botKey { get; set; }

        public int? masterId { get; set; }
        public BotRackInfoMaster master { get; set; }

        public string firstMessage { get; set; }

        public int? menuId { get; set; }
        public Menu menu { get; set; }
        public string url { get; set; } = "";
    }
}
