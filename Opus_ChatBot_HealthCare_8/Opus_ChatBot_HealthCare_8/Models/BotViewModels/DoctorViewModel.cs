using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public int doctorSpecializationId { get; set; }
        public int DepartmentId { get; set; }
        public string doctorCode { get; set; }
        public string name { get; set; }
        public string departmentName { get; set; }
        public string designationName { get; set; }
        public string doctorSpecialization { get; set; }
        public int? status { get; set; }
        public string gender { get; set; }//Male,Female 
        //public string botkey { get; set; }
        public string doctorstatus { get; set; } 
        public int branchInfoId { get; set; } 
        
    }
}
