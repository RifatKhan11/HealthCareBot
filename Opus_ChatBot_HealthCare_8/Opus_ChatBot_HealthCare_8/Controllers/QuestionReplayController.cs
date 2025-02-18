using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [AllowAnonymous]
    public class QuestionReplayController : Controller
    {
        private readonly IMenuService menuService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly IFacebookService facebookService;
        private readonly IKnowledgeService knowledgeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionReplayController(IMenuService menuService, IQuestionReplayService questionReplayService, IFacebookService facebookService, UserManager<ApplicationUser> userManager, IKnowledgeService knowledgeService)
        {
            this.menuService = menuService;
            this.questionReplayService = questionReplayService;
            this.facebookService = facebookService;
            this.knowledgeService = knowledgeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User)); ;
            QuestionReplayViewModel model = new QuestionReplayViewModel
            {
                menuQuestionAnswers = await questionReplayService.GetAllQuestionWithMenuAnser(fbPageId)
            };
            return View(model);
        }

        //This Method is for Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(QuestionReplayViewModel _model)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User)); ;
            if (!ModelState.IsValid)
            {
                QuestionReplayViewModel model = new QuestionReplayViewModel
                {
                    menuQuestionAnswers = await questionReplayService.GetAllQuestionWithMenuAnser(fbPageId)
                };
                return View(model);
            }

            //_model.AnswereText = DataFilter.FilterUserString(_model.AnswereText);
            //_model.QuestionText = DataFilter.FilterUserString(_model.QuestionText);

            questionReplayService.UpdateQuesReplay(_model);

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Delete(QuestionReplayViewModel model)
        {
            if (model.AnswerId != 0 && model.QuestionId != 0)
                questionReplayService.DeleteQuesRelay(model);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Create()
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            QuestionReplayViewModel model = new QuestionReplayViewModel
            {
                FbPageId = fbPageId,
                AnswerTypes = questionReplayService.AnswerTypesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionReplayViewModel _model)
        {

            //return Json(_model);

            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User)); ;
            //if (!ModelState.IsValid)
            //{
            //    QuestionReplayViewModel model = new QuestionReplayViewModel
            //    {
            //        FbPageId = fbPageId,
            //        AnswerTypes = questionReplayService.AnswerTypesAsync()
            //    };
            //    return View(model);
            //}

            if (_model.AnswerId == null || _model.AnswerId == 0)
            {
                if (_model.AnswereText != null && _model.AnswereTextEN != null)
                {
                    _model.AnswereText = DataFilter.FilterUserString(_model.AnswereText);
                    _model.AnswereTextEN = DataFilter.FilterUserString(_model.AnswereTextEN);
                }

                if (_model.AnswerTypeId == 1)
                {
                    KeyWordQuesAns mdl = new KeyWordQuesAns
                    {
                        facebookPageId = fbPageId,
                        question = _model.QuestionTextEN,
                        answer = _model.AnswereTextEN
                    };

                    await knowledgeService.SaveKnowledge(mdl);

                    questionReplayService.SaveNewQuesReplay(_model);
                }
                else if (_model.AnswerTypeId == 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (_model.TextWithButton[i].Title != null) _model.TextWithButton[i].Title = DataFilter.FilterUserString(_model.TextWithButton[i].Title);
                        if (_model.TextWithButton[i].TitleEN != null) _model.TextWithButton[i].TitleEN = DataFilter.FilterUserString(_model.TextWithButton[i].TitleEN);

                    }
                    questionReplayService.SaveNewTextWithButton(_model);
                }
                else if (_model.AnswerTypeId == 3)
                {
                    for (int i = 0; i < _model.GenericTemplates.Count; i++)
                    {
                        if (_model.GenericTemplates[i].title != null) _model.GenericTemplates[i].title = DataFilter.FilterUserString(_model.GenericTemplates[i].title);
                        if (_model.GenericTemplates[i].titleEN != null) _model.GenericTemplates[i].titleEN = DataFilter.FilterUserString(_model.GenericTemplates[i].titleEN);
                        if (_model.GenericTemplates[i].subtitle != null) _model.GenericTemplates[i].subtitle = DataFilter.FilterUserString(_model.GenericTemplates[i].subtitle);
                        if (_model.GenericTemplates[i].subtitleEN != null) _model.GenericTemplates[i].subtitleEN = DataFilter.FilterUserString(_model.GenericTemplates[i].subtitleEN);

                        for (int n = 0; n < 3; n++)
                        {
                            if (_model.GenericTemplates[i].buttons[n].Title != null) _model.GenericTemplates[i].buttons[n].Title = DataFilter.FilterUserString(_model.GenericTemplates[i].buttons[n].Title);
                            if (_model.GenericTemplates[i].buttons[n].TitleEN != null) _model.GenericTemplates[i].buttons[n].TitleEN = DataFilter.FilterUserString(_model.GenericTemplates[i].buttons[n].TitleEN);
                        }
                    }
                    questionReplayService.SaveNewCrousal(_model);
                }
            }
            else
            {
                questionReplayService.UpdateQuesReplay(_model);
            }

            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        [Route("global/api/getmenuquestion/{Id}")]
        [HttpGet]

        public async Task<IActionResult> getmenuquestion(int Id)
        {
          //  int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalCountViewModel> model = await policeDashBoardService.TotalCountViewModelsDateRange(fromDate, toDate);
            var model = await questionReplayService.Getanswerbymenuid(Id);

            return Json(model);
        }
    }
}