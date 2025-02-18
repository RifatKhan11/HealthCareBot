using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IKeyWordQuesService
    {
        Task<KeyWordQuesAns>GetKeyWordQuesAnsByMessageAndFbPageId(int pagrId, string message, string combinedId, string userFbId);
        Task<KeyWordQuesAns>GetKeyWordQuesAnsByMessageAndFbPageId(int id);
        Task<KeyWordQuesAns>GetNextKeyWordQuesAnsByCOmbinedID(string combinedId);
        Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageIdS(int pagrId, string message, string combinedId, string userFbId);
        Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageIdSS(int pagrId, string message, string combinedId, string userFbId);
        Task<KeyWordQuesAns> GetNextKeyWordQuesAnsByCOmbinedIDcatid(int catId, int quesid,string answer);
        Task<KeyWordQuesAns> GetNextKeyWordQuesAnsByCOmbinedIDNewcatid(int quesid, string answer);
        Task<bool> Saveunknownquestion(UnKnownKeyWordQuestion unKnownKeyWordQuestion);
        Task<UnKnownKeyWordQuestion> getunknowquestion();
        int GetKeyWordQuesAnsByMessageAndFbPageIdcount(int pagrId, string message, string combinedId, string userFbId);
        Task<UnKnownKeyWordQuestion> getunknowquestionbyuserid(string userId);
        Task<int> SavekeyWordQuesAns(KeyWordQuesAns model);
        Task<int> SaveBotKnowledges(BotKnowledge model);
        Task<int> SaveServiceFlows(ServiceFlow model);
    }
}
