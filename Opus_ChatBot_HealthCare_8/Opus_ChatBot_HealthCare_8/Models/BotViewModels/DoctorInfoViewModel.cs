using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DoctorInfoViewModel
    {
        public int? Id { get; set; }
        public int? menuId { get; set; }
        public string doctorName { get; set; }
        public string doctorNameBn { get; set; }
        public string designationName { get; set; }
        public string designationNameBn { get; set; }
        public string gender { get; set; }
        public string details { get; set; }

        public IEnumerable<DoctorInfo> doctorInfos { get; set; }
        public IEnumerable<Menu> menus { get; set; }
    }
}
