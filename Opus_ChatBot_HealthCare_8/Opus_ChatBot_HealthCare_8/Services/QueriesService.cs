using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class QueriesService : IQueriesService
    {
        private readonly ApplicationDbContext _contex;
        private readonly IDapper _dapper;
        public QueriesService(ApplicationDbContext contex, IDapper dapper)
        {
            _contex = contex;
            _dapper = dapper;
        }
        public async Task<IEnumerable<QueriesDataViewModel>> GetQueriesDataViewModels(int fbid)
        {
            var data = await _dapper.FromSqlAsync<QueriesDataViewModel>($"getqueries {fbid}");
            return data;
        }
        public async Task<IEnumerable<Queries>> GetAllQueries(int fbPageId)
        {
            return await  _contex.Queries.Where(x => x.FacebookPageId == fbPageId && x.FbUserID=="").AsNoTracking().ToListAsync();
        }

        public async Task<Queries> GetQueries(int Id)
        {
            try
            {
                Queries queries = await _contex.Queries.FindAsync(Id);
                return queries;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            throw new NotImplementedException();
        }

        public async Task<int> GetQueriesCount(int fbPageId)
        {
            return await _contex.Queries.CountAsync(x => x.FacebookPageId == fbPageId);
        }

        public async Task<bool> SaveUserQueries(UserQueriesViewModel model)
        {
            try
            {
                Queries queries = new Queries
                {
                    FacebookPageId = model.fbPageId,
                    FbUserName = model.userName,
                    FbUserID = model.userId,
                    QueriesText = model.userQuestion,
                    entryby="",
                    entryDate=DateTime.Now
                };

                _contex.Queries.Add(queries);

                return 1 == await _contex.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public async Task<bool> UpdateReplay(QueriesViewModel model)
        {
            try
            {
                Queries queries = await _contex.Queries.FindAsync(model.QueriesId);
                queries.AnswerText = model.AnswerText;
                return  1 == await _contex.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
