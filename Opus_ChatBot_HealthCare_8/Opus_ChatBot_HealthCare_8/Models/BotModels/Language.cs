using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Language: BotBase
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string PageId { get; set; }

        [Required]
        public string Lang { get; set; }
    }
}
