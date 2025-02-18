using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.MasterData
{
    public class questionCategory:BotBase
    {
        public string categoryName { get; set; }
        public int? facebookPageId { get; set; }
        public FacebookPage facebookPage { get; set; }
      
    }
}
