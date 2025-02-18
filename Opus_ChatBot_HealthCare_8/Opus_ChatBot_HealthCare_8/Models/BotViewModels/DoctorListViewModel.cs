using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DoctorListViewModel
    {
        public int? Id { get; set; }
        public IEnumerable<DoctorInfo> doctorInfos { get; set; }
        public IEnumerable<Menu> menus { get; set; }
        public IEnumerable<DepartmentInfo> departmentInfos { get; set; }
        public IEnumerable<DoctorSpecialization> doctorSpecializations { get; set; }
    }
}
