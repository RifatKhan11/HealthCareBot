using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IBotFlowService
    {
        string GetCurrentFlowStatus(string CombinedId);
        string UpdateFlow(string CombinedId, string flowMessage);
        DateTime GetCurrentFlowStatusDateTime(string CombinedId);
        Task<ChatbotInfo> GetBotInfoByBotKey(string botKey);
        Task<IEnumerable<BotRackInfoDetail>> GetBotRackInfoByBotKey(string botKey);
        Task<List<WrapperHeaderImg>> GetWrapperHeaderImageList(string botKey);
    }
}
