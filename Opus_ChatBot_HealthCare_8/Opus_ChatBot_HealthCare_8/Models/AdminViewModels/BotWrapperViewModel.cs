using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class BotWrapperViewModel
    {
        public int Id { get; set; }
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? BotKey { get; set; }

        public IEnumerable<BotWrapperViewModel> BotWrapperViewModels { get; set; }
        public IEnumerable<BotRackInfoMaster> botRackInfoMasters { get; set; }
    }

}
