using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _contex;

        public AnalyticsService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> SaveAnalytics(Analytics analytics)
        {
            _contex.Analytics.Add(analytics);
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<int> TodaysNewUserByPageId(string pageId)
        {
            return await _contex.Analytics.Where(x => x.DateTime.Date == DateTime.Now.Date).AsNoTracking().Select(x => x.SenderId).Distinct().CountAsync();
        }

        public async Task<int> TodaysRepeatedUserByPageId(string pageId)
        {
            List<string> data =  await _contex.Analytics.Where(x => x.DateTime.Date == DateTime.Now.Date).Select(x => x.SenderId).ToListAsync();

            return data.GroupBy(x => x).Where(x => x.Count() > 1).Count();
        }
    }
}
