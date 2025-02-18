using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class DashboardViewModel
    {
        public int menus { get; set; }

        public int queries { get; set; }

        public int questions { get; set; }

        public int todaysNewUser { get; set; }

        public int todaysRepeatedUser { get; set; }

        public int FbPageId { get; set; }
        public int totalDoctors { get; set; }
        public int totalDept { get; set; }
        public int totalAppoint { get; set; }
        public int todayAppoint { get; set; }

        [Required]
        public string GreetingsMessage { get; set; }

        [Required]
        public string GreetingsMessageEN { get; set; }

        public IEnumerable<TotalCountViewModel> totalCountViewModels { get; set; }
        public IEnumerable<TotalHitInfotViewModel> totalSuccessHitInfotViewModels { get; set; }
        public IEnumerable<TotalHitInfotViewModel> totalUnSuccessHitInfotViewModels { get; set; }
        public IEnumerable<TotalHitMenuLogViewModel> menuHitLogs { get; set; }
        public IEnumerable<TotalHitKnowledgeLogViewModel> totalHitKnowledgeLogViewModels { get; set; }
        public IEnumerable<Queries> allqueries { get; set; }
        public IEnumerable<DashboardListVM> DashboardListVM { get; set; }
    }
}
