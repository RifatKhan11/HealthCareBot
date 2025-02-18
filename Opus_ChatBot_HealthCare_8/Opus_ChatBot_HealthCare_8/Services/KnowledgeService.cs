using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class KnowledgeService : IKnowledgeService
    {
        private readonly ApplicationDbContext _contex;
        private readonly IDapper _dapper;

        public KnowledgeService(ApplicationDbContext contex, IDapper dapper)
        {
            _contex = contex;
            this._dapper = dapper;
        }

        public async Task<bool> DeleteKnowledgeById(int id)
        {
            _contex.knowledgeHitLogs.RemoveRange(_contex.knowledgeHitLogs.Where(x => x.keyWordQuesAnsId == id).ToList());
            _contex.keyWordQuesAns.RemoveRange(_contex.keyWordQuesAns.Where(x => x.keyWordQuesAnsId == id).ToList());
            _contex.keyWordQuesAns.Remove(_contex.keyWordQuesAns.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPageId(int fbPageId)
        {
            return await _contex.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == fbPageId).AsNoTracking().OrderBy(x => x.priority).ToListAsync();
        }
        public async Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyCatId(int catId, int fbPageId)
        {
            return await _contex.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == fbPageId && x.questionCategoryId == catId).AsNoTracking().OrderBy(x => x.priority).ToListAsync();
        }

        public async Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPagebyword(string fbPageId)
        {
            return await _contex.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.question == fbPageId).AsNoTracking().OrderBy(x => x.priority).ToListAsync();
        }
        public async Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPagebywordfbid(string value, int fbPageId)
        {
            return await _contex.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.question.Contains(value) && x.facebookPageId == fbPageId).AsNoTracking().OrderBy(x => x.priority).ToListAsync();
        }

        public async Task<KeyWordQuesAns> GetKnowledgeById(int id)
        {
            return await _contex.keyWordQuesAns.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<int> SaveKnowledge(KeyWordQuesAns keyWordQuesAns)
        {
            if (keyWordQuesAns.Id != 0)
                _contex.keyWordQuesAns.Update(keyWordQuesAns);
            else
                _contex.keyWordQuesAns.Add(keyWordQuesAns);
            await _contex.SaveChangesAsync();
            _contex.Queries.RemoveRange(_contex.Queries.Where(x => x.QueriesText == keyWordQuesAns.question));
            return keyWordQuesAns.Id;
        }
        public async Task<IEnumerable<InputGroupMaster>> GroupMasters(string botKey)
        {
            return await _contex.InputGroupMasters.Include(x => x.menu).Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<BotKnowledge>> BotKnowledge(string botKey)
        {
            var data = await _contex.BotKnowledges.Where(x => x.botKey == botKey).Include(x => x.keyWordQuesAns).AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();

            return data;

        }
        public async Task<int> SaveGroupMaster(InputGroupMaster inputGroupMaster)
        {
            if (inputGroupMaster.Id != 0)
                _contex.InputGroupMasters.Update(inputGroupMaster);
            else
                _contex.InputGroupMasters.Add(inputGroupMaster);
            await _contex.SaveChangesAsync();
            return inputGroupMaster.Id;
        }
        public async Task<int> DeleteGroupMasterById(int id)
        {
            var data = await _contex.InputGroupMasters.FindAsync(id);

            _contex.InputGroupMasters.Remove(data);

            return await _contex.SaveChangesAsync();
        }
        public async Task<int> DeleteBotKnowledgeById(int id)
        {
            var data = await _contex.BotKnowledges.FindAsync(id);

            _contex.BotKnowledges.Remove(data);

            return await _contex.SaveChangesAsync();
        }

        public async Task<IEnumerable<InputGroupDetail>> GetInputGrpDetrailsByMasterId(int masId)
        {
            var data = await _contex.InputGroupDetails.Include(y => y.master).Where(y => y.masterId == masId).AsNoTracking().ToListAsync();
            return data;
        }

        public async Task<int> SaveGroupDetails(InputGroupDetail inputGroupDetail)
        {
            if (inputGroupDetail.Id != 0)
                _contex.InputGroupDetails.Update(inputGroupDetail);
            else
                _contex.InputGroupDetails.Add(inputGroupDetail);
            await _contex.SaveChangesAsync();
            return inputGroupDetail.Id;
        }
        public async Task<IEnumerable<InputGroupDetail>> GroupDetails(int masterId)
        {
            var data = await _contex.InputGroupDetails.AsNoTracking().Where(x => x.masterId == masterId).ToListAsync();
            return data;
        }
        public async Task<int> DeleteGroupDetailsById(int id)
        {
            var data = await _contex.InputGroupDetails.FindAsync(id);

            _contex.InputGroupDetails.Remove(data);

            return await _contex.SaveChangesAsync();
        }
        public async Task<int> SaveBotKnowledge(BotKnowledge botKnowledge)
        {
            if (botKnowledge.Id != 0)
                _contex.BotKnowledges.Update(botKnowledge);
            else
                _contex.BotKnowledges.Add(botKnowledge);
            await _contex.SaveChangesAsync();
            return botKnowledge.Id;
        }

        #region knowledgehitlog
        public async Task<bool> SaveKnowledgeHitLog(KnowledgeHitLog knowledgehitlog)
        {
            try
            {
                if (knowledgehitlog.Id != 0)
                {
                    _contex.knowledgeHitLogs.Update(knowledgehitlog);

                }
                else
                {
                    _contex.knowledgeHitLogs.Add(knowledgehitlog);
                }

                await _contex.SaveChangesAsync();
                //return passportInfo.Id;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TotalHitKnowledgeLogViewModel>> TotalHitKnowledgeLogViewModelsByDate(DateTime FDate, DateTime TDate, int fbid)
        {
            //var data = await _contex.totalHitKnowledgeLogViewModels.FromSql($"getknowledgehitlog {FDate},{TDate},{fbid}").AsNoTracking().ToListAsync();
            var data = await _dapper.FromSqlAsync<TotalHitKnowledgeLogViewModel>($"getknowledgehitlog '{FDate}','{TDate}',{fbid}");
            return data;
        }
        public async Task<IEnumerable<TotalHitKnowledgeLogViewModel>> TotalHitKnowledgeLogWPD(int fbid)
        {
            //var data = await _contex.totalHitKnowledgeLogViewModels.FromSql($"getknowledgehitlogWPD {fbid}").AsNoTracking().ToListAsync();
            var data = await _dapper.FromSqlAsync<TotalHitKnowledgeLogViewModel>($"getknowledgehitlogWPD {fbid}");
            return data;
        }

        public async Task<IEnumerable<KeyWordQuesAns>> GetKeyWordQuesAns()
        {
            var data = await _contex.keyWordQuesAns.AsNoTracking().ToListAsync();
            return data;
        }
        public async Task<IEnumerable<BotRackInfoMaster>> GetBotMaster()
        {
            var data = await _contex.botRackInfoMasters.AsNoTracking().ToListAsync();
            return data;
        }
        public async Task<bool> KnowledgeDeleteById(int id)
        {
            //_contex.knowledgeHitLogs.RemoveRange(_contex.knowledgeHitLogs.Where(x => x.keyWordQuesAnsId == id).ToList());
            _contex.keyWordQuesAns.RemoveRange(_contex.keyWordQuesAns.Where(x => x.keyWordQuesAnsId == id).ToList());
            _contex.keyWordQuesAns.Remove(_contex.keyWordQuesAns.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }
        #endregion
    }
}
