using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class OnlineUserVm
    {
        public string botKey { get; set; }
        public string connectionId { get; set; }
        public DateTime? entryDate { get; set; } = DateTime.Now;
    }
}
