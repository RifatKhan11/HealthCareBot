using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IOTPService
    {
        Task<bool> GenerateAndSendOTP(string UsersMobile, string UserId, string PageId);
        Task<bool> VerifyOTP(string UsersId, string PageId, string otp);
        Task<bool> SendEMAIL(string mail, string subject, string message);
    }
}
