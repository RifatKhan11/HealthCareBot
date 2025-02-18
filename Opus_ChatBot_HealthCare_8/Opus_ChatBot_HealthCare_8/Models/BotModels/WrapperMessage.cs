namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class WrapperMessage:BotBase
    {
        public int BotWrapperDetailId { get; set; }
        public BotRackInfoDetail BotWrapperDetail { get; set; }

        public string message { get; set; }
        public int orderNo { get; set; } = 1;
        public int status { get; set; } = 1;
        public int type { get; set; } = 1; //1=text, 2=button, 3=btn group
    }
}
