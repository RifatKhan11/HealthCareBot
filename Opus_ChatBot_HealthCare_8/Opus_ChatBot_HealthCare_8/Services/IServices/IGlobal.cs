using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using System.Collections.Generic;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IGlobal
    {
        List<OnlineUserVm> GetOnlineUsers();
        void AddUser(string userName);
    }
}
