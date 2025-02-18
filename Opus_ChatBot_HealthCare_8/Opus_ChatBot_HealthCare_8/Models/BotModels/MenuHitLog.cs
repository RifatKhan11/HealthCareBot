using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class MenuHitLog:BotBase
    {
        public int? menuId { get; set; }
        public Menu menu { get; set; }

        public int? facebookPageId { get; set; }
        public FacebookPage facebookPage { get; set; }
        public DateTime? dateTime { get; set; }
        public string connectionId { get; set; }
    }
}
