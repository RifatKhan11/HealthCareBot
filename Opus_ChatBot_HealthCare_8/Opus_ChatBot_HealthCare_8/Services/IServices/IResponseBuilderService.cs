using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IResponseBuilderService
    {
        Task<FacebookPage> GetGritingsMessgaeWithMenus(string PageId);
        Question GetQuestion(int MenuId, int QuesId);
        Answer GetAnswer(int QuesId);
        IEnumerable<Menu> GetMenus(int ParentId, int PageId);
        int GetParrentId(int CurrentId);
    }
}
