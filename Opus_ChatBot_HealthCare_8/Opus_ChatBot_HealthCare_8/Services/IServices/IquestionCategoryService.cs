using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IquestionCategoryService
    {
        Task<int> SaveCategory(questionCategory questionCategory);
        Task<IEnumerable<questionCategory>> GetAllquestionCAtegory();
        Task<questionCategory> GetCategoryById(int id);
        Task<bool> DeleteCategoryById(int id);
        Task<IEnumerable<questionCategory>> GetAllquestionCAtegorybyFacebookId(int facebookId);

        #region Operator
        Task<int> SaveOperator(Operator questionCategory);
        Task<IEnumerable<Operator>> GetAllOperator();
        Task<Operator> GetOperatorById(int id);
        Task<Operator> GetOperatorFirst();
        Task<bool> DeleteOperatorById(int id);
        #endregion

        #region LastGrettings
        Task<int> SaveLastGrettings(LastGrettings questionCategory);
        Task<IEnumerable<LastGrettings>> GetAllFirstGrettings();
        Task<IEnumerable<LastGrettings>> GetGreetingsMessage(string botKey);
        Task<IEnumerable<LastGrettings>> GetAllLastGrettings();
        Task<LastGrettings> GetLastGrettingsById(int id);
        Task<bool> DeleteLastGrettingsById(int id);
        Task<LastGrettings> GetLastGrettingsleastone();
        #endregion

        #region OperatorAssign
        Task<int> SaveOperatorAssign(OperatorAssign questionCategory);
        Task<IEnumerable<OperatorAssign>> GetAllOperatorAssign();
        Task<OperatorAssign> GetOperatorAssignById(int id);
        Task<bool> DeleteOperatorAssignById(int id);
        #endregion

        #region ComplainSuggestion
        Task<int> SaveComplainSuggestion(ComplainSuggestion questionCategory);
        Task<IEnumerable<ComplainSuggestion>> GetAllComplainSuggestion();
        Task<IEnumerable<ComplainSuggestion>> GetAllComplainSuggestionByType(int type);
        Task<ComplainSuggestion> GetComplainSuggestionById(int id);
        Task<bool> DeleteComplainSuggestionById(int id);
        #endregion
        


    }
}
