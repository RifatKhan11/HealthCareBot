using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class FacebookPage: BotBase
    {
        [Required]
        [StringLength(50)]
        public string PageId { get; set; }

        [Required]
        public string PageAccessToken { get; set; }

        //[Required]
        public string UserFbId { get; set; }

        [Required]
        public string PageGreetingMessage { get; set; }

        [Required]
        public string PageGreetingMessageEN { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public List<Menu> Menus { get; set; }

    }
}
