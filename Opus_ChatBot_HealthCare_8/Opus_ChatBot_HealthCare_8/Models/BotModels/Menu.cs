using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class Menu: BotBase
    {
        [Required]
        //[StringLength(25)]
        public string MenuName { get; set; }

        [Required]
        //[StringLength(25)]
        public string MenuNameEN { get; set; }

        [DefaultValue(0)]
        public int ParrentId { get; set; }

        [DefaultValue(true)]
        public bool IsLast { get; set; }

        [Required]
        public int FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }

        public string menuType { get; set; } //Button, Input
        public string responseApi { get; set; }

    }
}
