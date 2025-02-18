using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class questionCategoryService : IquestionCategoryService
    {
        private readonly ApplicationDbContext _contex;

        public questionCategoryService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> DeleteCategoryById(int id)
        {
            _contex.questionCategories.Remove(_contex.questionCategories.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<questionCategory>> GetAllquestionCAtegory()
        {
            return await _contex.questionCategories.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<questionCategory>> GetAllquestionCAtegorybyFacebookId(int facebookId)
        {
            return await _contex.questionCategories.Where(x => x.facebookPageId == facebookId).Include(x => x.facebookPage).AsNoTracking().ToListAsync();
        }
        public async Task<questionCategory> GetCategoryById(int id)
        {
            return await _contex.questionCategories.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<int> SaveCategory(questionCategory questionCategory)
        {
            if (questionCategory.Id != 0)
                _contex.questionCategories.Update(questionCategory);
            else
                _contex.questionCategories.Add(questionCategory);
            await _contex.SaveChangesAsync();

            return questionCategory.Id;
        }

        #region Operator

        public async Task<bool> DeleteOperatorById(int id)
        {
            _contex.operators.Remove(_contex.operators.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<Operator>> GetAllOperator()
        {
            return await _contex.operators.Where(x=>x.entryby=="Active").AsNoTracking().ToListAsync();
        }

        public async Task<Operator> GetOperatorById(int id)
        {
            return await _contex.operators.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Operator> GetOperatorFirst()
        {
            Operator data = new Operator();
            var dataassin = await _contex.operatorAssigns.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (dataassin != null)
            {
                data = await _contex.operators.Where(x => x.Id > dataassin.OperatorId).FirstOrDefaultAsync();
                if (data == null)
                {
                    data = await _contex.operators.FirstOrDefaultAsync();
                }
            }
            else
            {
              data=  await _contex.operators.FirstOrDefaultAsync();
            }
            return data; //await _contex.operators.FirstOrDefaultAsync();
        }

        public async Task<int> SaveOperator(Operator questionCategory)
        {
            if (questionCategory.Id != 0)
                _contex.operators.Update(questionCategory);
            else
                _contex.operators.Add(questionCategory);
            await _contex.SaveChangesAsync();

            return questionCategory.Id;
        }

        #endregion


        #region LastGrettings

        public async Task<bool> DeleteLastGrettingsById(int id)
        {
            _contex.lastGrettings.Remove(_contex.lastGrettings.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }
        public async Task<IEnumerable<LastGrettings>> GetAllFirstGrettings()
        {
            return await _contex.lastGrettings.Where(x => x.entryby == "First").AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<LastGrettings>> GetGreetingsMessage(string botKey)
        {
            return await _contex.lastGrettings.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<LastGrettings>> GetAllLastGrettings()
        {
            return await _contex.lastGrettings.Where(x=>x.entryby=="Last").AsNoTracking().ToListAsync();
        }
        public async Task<LastGrettings> GetLastGrettingsById(int id)
        {
            return await _contex.lastGrettings.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<LastGrettings> GetLastGrettingsleastone()
        {
            return await _contex.lastGrettings.AsNoTracking().OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveLastGrettings(LastGrettings questionCategory)
        {
            if (questionCategory.Id != 0)
                _contex.lastGrettings.Update(questionCategory);
            else
                _contex.lastGrettings.Add(questionCategory);
            await _contex.SaveChangesAsync();

            return questionCategory.Id;
        }

        #endregion


        #region OperatorAssign

        public async Task<bool> DeleteOperatorAssignById(int id)
        {
            _contex.operatorAssigns.Remove(_contex.operatorAssigns.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<OperatorAssign>> GetAllOperatorAssign()
        {
            return await _contex.operatorAssigns.AsNoTracking().ToListAsync();
        }
        public async Task<OperatorAssign> GetOperatorAssignById(int id)
        {
            return await _contex.operatorAssigns.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<int> SaveOperatorAssign(OperatorAssign questionCategory)
        {
            if (questionCategory.Id != 0)
                _contex.operatorAssigns.Update(questionCategory);
            else
                _contex.operatorAssigns.Add(questionCategory);
            await _contex.SaveChangesAsync();

            return questionCategory.Id;
        }

        #endregion


        #region OperatorAssign

        public async Task<bool> DeleteComplainSuggestionById(int id)
        {
            _contex.complainSuggestions.Remove(_contex.complainSuggestions.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<ComplainSuggestion>> GetAllComplainSuggestion()
        {
            return await _contex.complainSuggestions.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ComplainSuggestion>> GetAllComplainSuggestionByType(int type)
        {
            return await _contex.complainSuggestions.Where(x=>x.type ==type).AsNoTracking().ToListAsync();
        }

        public async Task<ComplainSuggestion> GetComplainSuggestionById(int id)
        {
            return await _contex.complainSuggestions.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<int> SaveComplainSuggestion(ComplainSuggestion questionCategory)
        {
            if (questionCategory.Id != 0)
                _contex.complainSuggestions.Update(questionCategory);
            else
                _contex.complainSuggestions.Add(questionCategory);
            await _contex.SaveChangesAsync();

            return questionCategory.Id;
        }

        #endregion



    }
}
