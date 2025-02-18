using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class DoctorLog:BotBase
    {
        public DateTime? logDate { get; set; }

        public string doctorCode { get; set; }
        public string name { get; set; }
        public string nameBn { get; set; }
        public string departmentName { get; set; }
        public string designationName { get; set; }
        public string designationNameBn { get; set; }
        public int? menuId { get; set; }
        public int? status { get; set; }
        public string gender { get; set; }//Male,Female
        public string details { get; set; }

        public int? DepartmentId { get; set; }
        public int? doctorSpecializationId { get; set; }
    }
}
