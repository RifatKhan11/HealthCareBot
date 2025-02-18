using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class MenuViewModel
    {
        [Required]
        //[StringLength(20, ErrorMessage = "Menu Name cannot be longer than 20 characters")]
        public string MenuName { get; set; }

        [Required]
        //[StringLength(, ErrorMessage = "Menu Name cannot be longer than 20 characters")]
        public string MenuNameEN { get; set; }

        public int ParrentMenuId { get; set; }
        public string botKey { get; set; }

        public string ParrentMenuName { get; set; }

        public string ParrentMenuNameEN { get; set; }

        public string IsLast { get; set; }

        public string Menus { get; set; }

        public int FbPageId { get; set; }

        public int Depth { get; set; }

        public string SubmitType { get; set; }
        public string menuType { get; set; }
        public string responseAPI { get; set; }

        public IEnumerable<MenuQuestionAnswer> menuQuestionAnswers { get; set; }
        public IEnumerable<Menu> AllMenus { get; set; }
    }
}
