using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class AppoinmentInfo:BotBase
    {
        public string bookingNo { get; set; }
        public int? doctorInfoId { get; set; }
        public string doctorName { get; set; }
        public string designationName { get; set; }
        public string departmentName { get; set; }
        public string specializationsName { get; set; }
        //public virtual DoctorInfo doctorInfo { get; set; }
        public string userInfoId { get; set; }
        public virtual UserInfo userInfo { get; set; }
        public int? weeksId { get; set; }
        public virtual Weeks weeks { get; set; }
        public DateTime? date { get; set; }
        public int? isVerified { get; set; }
        public int? status { get; set; }    //0=Appointed, 1=verified, 3=Confirm, 5=Canceled, 6=Rejected
        public string time { get; set; }
        public int? doctorVisitTimeId { get; set; }
        public virtual DoctorVisitTimePeriod doctorVisitTime { get; set; }
        public string appointStatus { get; set; }
    }
}
