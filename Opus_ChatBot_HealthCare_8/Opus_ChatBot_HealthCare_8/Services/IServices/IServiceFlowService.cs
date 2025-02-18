using Opus_ChatBot_HealthCare_8.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IServiceFlowService
    {
        string CLearServiceData(string CombinedId);
        ServiceFlow CurrentServiceState(string CombinedId);
        string InitNewService(string CombinedId, string ServiceCode, string InfoType);
        bool increaseAttempt(string CombinedId);
        bool UpdateNextStep(string CombinedId, string ServiceCode, string InfoType, int stepNo);
        Task<ServiceFlow> SaveServiceFlow(ServiceFlow serviceFlow);
    }
}
