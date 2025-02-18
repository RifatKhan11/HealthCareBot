using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IKnowledgeService
    {
        Task<int> SaveKnowledge(KeyWordQuesAns leaveRegister);
        Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPageId(int fbPageId);
        Task<KeyWordQuesAns> GetKnowledgeById(int id);
        Task<bool> DeleteKnowledgeById(int id);
        Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPagebyword(string fbPageId);
        Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyPagebywordfbid(string value, int fbPageId);
        Task<IEnumerable<KeyWordQuesAns>> GetAllKnowledgebyCatId(int catId, int fbPageId);
        Task<bool> SaveKnowledgeHitLog(KnowledgeHitLog knowledgehitlog);
        Task<IEnumerable<TotalHitKnowledgeLogViewModel>> TotalHitKnowledgeLogViewModelsByDate(DateTime FDate, DateTime TDate, int fbid);
        Task<IEnumerable<TotalHitKnowledgeLogViewModel>> TotalHitKnowledgeLogWPD(int fbid);
        Task<IEnumerable<KeyWordQuesAns>> GetKeyWordQuesAns();
        Task<IEnumerable<BotRackInfoMaster>> GetBotMaster();
        Task<bool> KnowledgeDeleteById(int id);
        Task<IEnumerable<InputGroupMaster>> GroupMasters(string botKey);
        Task<IEnumerable<BotKnowledge>> BotKnowledge(string botKey);
        Task<int> SaveGroupMaster(InputGroupMaster inputGroup);
        Task<int> DeleteGroupMasterById(int id);
        Task<int> DeleteBotKnowledgeById(int id);
        Task<IEnumerable<InputGroupDetail>> GetInputGrpDetrailsByMasterId(int masId);
        Task<int> SaveGroupDetails(InputGroupDetail inputGroupDetail);
        Task<IEnumerable<InputGroupDetail>> GroupDetails(int masterId);
        Task<int> DeleteGroupDetailsById(int id);
        Task<int> SaveBotKnowledge(BotKnowledge botKnowledge);

    }
}
