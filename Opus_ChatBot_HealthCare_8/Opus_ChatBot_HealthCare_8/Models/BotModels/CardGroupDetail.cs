namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class CardGroupDetail:BotBase
    {
        public string cardThumb { get; set; }
        public string cardTitle { get; set; }
        public string cardSubtitle { get; set; }

        public int status { get; set; } = 1;
        public int sortOrder { get; set; } = 1;

        public string functionName { get; set; }

        public int cardGroupMasterId { get; set; }
        public CardGroupMaster cardGroupMaster { get; set; }
    }
}
