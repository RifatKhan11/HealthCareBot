using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Analytics: BotBase
    {

        [Required]
        public string PageId { get; set; }
        
        [Required]
        public string SenderId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string QueryType { get; set; }

    }
}
