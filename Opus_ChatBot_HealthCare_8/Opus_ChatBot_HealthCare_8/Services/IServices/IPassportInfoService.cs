using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IPassportInfoService
    {
        Task<PassportInfo> GetPassportInfoByPasspoertIds(string id);
        Task<bool> DeletePassportInfoById(int id);
        Task<int> SavePassportInfo(PassportInfo passportInfo);
        Task<IEnumerable<PassportInfo>> GetPassportInfo();
        Task<IEnumerable<ColumnHeading>> GetAllColumnBySp();
        Task<bool> GETApi(string data);
        Task<PassportInfo> GetPassportInfoByPasspoertid(int id);

        #region PoliceClearenceLog
        Task<int> SavePoliceClearenceLog(PoliceClearenceLog passportInfo);
        Task<IEnumerable<PoliceClearenceLog>> GetPoliceClearenceLog();
        #endregion
    }
}
