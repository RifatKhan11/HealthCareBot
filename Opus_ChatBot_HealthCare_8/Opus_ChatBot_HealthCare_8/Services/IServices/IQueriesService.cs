using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IQueriesService
    {
        Task<IEnumerable<Queries>> GetAllQueries(int fbPageId);

        Task<bool> UpdateReplay(QueriesViewModel model);

        Task<int> GetQueriesCount(int fbPageId);

        Task<bool> SaveUserQueries(UserQueriesViewModel model);

        Task<Queries> GetQueries(int Id);
   
        Task<IEnumerable<QueriesDataViewModel>> GetQueriesDataViewModels(int fbid);
    }
}
