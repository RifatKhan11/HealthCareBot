using System.Collections;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class DoctorTimeSlotVm
    {
        public int doctorId { get; set; }

        public int[] Sunday { get; set; }
        public int[] Monday { get; set; }
        public int[] Tuesday { get; set; }
        public int[] Wednesday { get; set; }
        public int[] Thursday { get; set; }
        public int[] Friday { get; set; }
        public int[] Saturday { get; set; }


        public IEnumerable<TimePeriod> TimePeriods { get; set; }
        public IEnumerable<DoctorInfo> DoctorInfos { get; set; }
        public IEnumerable<GetTimeSlotsByDoctorIdVm> DoctorTimeSlots { get; set; }
    }


    
}
