using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels.Sub
{
    public class GenericTemplate
    {
        [StringLength(80, ErrorMessage = "The title at most 80 characters long.")]
        public string title { get; set; }

        [StringLength(80, ErrorMessage = "The titleEN at most 80 characters long.")]
        public string titleEN { get; set; }

        public string image_url { get; set; }

        public string subtitle { get; set; }
        public string subtitleEN { get; set; }
        public List<Buttons> buttons { get; set; }
    }
}