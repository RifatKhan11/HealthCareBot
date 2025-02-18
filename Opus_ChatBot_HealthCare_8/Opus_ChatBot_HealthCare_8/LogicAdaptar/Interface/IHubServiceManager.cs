using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface
{
    public interface IHubServiceManager
    {
        Task<List<string>> CustoomVisaService(string senderId, string pageId, string message,string postback,string userId);
        Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack, string userId);
        Task<List<string>> CustoomVisaServiceBN(string senderId, string pageId, string message,string postback, string userId);
        Task<List<string>> CustomMessageGenerator(string senderId, string pageId, string message, string postback, string userId, string botKey);

    }
}
