using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BotFlow
    {
        public string ID { get; set; }

        [Required]
        public string currentFlow { get; set; }

        public DateTime DateTime { get; set; }
    }
}
