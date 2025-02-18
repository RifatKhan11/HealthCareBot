using System.Collections;
using System.Collections.Generic;
using static Opus_ChatBot_HealthCare_8.Controllers.HomeController;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DoctorWithSlotsVm
    {
        public DoctorVm doctor { get; set; }
        public IEnumerable<DoctorSlotVm> slots { get; set; }
        public List<string> dates { get; set; }
    }


    public class DoctorWithSlotsVm2
    {
        public DoctorVm2 doctor { get; set; }
        public IEnumerable<DoctorSlotVm> slots { get; set; }
        public List<string> dates { get; set; }
    }
}
