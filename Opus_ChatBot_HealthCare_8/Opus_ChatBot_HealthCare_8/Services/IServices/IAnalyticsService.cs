using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IAnalyticsService
    {
        Task<bool> SaveAnalytics(Analytics analytics);
        Task<int> TodaysNewUserByPageId(string pageId);
        Task<int> TodaysRepeatedUserByPageId(string pageId);
    }
}
