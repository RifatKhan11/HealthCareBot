using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IFacebookService
    {
        Task SendMessageToFacebook(string userId, List<string> messages, string PageAccessToken);
        Task SendTypingResponse(string userId, string PageAccessToken);
        Task SendSeenResponse(string userId, string PageAccessToken);
        Task<string> GetUserName(string userId, string PageAccessToken);
        Task<string> GetAccessToken(string PageId);
        Task<string> GetAccessTokenById(int Id);
        Task<int> GetFacebookpageId(string PageId);
        Task<bool> SaveNewPage(FacebookPage model);
        Task<int> GetPageIdByLoggedInUserId(string ApplicationUserId);
        Task<FacebookPage> GetFacebookPageById(int PageId);
        Task<bool> UpdatePageGreetingsMessage(int PageId, string GreetingsMessage, string GreetingsMessageEN);
        Task<bool> SaveChatBotInfo(ChatbotInfo model);
        Task<IEnumerable<ChatbotInfo>> GetAllBots();
        Task<string> GetScriptById(int id);
        Task<ChatbotInfo> botKeyInfo(string botkey);
    }
}
