using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class File: BotBase
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int FacebookPageId { get; set; }

        public FacebookPage FacebookPage { get; set; }
    }
}
