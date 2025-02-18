using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class AppointmentViewModel
    {
        public string msg { get; set; }
        public string doctorName { get; set; }
        public string patientName { get; set; }
        public string mobile { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string designationName { get; set; }
        public string departmentName { get; set; }
        public string UHID { get; set; }
        public string bookingNo { get; set; }
        public string status { get; set; }
        public AppoinmentInfo appoinment { get; set; }
        public List<string> dates { get; set; }
        public List<string> lstMorning { get; set; }
        public List<string> lstEvening { get; set; }
    }
}
