using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IQuestionReplayService
    {
        bool SaveNewQuesReplay(QuestionReplayViewModel model);
        bool SaveNewTextWithButton(QuestionReplayViewModel model);
        bool SaveNewCrousal(QuestionReplayViewModel model);
        bool DeleteQuesRelay(QuestionReplayViewModel model);
        Task<int> GetAllQUesCountByFbPageID(int fbPageId);

        Task<IEnumerable<MenuQuestionAnswer>> GetAllQuestionWithMenuAnser(int fbPageId);
        IEnumerable<AnswerType> AnswerTypesAsync();
        bool UpdateQuesReplay(QuestionReplayViewModel model);
        Task<Question> GetquestionbymenuID(int Id);
        Task<Answer> GetanswerbyqID(int Id);
        Task<Answer> Getanswerbymenuid(int Id);
    }
}
