using Microsoft.AspNetCore.Http;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class WrapperHeaderVM
    {
        public string heading { get; set; }
        public string subHeading { get; set; }
        public string url { get; set; }
        public int isActive { get; set; } = 1;
        public int WrapperHeaderId { get; set; }
        public WrapperHeader WrapperHeader { get; set; }
        public int WrapperHeaderImgId { get; set; }
        public string imgUrl { get; set; }
        public IFormFile fileUrl { get; set; }
        public int sortOrder { get; set; } = 1;
        public int status { get; set; } = 1;
        public int branchInfoId { get; set; } = 1;
        public IEnumerable<WrapperHeader> wrapperHeader { get; set; }
        public IEnumerable<WrapperHeaderImg> wrapperHeaderImg { get; set; }
    }
}
