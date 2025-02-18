using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class AppointmentListViewModel
    {
        public int id { get; set; }
        public string phone { get; set; }
        public string frmDate { get; set; }
        public string time { get; set; }
        public string status { get; set; }




        public IEnumerable<PatientInfoViewModel> patientInfoViewModels { get; set; }
    }
}
