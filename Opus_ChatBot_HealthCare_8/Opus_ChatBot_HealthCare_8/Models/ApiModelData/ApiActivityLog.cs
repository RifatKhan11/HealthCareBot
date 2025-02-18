using System;

namespace Opus_ChatBot_HealthCare_8.Models.ApiModelData
{
    public class ApiActivityLog
    {
        public int Id { get; set; }
        public string type { get; set; } // pull, process
        public DateTime DateTime { get; set; }
        public string createBy { get; set; }
        public string uniqueKey { get; set; }

        public string jsonResponse { get; set; }
    }
}
