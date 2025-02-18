using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class ResponseBuilderService : IResponseBuilderService
    {
        private readonly ApplicationDbContext _context;
        public ResponseBuilderService( ApplicationDbContext applicationDbContext)
        {
            this._context = applicationDbContext;
        }


        public async Task<FacebookPage> GetGritingsMessgaeWithMenus(string PageId)
        {
            FacebookPage Data = null;
            try
            {
                Data = await _context.FacebookPages.FirstOrDefaultAsync(x => x.PageId == PageId);
            }catch(Exception e)
            {

                Console.WriteLine(e.Message);
            }

            return Data;
        }

        public Question GetQuestion(int MenuId, int QuesId)
        {
            Question data = null;
            try
            {
                data = _context.Questions.FromSql("EXEC SP_SelectQuestion {0},{1}", QuesId, MenuId).ToList().Single();

            }catch(Exception e)
            {
                Console.WriteLine("Error From Response builder service Get QUestion");
                Console.WriteLine(e.Message);
                data = new Question();
            }
            return data;
        }

        public Answer GetAnswer(int QuesId)
        {
            return _context.Answers.Where(x => x.QuestionId == QuesId).OrderBy(X => X.Id).FirstOrDefault();

        }

        public IEnumerable<Menu> GetMenus(int ParentId, int PageId)
        {
            return  _context.Menus.Where(x=> x.ParrentId == ParentId && x.FacebookPageId == PageId).ToList();
        }

        public int GetParrentId(int CurrentId)
        {
            return _context.Menus.Find(CurrentId).ParrentId;
        }
    }
}
