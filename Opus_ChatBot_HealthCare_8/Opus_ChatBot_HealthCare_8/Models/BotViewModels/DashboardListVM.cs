using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotViewModels
{
    public class DashboardListVM
    { 
        public int totalDoctors { get; set; }
        public int totalDept { get; set; }
        public int totalAppoint { get; set; }
        public int todayAppoint { get; set; }  
     }
    public class ApiListVM
    {
        public int totalDoctors { get; set; }
        public int totalDept { get; set; } 
        public int totalSpc { get; set; }
    }
}
