using Microsoft.AspNetCore.Http;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DepartmentVM
    {
        public int Id { get; set; }
        public string departmentCode { get; set; }
        public string departmentName { get; set; }
        public string shortName { get; set; }
        public string thumbUrl { get; set; }
        public int status { get; set; } = 1;
        public string Departmentstatus { get; set; }
        public string location { get; set; }
        public string botkey { get; set; }
        public string fileName { get; set; }
        public int branchId { get; set; }
        public IFormFile fileUrl { get; set; }
        public IEnumerable<DepartmentInfo> departmentInfos { get; set; }
    }
}
