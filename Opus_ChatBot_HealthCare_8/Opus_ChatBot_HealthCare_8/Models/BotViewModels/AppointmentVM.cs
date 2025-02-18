using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class AppointmentVM
    {
        public int? id{get;set;}
        public string bookingNo { get; set; }
        public int? doctorInfoId { get; set; }
        public string doctorname { get; set; }
        public string departmentName { get; set; }
        public virtual DoctorInfo doctorInfo { get; set; }
        public string userInfoId { get; set; }
        public string mobile { get; set; }
        public string UHID { get; set; }
        public string name { get; set; }
        public string dob { get; set; }
        public string gender { get; set; }
        public virtual UserInfo userInfo { get; set; }
        public int? weeksId { get; set; }
        public virtual Weeks weeks { get; set; }
        public string date { get; set; }
        public string isVerified { get; set; }
        public string status { get; set; }    //0=Appointed, 1=verified, 3=Confirm, 5=Canceled, 6=Rejected
        public string time { get; set; }
        public int? doctorVisitTimeId { get; set; }
        public virtual DoctorVisitTimePeriod doctorVisitTime { get; set; }
        public int totalAppointment { get; set; }
        public string appointStatus { get; set; }
    }
    public class ChartVM
    {
        public string date { get; set; }
        public int totalAppointment { get; set; }
    }

}
