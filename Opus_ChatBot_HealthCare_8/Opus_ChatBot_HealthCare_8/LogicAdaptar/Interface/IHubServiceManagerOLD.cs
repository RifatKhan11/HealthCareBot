using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface
{
    public interface IHubServiceManagerOld
    {
        Task<List<string>> CustoomVisaService(string senderId, string pageId, string message);
        Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack);
        //Task<List<string>> CustoomVisaServiceBN(string senderId, string pageId, string message);
    }
}
