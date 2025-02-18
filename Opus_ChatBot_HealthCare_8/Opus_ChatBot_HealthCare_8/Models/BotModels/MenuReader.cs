namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class MenuReader:BotBase
    {
        public int? menuId { get; set; }
        public Menu menu { get; set; }

        public string message { get; set; }
        public string htmlText { get; set; }
        public string Type { get; set; } = "Input";
        public string parameterName { get; set; }
        public int stepNo { get; set; } = 0;
    }
}
