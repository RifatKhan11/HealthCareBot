using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IPoliceDashBoardService
    {
        Task<IEnumerable<TotalCountViewModel>> TotalCountViewModelsToday();
        Task<IEnumerable<TotalCountViewModel>> TotalCountViewModelsDateRange(DateTime FDate, DateTime TDate);
        Task<IEnumerable<TotalHitInfotViewModel>> TotalHitInfoViewModels(DateTime FDate, DateTime TDate, string Status);
        Task<IEnumerable<TotalHitInfotViewModel>> TotalHitInfoViewModelsWOD(string Status);
    }
}
