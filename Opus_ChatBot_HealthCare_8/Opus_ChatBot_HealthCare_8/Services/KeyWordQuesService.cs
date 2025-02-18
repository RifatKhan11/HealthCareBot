using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class KeyWordQuesService : IKeyWordQuesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserInfoService userInfoService;
        private readonly IFacebookService facebookService;
        private readonly IQueriesService queriesService;

        public KeyWordQuesService(ApplicationDbContext context, IUserInfoService userInfoService, IFacebookService facebookService, IQueriesService queriesService)
        {
            _context = context;
            this.userInfoService = userInfoService;
            this.facebookService = facebookService;
            this.queriesService = queriesService;
        }
        public  int GetKeyWordQuesAnsByMessageAndFbPageIdcount(int pagrId, string message, string combinedId, string userFbId)
        {
            char[] separator = { ' ' };
            string[] messages = message.Split(separator);
            List<int> QuestionsId = new List<int>();
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();

            try
            {
                foreach (string msg in messages)
                {

                    //IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.questionCategory.categoryName.ToLower().Contains(msg.ToLower())).ToListAsync();
                    IEnumerable<KeyWordQuesAns> temp =  _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.keyWord.ToLower().Contains(msg.ToLower())).ToList();

                    //List<int> data = temp.OrderBy(x => x.priority).Select(x => x.Id).ToList();
                    List<int> data = temp.OrderBy(x => x.Id).Select(x => x.Id).ToList();
                    QuestionsId.AddRange(data);
                }

               

            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n");
                Console.WriteLine(e.Message);
                Console.WriteLine("\n\n\n");
            }


            return QuestionsId.Count();
        }
        public async Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageId(int pagrId, string message, string combinedId, string userFbId)
        {
            List<string> keydatas = _context.keyWordQuesAns.Where(x=>x.keyWord!=null).Select(x => x.keyWord).ToList();
            List<string> keyarray=new List<string>();
            foreach (string x in keydatas)
            {
                string[] newmessage = x.Split(",");
                foreach (string text in newmessage)
                {
                    keyarray.Add(text.Trim().ToLower());
                }
               // keyarray.AddRange(newmessage.ToList());
            }
            char[] separator = { ' ' };
            string[] messages = message.Split(separator);
            List<int> QuestionsId = new List<int>();
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();
            
            try
            {
                foreach (string msg in messages)
                {
                    if(keyarray.Contains(msg.Trim().ToLower()))
                    {
                        //IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.questionCategory.categoryName.ToLower().Contains(msg.ToLower())).ToListAsync();
                        IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.keyWord.ToLower().Contains(msg.ToLower())).ToListAsync();

                        //List<int> data = temp.OrderBy(x => x.priority).Select(x => x.Id).ToList();
                        List<int> data = temp.OrderBy(x => x.Id).Select(x => x.Id).ToList();
                        QuestionsId.AddRange(data);
                    }
                    
                }

                List<int> maxRepeatedItems = new List<int>();

                if (QuestionsId.Count > 0)
                {
                    var grouped = QuestionsId.ToLookup(x => x);
                    var maxRepetitions = grouped.Max(x => x.Count());
                    maxRepeatedItems = grouped.Where(x => x.Count() == maxRepetitions).Select(x => x.Key).ToList();
                }



                if (maxRepeatedItems.Count > 0)
                {
                    keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.Id == maxRepeatedItems[0]).FirstOrDefaultAsync();
                    maxRepeatedItems.RemoveAt(0);
                    userInfoService.UpdateUserInfo(combinedId, "keyWordQues", string.Join<int>(";", maxRepeatedItems));
                }
                else
                {
                    string accessToken = await facebookService.GetAccessTokenById(pagrId);
                    string userName = await facebookService.GetUserName(userFbId, accessToken);

                    UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                    {
                        fbPageId = pagrId,
                        userId = userFbId,
                        userName = userName,
                        userQuestion = message
                    };
                    await queriesService.SaveUserQueries(userQueriesViewModel);
                    userInfoService.UpdateUserInfo(combinedId, "keyWordQues", "");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n");
                Console.WriteLine(e.Message);
                Console.WriteLine("\n\n\n");
            }


            return keyWordQuesAns;
        }
        public async Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageIdS(int pagrId, string message, string combinedId, string userFbId)
        {
            char[] separator = { ' ' };
            string[] messages = message.Split(separator);
            List<int> QuestionsId = new List<int>();
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();

            try
            {
                foreach (string msg in messages)
                {
                    //IEnumerable<KeyWordQuesAns> temp =  await _context.keyWordQuesAns.Include(x=>x.questionCategory).Where(x => x.facebookPageId == pagrId &&  x.question.ToLower().Contains(msg.ToLower())).ToListAsync();
                    IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.questionCategory.categoryName.ToLower() == msg.ToLower()).ToListAsync();
                    // IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == pagrId && x.questionCategoryId==2).ToListAsync();
                    //List<int> data = temp.Where(x => x.question.ToLower().Contains(msg.ToLower())).Select(x => x.Id).ToList(); // partial Matching

                    /* List<int> data = temp.Where(x => x.question.ToLower().Split().Contains(msg.ToLower())).Select(x => x.Id).ToList(); */// Strict Matchning.
                    List<int> data = temp.OrderBy(x => x.priority).Select(x => x.Id).ToList();
                    QuestionsId.AddRange(data);
                }

                List<int> maxRepeatedItems = new List<int>();

                if (QuestionsId.Count > 0)
                {
                    var grouped = QuestionsId.ToLookup(x => x);
                    var maxRepetitions = grouped.Max(x => x.Count());
                    maxRepeatedItems = grouped.Where(x => x.Count() == maxRepetitions).Select(x => x.Key).ToList();
                }



                if (maxRepeatedItems.Count > 0)
                {
                    keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.Id == maxRepeatedItems[0]).FirstOrDefaultAsync();
                    maxRepeatedItems.RemoveAt(0);
                    userInfoService.UpdateUserInfo(combinedId, "keyWordQues", string.Join<int>(";", maxRepeatedItems));
                }
                else
                {
                    string accessToken = await facebookService.GetAccessTokenById(pagrId);
                    string userName = await facebookService.GetUserName(userFbId, accessToken);

                    UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                    {
                        fbPageId = pagrId,
                        userId = userFbId,
                        userName = userName,
                        userQuestion = message
                    };
                    await queriesService.SaveUserQueries(userQueriesViewModel);
                    userInfoService.UpdateUserInfo(combinedId, "keyWordQues", "");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n");
                Console.WriteLine(e.Message);
                Console.WriteLine("\n\n\n");
            }


            return keyWordQuesAns;
        }
        public async Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageIdSS(int pagrId, string message, string combinedId, string userFbId)
        {


            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();
            keyWordQuesAns = _context.keyWordQuesAns.Where(x => x.facebookPageId == pagrId && x.question.Trim() == message).FirstOrDefault();
            return keyWordQuesAns;
        }

        public async Task<KeyWordQuesAns> GetKeyWordQuesAnsByMessageAndFbPageId(int id)
        {
            return await _context.keyWordQuesAns.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<KeyWordQuesAns> GetNextKeyWordQuesAnsByCOmbinedID(string combinedId)
        {

            UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();

            if (userInfo.keyWordQues != "")
            {
                List<string> splitData = userInfo.keyWordQues.Split(";").ToList();
                if (splitData.Count() > 0)
                {
                    keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.Id == Int32.Parse(splitData[0])).FirstOrDefaultAsync();
                    splitData.RemoveAt(0);
                    userInfoService.UpdateUserInfo(combinedId, "keyWordQues", string.Join<string>(";", splitData));
                }
            }
            return keyWordQuesAns;
        }
        public async Task<KeyWordQuesAns> GetNextKeyWordQuesAnsByCOmbinedIDcatid(int catId, int quesid, string answer)
        {
            if (answer == "হ্যাঁ")
            {
                answer = "yes";
            }
            if (answer == "না")
            {
                answer = "no";
            }
            var dat = await _context.keyWordQuesAns.Where(x => x.Id == quesid).FirstOrDefaultAsync();
            // UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();
            keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.keyWordQuesAnsId == quesid && x.questionCategoryId == catId && x.question.Trim().ToLower() == answer.Trim().ToLower()).FirstOrDefaultAsync();

            if (keyWordQuesAns == null)
            {
                var data = await _context.unKnownKeyWordQuestions.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
              
                if (data != null)
                {
                    string message = data.question;
                    char[] separator = { ' ' };
                    string[] messages = message.Split(separator);
                    List<int> QuestionsId = new List<int>();

                    foreach (string msg in messages)
                    {

                        //IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == dat.facebookPageId && x.questionCategory.categoryName.ToLower() == msg.ToLower()).ToListAsync();

                        //List<int> datas = temp.OrderBy(x => x.priority).Select(x => x.Id).ToList();
                        //QuestionsId.AddRange(datas);
                        IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == dat.facebookPageId && x.questionCategory.categoryName.ToLower().Contains(msg.ToLower())).ToListAsync();

                        List<int> datas = temp.OrderBy(x => x.priority).Select(x => x.Id).ToList();
                        QuestionsId.AddRange(datas);
                    }
                    List<int> maxRepeatedItems = new List<int>();

                    if (QuestionsId.Count > 0)
                    {
                        var grouped = QuestionsId.ToLookup(x => x);
                        var maxRepetitions = grouped.Max(x => x.Count());
                        maxRepeatedItems = grouped.Where(x => x.Count() == maxRepetitions).Select(x => x.Key).ToList();
                    }
                    if (maxRepeatedItems.Count > 0)
                    {
                        keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.priority >dat.priority && maxRepeatedItems.Contains(x.Id)).FirstOrDefaultAsync();
                       // maxRepeatedItems.RemoveAt(0);
                       // userInfoService.UpdateUserInfo("testid", "keyWordQues", string.Join<int>(";", maxRepeatedItems));
                    }
                    //if (QuestionsId.Count() > 0)
                    //{
                    //    int i = 0;
                    //    foreach (int id in QuestionsId)
                    //    {
                    //        if (id > quesid && i<=1)
                    //        {
                    //            keyWordQuesAns= await _context.keyWordQuesAns.Where(x => x.Id == id).FirstOrDefaultAsync();
                    //            i++;
                    //        }

                    //    }
                    //}


                }

            }


            return keyWordQuesAns;
        }
        #region unknownquestion
        public async Task<bool> Saveunknownquestion(UnKnownKeyWordQuestion unKnownKeyWordQuestion)
        {
            try
            {
                if (unKnownKeyWordQuestion.Id != 0)
                {
                    _context.unKnownKeyWordQuestions.Update(unKnownKeyWordQuestion);

                }
                else
                {
                    _context.unKnownKeyWordQuestions.Add(unKnownKeyWordQuestion);
                }

                await _context.SaveChangesAsync();
                //return passportInfo.Id;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UnKnownKeyWordQuestion> getunknowquestion()
        {
            var data = await _context.unKnownKeyWordQuestions.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return data;
        }
        public async Task<UnKnownKeyWordQuestion> getunknowquestionbyuserid(string userId)
        {
            var data = await _context.unKnownKeyWordQuestions.Where(x=>x.autoNumber==userId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return data;
        }
        #endregion
        public async Task<KeyWordQuesAns> GetNextKeyWordQuesAnsByCOmbinedIDNewcatid(int quesid, string answer)
        {
            if (answer == "হ্যাঁ")
            {
                answer = "yes";
            }
            if (answer == "না")
            {
                answer = "no";
            }
            var dat = await _context.keyWordQuesAns.Where(x => x.Id == quesid).FirstOrDefaultAsync();
            // UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns();
            keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.keyWordQuesAnsId == quesid && x.question.Trim().ToLower() == answer.Trim().ToLower()).FirstOrDefaultAsync();

            if (keyWordQuesAns == null)
            {
                var data = await _context.unKnownKeyWordQuestions.OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                if (data != null)
                {
                    List<string> keydatas = _context.keyWordQuesAns.Where(x => x.keyWord != null).Select(x => x.keyWord).ToList();
                    List<string> keyarray = new List<string>();
                    foreach (string x in keydatas)
                    {
                        string[] newmessage = x.Split(",");
                        foreach (string text in newmessage)
                        {
                            keyarray.Add(text.Trim().ToLower());
                        }
                        // keyarray.AddRange(newmessage.ToList());
                    }

                    string message = data.question;
                    char[] separator = { ' ' };
                    string[] messages = message.Split(separator);
                    List<int> QuestionsId = new List<int>();

                    foreach (string msg in messages)
                    {
                        if(msg!=" ")
                        {
                            if(keyarray.Contains(msg.Trim().ToLower()))
                            {
                                IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == dat.facebookPageId && x.keyWord.ToLower().Contains(msg.ToLower())).ToListAsync();

                                List<int> datas = temp.OrderBy(x => x.Id).Select(x => x.Id).ToList();
                                QuestionsId.AddRange(datas);
                            }
                            
                        }
                       
                        //IEnumerable<KeyWordQuesAns> temp = await _context.keyWordQuesAns.Include(x => x.questionCategory).Where(x => x.facebookPageId == dat.facebookPageId && x.questionCategory.categoryName.ToLower().Contains(msg.ToLower())).ToListAsync();
                       
                    }
                    List<int> maxRepeatedItems = new List<int>();

                    //if (QuestionsId.Count > 0)
                    //{
                    //    var grouped = QuestionsId.ToLookup(x => x);
                    //    var maxRepetitions = grouped.Max(x => x.Count());
                    //    maxRepeatedItems = grouped.Where(x => x.Count() == maxRepetitions).Select(x => x.Key).ToList();
                    //}
                    //if (maxRepeatedItems.Count > 0)
                    //{
                    //    keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.priority > dat.priority && maxRepeatedItems.Contains(x.Id)).FirstOrDefaultAsync();

                    //}
                    int i = 0;
                    foreach (int id in QuestionsId.Distinct())
                    {
                        if(i<1)
                        {
                            if (id > dat.Id)
                            {

                                i = 1;
                                keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.Id==id).FirstOrDefaultAsync();
                            }
                        }
                       
                       
                    }
                  


                }

            }


            return keyWordQuesAns;
        }
        #region-keyWordQuesAns
        public async Task<int> SavekeyWordQuesAns(KeyWordQuesAns model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.keyWordQuesAns.Update(model);
                }
                else
                {

                    _context.keyWordQuesAns.Add(model);
                }

                await _context.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception e)
            {
                throw e;

            }

        }
        public async Task<int> SaveBotKnowledges(BotKnowledge model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.BotKnowledges.Update(model);
                }
                else
                {

                    _context.BotKnowledges.Add(model);
                }

                await _context.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception e)
            {
                throw e;

            }

        }

        public async Task<int> SaveServiceFlows(ServiceFlow model)
        {
            try
            {
                if (model.Id != null)
                {

                    _context.ServiceFlows.Update(model);
                }
                else
                {

                    _context.ServiceFlows.Add(model);
                }

                await _context.SaveChangesAsync();
                return Convert.ToInt32(model.Id);
            }
            catch (Exception e)
            {
                throw e;

            }

        }
        #endregion

    }
}
