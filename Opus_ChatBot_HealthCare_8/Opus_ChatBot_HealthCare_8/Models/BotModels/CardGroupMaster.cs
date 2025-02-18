namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class CardGroupMaster:BotBase
    {
        public string name { get; set; }
        public string category { get; set; }

        public int? menuId { get; set; }
        public Menu menu { get; set; }
    }
}
