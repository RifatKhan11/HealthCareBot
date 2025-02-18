using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.IServices.IServices
{
    public interface ISMSService
    {
        Task<string> SendSMSAsync(string mobile, string message, string bpNo);
    }
}
