using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class GlobalService:IGlobal
    {
        private List<OnlineUserVm> onlineUsers = new List<OnlineUserVm>();

        public List<OnlineUserVm> GetOnlineUsers()
        {
            return onlineUsers.ToList();
        }

        public void AddUser(string userName)
        {
            var data = onlineUsers.FirstOrDefault();
        }
    }
}
