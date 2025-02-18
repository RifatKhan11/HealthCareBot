namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class WrapperHeaderImg:BotBase
    {
        public int? WrapperHeaderId { get; set; }
        public WrapperHeader WrapperHeader { get; set; }
        public string imgUrl { get; set; }
        public int sortOrder { get; set; } = 1;
        public int status { get; set; } = 1;
    }
}
