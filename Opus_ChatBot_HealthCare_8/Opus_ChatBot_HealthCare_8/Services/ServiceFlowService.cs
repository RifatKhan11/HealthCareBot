using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class ServiceFlowService : IServiceFlowService
    {
        private readonly ApplicationDbContext _contex;

        public ServiceFlowService(ApplicationDbContext contex)
        {
            _contex = contex;
        }
        public async Task<ServiceFlow> SaveServiceFlow(ServiceFlow serviceFlow)
        {
            try
            {
                _contex.ServiceFlows.Add(serviceFlow);

                await _contex.SaveChangesAsync();
                return serviceFlow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ServiceFlow CurrentServiceState(string CombinedId)
        {
            try
            {
                return _contex.ServiceFlows.Find(CombinedId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ServiceFlow();
            }
        }

        public string InitNewService(string CombinedId, string ServiceCode, string InfoType)
        {
            try
            {
                ServiceFlow serviceFlow = _contex.ServiceFlows.Find(CombinedId);
                if (serviceFlow == null)
                {
                    serviceFlow = new ServiceFlow
                    {
                        Id = CombinedId,
                        Attempt = 0,
                        DateTime = DateTime.Now,
                        ServiceCode = ServiceCode,
                        InfoType = InfoType,
                        StepNo = 0
                    };
                    _contex.ServiceFlows.Add(serviceFlow);
                }
                else
                {
                    serviceFlow.Attempt = 0;
                    serviceFlow.DateTime = DateTime.Now;
                    serviceFlow.ServiceCode = ServiceCode;
                    serviceFlow.InfoType = InfoType;
                    serviceFlow.StepNo = 0;
                }

                if (_contex.SaveChanges() == 1) return "success";
                return "fail";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something went wrong.";
            }
        }

        public string CLearServiceData(string CombinedId)
        {
            try
            {
                _contex.ServiceFlows.Remove(_contex.ServiceFlows.Find(CombinedId));
                _contex.SaveChanges();
                return "Ok";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "fail";
            }
        }

        public bool increaseAttempt(string CombinedId)
        {
            try
            {
                ServiceFlow serviceFlow = _contex.ServiceFlows.Find(CombinedId);
                if (serviceFlow != null)
                {
                    serviceFlow.Attempt++;
                    if (_contex.SaveChanges() == 1) return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool UpdateNextStep(string CombinedId, string ServiceCode, string InfoType, int stepNo)
        {
            try
            {
                ServiceFlow serviceFlow = _contex.ServiceFlows.Find(CombinedId);
                if(serviceFlow != null)
                {
                    serviceFlow.DateTime = DateTime.Now;
                    serviceFlow.Attempt = 0;
                    serviceFlow.InfoType = InfoType;
                    serviceFlow.ServiceCode = ServiceCode;
                    serviceFlow.StepNo = stepNo;
                }
                if (_contex.SaveChanges() == 1) return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
