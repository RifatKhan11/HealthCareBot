using System.Data.Common;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class SaveMenuReaderViewModel
    {
        public int Id { get; set; }
        public int parentNodeEn { get; set; }
        public string questionEn { get; set; }
        public string parameterName { get; set; }
        public int stepNo { get; set; } = 0;
    }
}
