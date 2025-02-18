using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface ILanguageService
    {
        string MyLanguage(string PageId, string UserId);
        Task<bool> SaveMyLanguage(string PageId, string UserId, string Lang);

    }
}
