using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class PoliceDashBoardService : IPoliceDashBoardService
    {
        private readonly ApplicationDbContext _contex;
        private readonly IDapper _dapper;

        public PoliceDashBoardService(ApplicationDbContext contex, IDapper dapper)
        {
            _contex = contex;
            this._dapper = dapper;
        }


        public async Task<IEnumerable<TotalCountViewModel>> TotalCountViewModelsToday()
        {
            //return await _contex.totalCountViewModels.FromSql($"totalHitToday").AsNoTracking().ToListAsync();
            return await _dapper.FromSqlAsync<TotalCountViewModel>($"totalHitToday");

        }
        public async Task<IEnumerable<TotalCountViewModel>> TotalCountViewModelsDateRange(DateTime FDate, DateTime TDate)
        {
            //return await _contex.totalCountViewModels.FromSql($"totalHitTodaybyDate {Convert.ToDateTime(FDate).ToString("yyyyMMdd")},{Convert.ToDateTime(TDate).ToString("yyyyMMdd")}").AsNoTracking().ToListAsync();
            return await _dapper.FromSqlAsync<TotalCountViewModel>($"totalHitTodaybyDate '{Convert.ToDateTime(FDate).ToString("yyyyMMdd")}','{Convert.ToDateTime(TDate).ToString("yyyyMMdd")}'");
        }
        public async Task<IEnumerable<TotalHitInfotViewModel>> TotalHitInfoViewModels(DateTime FDate, DateTime TDate, string Status)
        {
            //return await _contex.totalHitInfotViewModels.FromSql($"totalHitTodaybyDateInfo {Convert.ToDateTime(FDate).ToString("yyyyMMdd")},{Convert.ToDateTime(TDate).ToString("yyyyMMdd")},{Status}").AsNoTracking().ToListAsync();
            return await _dapper.FromSqlAsync<TotalHitInfotViewModel>($"totalHitTodaybyDateInfo '{Convert.ToDateTime(FDate).ToString("yyyyMMdd")}','{Convert.ToDateTime(TDate).ToString("yyyyMMdd")}',{Status}");
        }
        public async Task<IEnumerable<TotalHitInfotViewModel>> TotalHitInfoViewModelsWOD(string Status)
        {
            //return await _contex.totalHitInfotViewModels.FromSql($"totalHitTodaybyDateInfoWOD {Status}").AsNoTracking().ToListAsync();
            return await _dapper.FromSqlAsync<TotalHitInfotViewModel>($"totalHitTodaybyDateInfoWOD {Status}");
        }


    }
}
