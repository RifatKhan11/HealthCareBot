using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Microsoft.AspNetCore.Http;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class FileViewModel
    {
        [Required]
        public IFormFile MyFile { get; set; }

        public string baseUrl { get; set; }

        public IEnumerable<BotModels.File> Files { get; set; }
    }
}
