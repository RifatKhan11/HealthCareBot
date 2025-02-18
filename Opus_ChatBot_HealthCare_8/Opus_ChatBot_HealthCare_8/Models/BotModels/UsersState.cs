using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class UsersState: BotBase
    {
        [Required]
        public string userId { get; set; }

        [Required]
        public string pageId { get; set; }

        [Required]
        public string laststate { get; set; }
    }
}
