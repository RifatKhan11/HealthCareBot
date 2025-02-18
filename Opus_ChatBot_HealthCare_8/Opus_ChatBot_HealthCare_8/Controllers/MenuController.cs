using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{

    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService menuService;
        private readonly IFacebookService facebookService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBotService _botService;
        private int Depth;
        public MenuController(IMenuService menuService, IBotService _botService, IFacebookService facebookService, UserManager<ApplicationUser> userManager, IQuestionReplayService questionReplayService)
        {
            this.menuService = menuService;
            this._botService = _botService;
            this.facebookService = facebookService;
            this.questionReplayService = questionReplayService;
            _userManager = userManager;
            Depth = 0;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var chatbotInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            MenuViewModel model = new MenuViewModel
            {
                FbPageId = FbPageId,
                menuQuestionAnswers = new List<MenuQuestionAnswer>(),
                //menuQuestionAnswers = await questionReplayService.GetAllQuestionWithMenuAnser(FbPageId)
                AllMenus = await _botService.GetAllMenusByBotKey(chatbotInfo.botKey)
            };
            if (model.AllMenus == null)
            {
                model.menuQuestionAnswers = new List<MenuQuestionAnswer>();
                model.AllMenus = new List<Menu>();

            }
            TempData["message"] = "message";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MenuViewModel _menu)
        {
            //return Json(_menu);

            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            if (!ModelState.IsValid)
            {
                MenuViewModel model = new MenuViewModel
                {
                    FbPageId = FbPageId
                };

                return View(model);
            }

            _menu.MenuName = DataFilter.FilterUserString(_menu.MenuName);
            _menu.MenuNameEN = DataFilter.FilterUserString(_menu.MenuNameEN);
            _menu.menuType = DataFilter.FilterUserString(_menu.menuType);
            _menu.responseAPI = _menu.responseAPI;

            string message = "success";

            if (_menu.SubmitType == "Save Menu")
            {
                if (!await _botService.SaveMenu(_menu, FbPageId, _menu.botKey)) message = "Invalid node for this action";
            }
            else if (_menu.SubmitType == "Rename")
            {
                if (!_botService.RenameMenu(_menu, _menu.botKey)) message = "Invalid node for this action";
            }
            else if (_menu.SubmitType == "Make It as Last")
            {
                message = menuService.MakeLastNode(_menu.ParrentMenuId);
            }
            else if (_menu.SubmitType == "Delete This Menu")
            {
                message = menuService.DeleteMenu(_menu.ParrentMenuId);
            }
            else if (_menu.SubmitType == "Make It as General")
            {
                message = menuService.MakeGeneralNode(_menu.ParrentMenuId);
            }

            if (message != "success")
            {
                MenuViewModel model = new MenuViewModel
                {
                    FbPageId = FbPageId
                };

                Errors.AddErrorToModelState("status", message, ModelState);
                model.menuQuestionAnswers = await questionReplayService.GetAllQuestionWithMenuAnser(FbPageId);
                return View(model);
            }


            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMenusJson(int FbPageId, string botKey)
        {
            Depth = 2;
            string s = "[{" + string.Format("\"data\":{0},\"name\":\"{1}\",\"nameEN\":\"{2}\",\"parent\":\"{3}\",\"type\":\"{4}\",\"children\":[{5}]", 0, "Start", "StartEN", "null", "parrent", await this.GenerateTree(0, "Start", FbPageId, 0, botKey)) + "}]";

            dynamic data = new JObject();
            data.menus = s;
            data.depth = Depth;
            return Json(data);
        }

        //Recursion For Retriving Tree 
        private async Task<string> GenerateTree(int parrentid, string parrentName, int facebookPageId, int level, string botKey)
        {
            Depth = Math.Max(level, Depth);
            string data = "";
            IEnumerable<Menu> MenuData = await _botService.GetMenusByBotKey(parrentid, facebookPageId, botKey);

            if (MenuData.Count() <= 0)
            {
                return data;
            }

            int last = MenuData.Last().Id;
            foreach (Menu menu in MenuData)
            {
                string type = "parrent";

                if (menu.IsLast) { type = "last"; }

                string child = await GenerateTree(menu.Id, menu.MenuName, facebookPageId, level + 1, botKey);

                string S = "{" + string.Format("\"data\":{0},\"name\":\"{1}\",\"nameEN\":\"{2}\",\"parent\":\"{3}\",\"type\":\"{4}\",\"children\":[{5}]", menu.Id, menu.MenuName, menu.MenuNameEN, parrentid, type, child) + "}";

                if (menu.Id != last)
                {
                    S += ",";
                }
                data += S;
            }

            return data;
        }


        public async Task<IActionResult> GetMenuReaderByMenuId(int menuId)
        {
            var data = await _botService.GetMenuReaderByMenuId(menuId);

            return Json(data);
        }

        public async Task<IActionResult> SaveMenuReader(SaveMenuReaderViewModel model)
        {
            var data = new MenuReader
            {
                Id = model.Id,
                message = model.questionEn,
                stepNo = model.stepNo,
                menuId = model.parentNodeEn,
                Type = "Question",
                parameterName = model.parameterName
            };

            await _botService.SaveMenuReader(data);

            return Json("ok");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMenuReader(int id)
        {
            var data = await _botService.DeleteMenuReader(id);

            return Json(data);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPreMessageByWrapperDetailsId(int id, string userId, string botKey)
        {
            var user = await _botService.GetChatBotInfoByBotKey(botKey);

            var conInfo = new Models.BotModels.ConnectionInfo
            {
                userId = userId,
                wrapperDetailsId = id,
                entryDate = DateTime.Now,
                branchInfoId = user.ApplicationUser?.branchId
            };

            await _botService.SaveConnectionInfo(conInfo);

            var data = await _botService.GetPreMessageByWrapperDetailsId(id);

            return Json(data);
        }
    }
}