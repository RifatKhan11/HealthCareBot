using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Response;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Authorize]
    public class QueriesController : Controller
    {
        private readonly IQueriesService queriesService;
        private readonly IFacebookService facebookService;
        private readonly IKnowledgeService knowledgeService;
        private readonly ILanguageService languageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string rootpath;

        public QueriesController(IWebHostEnvironment hostingEnvironment, IQueriesService queriesService, IFacebookService facebookService, UserManager<ApplicationUser> userManager, IKnowledgeService knowledgeService, ILanguageService languageService)
        {
            this.queriesService = queriesService;
            this.facebookService = facebookService;
            this.knowledgeService = knowledgeService;
            this.languageService = languageService;
            this.rootpath = hostingEnvironment.ContentRootPath;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? Id, DateTime? fromDate, DateTime? toDate)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            var allqueries = await queriesService.GetAllQueries(fbPageId);
            var data = await queriesService.GetQueriesDataViewModels(fbPageId);
            if (Id == 1)
            {
                allqueries = allqueries.Where(x => x.entryDate?.Date == DateTime.Now.Date);
                data = data.Where(x => x.entryDate?.Date == DateTime.Now.Date);
            }
            else if (Id == 2)
            {
                allqueries = allqueries.Where(x => x.entryDate?.Date <= DateTime.Now.Date);
                data = data.Where(x => x.entryDate?.Date <= DateTime.Now.Date);

            }
            else if (Id == 3)
            {
                allqueries = allqueries.Where(x => x.entryDate?.Date >= fromDate?.Date && x.entryDate?.Date <= toDate?.Date);
                data = data.Where(x => x.entryDate?.Date >= fromDate?.Date && x.entryDate?.Date <= toDate?.Date);

            }
            QueriesViewModel model = new QueriesViewModel
            {
                Queries = allqueries,
                queriesDataViewModels = data
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(QueriesViewModel _model)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            if (!ModelState.IsValid)
            {
                QueriesViewModel model = new QueriesViewModel
                {
                    Queries = await queriesService.GetAllQueries(fbPageId)
                };
                return View(model);
            }
            if (await queriesService.UpdateReplay(_model))
            {
                Queries queries = await queriesService.GetQueries(_model.QueriesId);

                List<string> Messages = new List<string>();
                List<quick_replies> quick_replies = new List<quick_replies>();

                quick_replies.Add(new quick_replies("text", "মেনু", payload: "start"));

                string response = "{ text:\"আপনার প্রশ্ন :) \\n " + queries.QueriesText + "\"}";

                Messages.Add(response);

                response = "{ text:\" => " + queries.AnswerText + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

                Messages.Add(response);

                Console.WriteLine("\n\n\nThe lengt is" + Messages.Count);

                await facebookService.SendMessageToFacebook(queries.FbUserID, Messages, await facebookService.GetAccessTokenById(queries.FacebookPageId));
                KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns
                {
                    facebookPageId = fbPageId,
                    question = queries.QueriesText,
                    answer = queries.AnswerText
                };
                await knowledgeService.SaveKnowledge(keyWordQuesAns);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UserQuery(string pageid, string userid)
        {
            var accessToken = await facebookService.GetAccessToken(pageid);



            string Lang = languageService.MyLanguage(pageid, userid);
            if (Lang == null || Lang == "") Lang = "BAN";
            UserQueriesViewModel model = new UserQueriesViewModel
            {

                fbPageId = await facebookService.GetFacebookpageId(pageid),
                userId = userid,
                userName = await facebookService.GetUserName(userid, accessToken),
            };

            if (Lang == "ENG")
            {
                using (StreamReader r = new StreamReader(rootpath + "/wwwroot/Lang/QuizeLanguageEN.json"))
                {
                    string json = r.ReadToEnd();
                    model.quizeLang = JsonConvert.DeserializeObject<QuizeLang>(json);
                }
            }
            else
            {
                using (StreamReader r = new StreamReader(rootpath + "/wwwroot/Lang/QuizeLanguageBN.json"))
                {
                    string json = r.ReadToEnd();
                    model.quizeLang = JsonConvert.DeserializeObject<QuizeLang>(json);
                }

            }


            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserQuery(UserQueriesViewModel _model)
        {
            if (!ModelState.IsValid)
            {
                return View(_model);
            }
            if (await queriesService.SaveUserQueries(_model))
            {
                List<string> Messages = new List<string>();
                List<quick_replies> quick_replies = new List<quick_replies>();

                quick_replies.Add(new quick_replies("text", "মেনু", payload: "start"));

                string response = "{ text:\"আপনার প্রশ্ন আমরা পেয়েছি। ধন্যবাদ :) \",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

                Messages.Add(response);
                await facebookService.SendMessageToFacebook(_model.userId, Messages, await facebookService.GetAccessTokenById(_model.fbPageId));

            }

            return RedirectToAction(nameof(UserQuerySuccess));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserQuerySuccess()
        {
            return View();
        }


    }
}