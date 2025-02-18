using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Route("[controller]/[action]")]
    public class MasterDataController : Controller
    {
       
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IquestionCategoryService questionCategoryService;
        private readonly IFacebookService facebookService;
        private readonly IPassportInfoService passportInfoService;

        public MasterDataController(IquestionCategoryService questionCategoryService, IPassportInfoService passportInfoService, UserManager<ApplicationUser> userManager, IFacebookService facebookService)
        {
         
            this.questionCategoryService = questionCategoryService;
            this.facebookService = facebookService;
            this.passportInfoService = passportInfoService;
            _userManager = userManager;
        }


        // GET: Knowledge
        // GET: Knowledge/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            QuestionCategoryViewModel model = new QuestionCategoryViewModel
            {
               
                questionCategories=await questionCategoryService.GetAllquestionCAtegorybyFacebookId(FbPageId)
            };

            return View(model);
        }

        // POST: Knowledge/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(QuestionCategoryViewModel model)
        {



            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            questionCategory keyWordQuesAns = new questionCategory // New Item To Save.
            {
                Id = model.Id,
                categoryName = model.questioncategoryName,
                facebookPageId= FbPageId

            };

            await questionCategoryService.SaveCategory(keyWordQuesAns); // Saving data In Here.

            return RedirectToAction(nameof(Index));
        }

        
        

        [HttpPost]
        public async Task<IActionResult> Delete(KnowledgeViewModel model)
        {
            if (model.Id != 0)
                await questionCategoryService.DeleteCategoryById(model.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> IndexOperator()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            OperatorViewModel model = new OperatorViewModel
            {
                operators = await questionCategoryService.GetAllOperator()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ComplainList()
        {
            OperatorViewModel model = new OperatorViewModel
            {
                complainSuggestions = await questionCategoryService.GetAllComplainSuggestionByType(1)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SuggestionList()
        {
            OperatorViewModel model = new OperatorViewModel
            {
                complainSuggestions = await questionCategoryService.GetAllComplainSuggestionByType(2)
            };

            return View(model);
        }


        // POST: Knowledge/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexOperator(OperatorViewModel model)
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            Operator keyWordQuesAns = new Operator // New Item To Save.
            {
                Id = model.Id,
                operatorName = model.questioncategoryName,

            };

            await questionCategoryService.SaveOperator(keyWordQuesAns); // Saving data In Here.

            return RedirectToAction(nameof(IndexOperator));
        }




        [HttpPost]
        public async Task<IActionResult> DeleteOperator(OperatorViewModel model)
        {
            if (model.Id != 0) {
                Operator data = await questionCategoryService.GetOperatorById(model.Id);
                data.entryby = "Inctive";
                await questionCategoryService.SaveOperator(data);
            }
            return RedirectToAction(nameof(IndexOperator));
        }

        [HttpGet]
        public async Task<JsonResult> IndexGrettingFirstJson()
        {
            var data = await questionCategoryService.GetAllFirstGrettings();

            return Json(data.LastOrDefault().NameEn);
        }

        [HttpGet]
        public async Task<JsonResult> GreetingsMessage(string botKey)
        {
            try
            {
                var data = await questionCategoryService.GetGreetingsMessage(botKey);

                if (data.Count() > 0)
                {
                    return Json(data.FirstOrDefault().NameEn);
                }
                else
                {
                    return Json("Welcome");
                }
            }
            catch (Exception ex)
            {
                return Json("Welcome");
            }
        }

        [HttpGet]
        public async Task<IActionResult> IndexGrettingFirst()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            OperatorViewModel model = new OperatorViewModel
            {
                lastGrettings = await questionCategoryService.GetAllFirstGrettings()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> IndexGretting()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            OperatorViewModel model = new OperatorViewModel
            {
                lastGrettings = await questionCategoryService.GetAllLastGrettings()
            };

            return View(model);
        }

        // POST: Knowledge/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexGretting(OperatorViewModel model)
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            LastGrettings keyWordQuesAns = new LastGrettings // New Item To Save.
            {
                Id = model.Id,
                NameBn = model.NameBn,
                NameEn = model.NameEn,
                entryby="Last"
            };

            await questionCategoryService.SaveLastGrettings(keyWordQuesAns); // Saving data In Here.

            return RedirectToAction(nameof(IndexGretting));
        }


        // POST: Knowledge/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexGrettingFirst(OperatorViewModel model)
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            LastGrettings keyWordQuesAns = new LastGrettings // New Item To Save.
            {
                Id = model.Id,
                NameBn = model.NameBn,
                NameEn = model.NameEn,
                entryby = "First"
            };

            await questionCategoryService.SaveLastGrettings(keyWordQuesAns); // Saving data In Here.

            return RedirectToAction(nameof(IndexGrettingFirst));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGretting(OperatorViewModel model)
        {
            if (model.Id != 0)
                await questionCategoryService.DeleteLastGrettingsById(model.Id);
            return RedirectToAction(nameof(IndexGretting));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FirstOperator()
        {
            var operators = await questionCategoryService.GetAllOperator();
            var rand = new Random();
            var data = operators.OrderBy(x => rand.Next()).FirstOrDefault();

            //var data = await questionCategoryService.GetOperatorFirst();
            //OperatorAssign operatorAssign = new OperatorAssign
            //{
            //    OperatorId = data.Id,
            //};
            //await questionCategoryService.SaveOperatorAssign(operatorAssign);
            return Json(data.operatorName);
        }

        [HttpGet("{complain}/{passport}")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveComplain(string complain,string passport)
        {
           var data = await passportInfoService.GetPassportInfoByPasspoertid(Convert.ToInt32(passport));
            ComplainSuggestion complainSuggestion = new ComplainSuggestion
            {
                text=complain,
                type =1,
                passportNumber= data.passportNo,
                dateTime=DateTime.Now
            };
            await questionCategoryService.SaveComplainSuggestion(complainSuggestion);
            return Json(1);
        }
        [HttpGet("{complain}/{passport}")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveSuggestion(string complain, string passport)
        {
            // var data = await questionCategoryService.GetOperatorFirst();
            var data = await passportInfoService.GetPassportInfoByPasspoertid(Convert.ToInt32(passport));
            ComplainSuggestion complainSuggestion = new ComplainSuggestion
            {
                text = complain,
                type = 2,
                passportNumber = data.passportNo,
                dateTime=DateTime.Now
            };
            await questionCategoryService.SaveComplainSuggestion(complainSuggestion);
            return Json(1);
        }

    }
}