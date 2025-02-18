using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.ApiModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFacebookService facebookService;
        private readonly IMenuService menuService;
        private readonly IQueriesService queriesService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly IAnalyticsService analyticsService;
        private readonly IPoliceDashBoardService policeDashBoardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IKnowledgeService knowledgeService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBotService _botService;
        private readonly IAPIFunction smsAPI;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        public HomeController(IFacebookService facebookService, ApplicationDbContext _context, IAPIFunction smsAPI, IKnowledgeService knowledgeService, UserManager<ApplicationUser> userManager, IPoliceDashBoardService policeDashBoardService, IQueriesService queriesService, IMenuService menuService, IQuestionReplayService questionReplayService, IAnalyticsService analyticsService, IAppointmentService appointmentService, IBotService _botService, IConfiguration _configuration)
        {
            this.queriesService = queriesService;
            this.menuService = menuService;
            this.facebookService = facebookService;
            this.questionReplayService = questionReplayService;
            this.analyticsService = analyticsService;
            this.policeDashBoardService = policeDashBoardService;
            this.knowledgeService = knowledgeService;
            _userManager = userManager;
            _appointmentService = appointmentService;
            this._botService = _botService;
            this.smsAPI = smsAPI;
            this._configuration = _configuration;
            this._context = _context;

        }



        [AllowAnonymous]
        public async Task<IActionResult> EvercareHome()
        {
            //int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            int fbPageId = 2;

            FacebookPage facebookPage = await facebookService.GetFacebookPageById(fbPageId);

            DashboardViewModel model = new DashboardViewModel
            {
                menus = await menuService.GetMenuCount(fbPageId),
                queries = await queriesService.GetQueriesCount(fbPageId),
                questions = await questionReplayService.GetAllQUesCountByFbPageID(fbPageId),
                FbPageId = fbPageId,
                todaysNewUser = await analyticsService.TodaysNewUserByPageId(facebookPage.PageId),
                todaysRepeatedUser = await analyticsService.TodaysRepeatedUserByPageId(facebookPage.PageId),
                GreetingsMessage = facebookPage.PageGreetingMessage,
                GreetingsMessageEN = facebookPage.PageGreetingMessageEN,
                totalCountViewModels = await policeDashBoardService.TotalCountViewModelsToday(),
                //totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Successfully"),
                totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModelsWOD("Successfully"),
                totalUnSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Unsuccessfully"),
                //menuHitLogs = await menuService.GetMenuHitLogByDate(DateTime.Now, DateTime.Now, fbPageId),
                menuHitLogs = await menuService.GetMenuHitLogByDateWOD(fbPageId),
                //totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(DateTime.Now, DateTime.Now, fbPageId),
                totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId),
                allqueries = await queriesService.GetAllQueries(fbPageId)

            };
            return View(model);
        }



        [AllowAnonymous]
        public async Task<IActionResult> EvercareHomeCTG()
        {
            //int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            int fbPageId = 2;

            FacebookPage facebookPage = await facebookService.GetFacebookPageById(fbPageId);

            DashboardViewModel model = new DashboardViewModel
            {
                menus = await menuService.GetMenuCount(fbPageId),
                queries = await queriesService.GetQueriesCount(fbPageId),
                questions = await questionReplayService.GetAllQUesCountByFbPageID(fbPageId),
                FbPageId = fbPageId,
                todaysNewUser = await analyticsService.TodaysNewUserByPageId(facebookPage.PageId),
                todaysRepeatedUser = await analyticsService.TodaysRepeatedUserByPageId(facebookPage.PageId),
                GreetingsMessage = facebookPage.PageGreetingMessage,
                GreetingsMessageEN = facebookPage.PageGreetingMessageEN,
                totalCountViewModels = await policeDashBoardService.TotalCountViewModelsToday(),
                //totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Successfully"),
                totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModelsWOD("Successfully"),
                totalUnSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Unsuccessfully"),
                //menuHitLogs = await menuService.GetMenuHitLogByDate(DateTime.Now, DateTime.Now, fbPageId),
                menuHitLogs = await menuService.GetMenuHitLogByDateWOD(fbPageId),
                //totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(DateTime.Now, DateTime.Now, fbPageId),
                totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId),
                allqueries = await queriesService.GetAllQueries(fbPageId)

            };
            return View(model);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("EvercareHome");
            //return Redirect("/Account/Login");
            //int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            //FacebookPage facebookPage = await facebookService.GetFacebookPageById(fbPageId);

            //DashboardViewModel model = new DashboardViewModel
            //{
            //    menus = await menuService.GetMenuCount(fbPageId),
            //    queries = await queriesService.GetQueriesCount(fbPageId),
            //    questions = await questionReplayService.GetAllQUesCountByFbPageID(fbPageId),
            //    FbPageId = fbPageId,
            //    todaysNewUser = await analyticsService.TodaysNewUserByPageId(facebookPage.PageId),
            //    todaysRepeatedUser = await analyticsService.TodaysRepeatedUserByPageId(facebookPage.PageId),
            //    GreetingsMessage = facebookPage.PageGreetingMessage,
            //    GreetingsMessageEN = facebookPage.PageGreetingMessageEN,
            //    totalCountViewModels = await policeDashBoardService.TotalCountViewModelsToday(),
            //    //totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Successfully"),
            //    totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModelsWOD("Successfully"),
            //    totalUnSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Unsuccessfully"),
            //    //menuHitLogs = await menuService.GetMenuHitLogByDate(DateTime.Now, DateTime.Now, fbPageId),
            //    menuHitLogs = await menuService.GetMenuHitLogByDateWOD(fbPageId),
            //    //totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(DateTime.Now, DateTime.Now, fbPageId),
            //    totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId),
            //    allqueries = await queriesService.GetAllQueries(fbPageId)



            //};
            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DashboardViewModel _model)
        {


            if (!ModelState.IsValid)
            {
                int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
                FacebookPage facebookPage = await facebookService.GetFacebookPageById(fbPageId);
                DashboardViewModel model = new DashboardViewModel
                {
                    FbPageId = fbPageId,
                    GreetingsMessage = facebookPage.PageGreetingMessage,
                    GreetingsMessageEN = facebookPage.PageGreetingMessageEN
                };
                return View(model);
            }

            _model.GreetingsMessage = DataFilter.FilterUserString(_model.GreetingsMessage);

            await facebookService.UpdatePageGreetingsMessage(_model.FbPageId, _model.GreetingsMessage, _model.GreetingsMessageEN);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Messages()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [Route("global/api/TotalHitInfoViewModels/{fromDate}/{toDate}/{Status}/{Id}")]
        [HttpGet]

        public async Task<IActionResult> TotalHitInfoViewModels(DateTime fromDate, DateTime toDate, string Status, int Id)
        {

            //IEnumerable<TotalHitInfotViewModel> model = await policeDashBoardService.TotalHitInfoViewModels(fromDate,toDate,Status);
            IEnumerable<TotalHitInfotViewModel> model = await policeDashBoardService.TotalHitInfoViewModelsWOD(Status);
            if (Id == 1)
            {
                model = model.Where(x => x.Date?.Date == DateTime.Now.Date);
            }
            else if (Id == 2)
            {
                model = model.Where(x => x.Date?.Date <= DateTime.Now.Date);
            }
            else
            {
                model = model.Where(x => x.Date?.Date >= fromDate.Date && x.Date?.Date <= toDate.Date);
            }
            return Json(model);
        }

        [AllowAnonymous]
        [Route("global/api/TotalHitInfoViewModelsTodayS/")]
        [HttpGet]
        public async Task<IActionResult> TotalHitInfoViewModelsTodayS()
        {
            IEnumerable<TotalHitInfotViewModel> model = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Successfully");
            return Json(model);
        }

        [AllowAnonymous]
        [Route("global/api/TotalHitInfoViewModelsTodayU/")]
        [HttpGet]
        public async Task<IActionResult> TotalHitInfoViewModelsTodayU()
        {
            IEnumerable<TotalHitInfotViewModel> model = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Unsuccessfully");
            return Json(model);
        }

        [AllowAnonymous]
        [Route("global/api/TotalCountViewModelsDateRange/{fromDate}/{toDate}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountViewModelsDateRange(DateTime fromDate, DateTime toDate)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalCountViewModel> model = await policeDashBoardService.TotalCountViewModelsDateRange(fromDate, toDate);
            var model = await policeDashBoardService.TotalHitInfoViewModelsWOD("Successfully");

            return Json(model.Where(x => x.Date?.Date >= fromDate && x.Date?.Date <= toDate).Count());
        }

        [AllowAnonymous]
        [Route("global/api/TotalCountMenuViewModelsDateRange/{fromDate}/{toDate}/{Id}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountMenuViewModelsDateRange(DateTime fromDate, DateTime toDate, int Id)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalHitMenuLogViewModel> model = await menuService.GetMenuHitLogByDate(fromDate, toDate, fbPageId);
            IEnumerable<TotalHitMenuLogViewModel> model = await menuService.GetMenuHitLogByDateWOD(fbPageId);
            if (Id == 1)
            {
                model = model.Where(x => x.hitDate?.Date == DateTime.Now.Date);
            }
            else if (Id == 2)
            {
                model = model.Where(x => x.hitDate?.Date <= DateTime.Now.Date);
            }
            else
            {
                model = model.Where(x => x.hitDate?.Date >= fromDate.Date && x.hitDate?.Date <= toDate.Date);
            }
            var modelx = (from a in model
                          group a by a.menuNameEn into tmp
                          select new
                          {
                              menuNameEn = tmp.Select(x => x.menuNameEn).FirstOrDefault(),
                              totalHit = tmp.Sum(x => x.totalHit)
                          }).ToList();

            return Json(modelx);
        }
        [AllowAnonymous]
        [Route("global/api/TotalCountMenuViewModelsDateRangeL/{fromDate}/{toDate}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountMenuViewModelsDateRangeL(DateTime fromDate, DateTime toDate)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalHitMenuLogViewModel> model = await menuService.GetMenuHitLogByDate(fromDate, toDate, fbPageId);
            var model = await menuService.GetMenuHitLogByDateWOD(fbPageId);



            return Json(model.Where(x => x.hitDate?.Date >= fromDate && x.hitDate?.Date <= toDate).Sum(x => x.totalHit));
        }

        [AllowAnonymous]
        [Route("global/api/TotalCountKnowledgerViewModelsDateRange/{fromDate}/{toDate}/{Id}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountKnowledgerViewModelsDateRange(DateTime fromDate, DateTime toDate, int Id)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalHitKnowledgeLogViewModel> model = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(fromDate, toDate, fbPageId);
            IEnumerable<TotalHitKnowledgeLogViewModel> model = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId);
            if (Id == 1)
            {
                model = model.Where(x => x.hitDate?.Date == DateTime.Now.Date);
            }
            else if (Id == 2)
            {
                model = model.Where(x => x.hitDate?.Date <= DateTime.Now.Date);
            }
            else
            {
                model = model.Where(x => x.hitDate?.Date >= fromDate.Date && x.hitDate?.Date <= toDate.Date);
            }
            return Json(model);
        }
        [AllowAnonymous]
        [Route("global/api/TotalCountKnowledgeViewModelsDateRangeL/{fromDate}/{toDate}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountKnowledgeViewModelsDateRangeL(DateTime fromDate, DateTime toDate)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            //IEnumerable<TotalHitKnowledgeLogViewModel> model = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(fromDate, toDate, fbPageId);
            var model = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId);

            return Json(model.Where(x => x.hitDate?.Date >= fromDate && x.hitDate?.Date <= toDate).Sum(x => x.totalHit));
        }

        [AllowAnonymous]
        [Route("global/api/TotalCountqueryViewModelsDateRangeL/{fromDate}/{toDate}")]
        [HttpGet]

        public async Task<IActionResult> TotalCountqueryViewModelsDateRangeL(DateTime fromDate, DateTime toDate)
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            IEnumerable<Queries> model = await queriesService.GetAllQueries(fbPageId);

            return Json(model.Where(x => x.entryDate?.Date >= fromDate.Date && x.entryDate?.Date <= toDate.Date).Count());
        }



        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            int fbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            FacebookPage facebookPage = await facebookService.GetFacebookPageById(fbPageId);

            DashboardViewModel model = new DashboardViewModel
            {
                menus = await menuService.GetMenuCount(fbPageId),
                queries = await queriesService.GetQueriesCount(fbPageId),
                questions = await questionReplayService.GetAllQUesCountByFbPageID(fbPageId),
                FbPageId = fbPageId,
                todaysNewUser = await analyticsService.TodaysNewUserByPageId(facebookPage.PageId),
                todaysRepeatedUser = await analyticsService.TodaysRepeatedUserByPageId(facebookPage.PageId),
                GreetingsMessage = facebookPage.PageGreetingMessage,
                GreetingsMessageEN = facebookPage.PageGreetingMessageEN,
                totalCountViewModels = await policeDashBoardService.TotalCountViewModelsToday(),
                //totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Successfully"),
                totalSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModelsWOD("Successfully"),
                totalUnSuccessHitInfotViewModels = await policeDashBoardService.TotalHitInfoViewModels(DateTime.Now, DateTime.Now, "Unsuccessfully"),
                //menuHitLogs = await menuService.GetMenuHitLogByDate(DateTime.Now, DateTime.Now, fbPageId),
                menuHitLogs = await menuService.GetMenuHitLogByDateWOD(fbPageId),
                //totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogViewModelsByDate(DateTime.Now, DateTime.Now, fbPageId),
                totalHitKnowledgeLogViewModels = await knowledgeService.TotalHitKnowledgeLogWPD(fbPageId),
                allqueries = await queriesService.GetAllQueries(fbPageId)



            };
            return View(model);
        }

        #region Dashboard

        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentInformationWithStatusForPublic(string botKey)
        {
            var username = User.Identity.Name;

            var bot = await _botService.GetBotInfoByUserName(username);

            //ChatbotInfo botkeys = await facebookService.botKeyInfo(botKey);
            var model = await _appointmentService.GetDepartmentInforamtionWithStatus(bot.botKey);

            return Json(model);
        }

        #endregion
        #region chart Appointment
        [HttpGet]
        public async Task<IActionResult> GetTotalAppointmentData(string date, string botKey)
        {
            var model = await _appointmentService.GetTotalAppointmentData(date, botKey);

            return Json(model);
        }

        #endregion

        #region API DATA
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> CheckUhid(string uhid, string prefix, string phone, string nextNodeId, string botKey, string connectionId)
        //{
        //    try
        //    {
        //        string apiData = "";
        //        string mobileno = prefix + phone;
        //        if (mobileno.Length > 11)
        //        {
        //            mobileno = mobileno.Substring(mobileno.Length - 11);
        //        }
        //        if (uhid != null)
        //        {
        //            apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=registrationno&value=" + uhid;
        //        }
        //        else if (phone != null)
        //        {
        //            apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=mobileno&value=" + mobileno;
        //        }


        //        var token = await _botService.GetActiveToken();
        //        int nodeId = int.Parse(nextNodeId);
        //        string bearerToken = token.token;
        //        var data = await ApiCall.GetApiResponseAsync<IEnumerable<UhidVm>>(apiData, bearerToken);
        //         //var data = await _context.UserInfos.Where(x => x.Mobile.EndsWith(mobileno.Substring(Math.Max(0, mobileno.Length - 10)))).ToListAsync();

        //        var user = new List<Models.BotModels.UserInfo>();
        //        if (data.Count() > 0)
        //        {
        //            foreach (var item in data)
        //            {
        //                if (item.facility == "EHD")
        //                {
        //                    user.Add(new Models.BotModels.UserInfo
        //                    {
        //                        Id = Guid.NewGuid().ToString(),
        //                        FullName = item.patientName,
        //                        Mobile = item.mobileNo,
        //                        UHID = item.registrationNo,
        //                        gender = item.gender,
        //                        dateOfBirth = item.dateofBirth,
        //                        Email = item.email,
        //                        branchInfoId = 1,
        //                    });

        //                }
        //                else if (item.facility == "EHC")
        //                {
        //                    user.Add(new Models.BotModels.UserInfo
        //                    {
        //                        Id = Guid.NewGuid().ToString(),
        //                        FullName = item.patientName,
        //                        Mobile = item.mobileNo,
        //                        UHID = item.registrationNo,
        //                        gender = item.gender,
        //                        dateOfBirth = item.dateofBirth,
        //                        Email = item.email,
        //                        branchInfoId = 2,
        //                    });
        //                }

        //            }
        //            var saveUhid = await _botService.SaveUhidUserFromApi(user);

        //            var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);

        //            if (data.Any())
        //            {
        //                var lastItem = data.Last();
        //                var answerText = lastItem.mobileNo;
        //                //var answerText = lastItem.Mobile;
        //                var serviceData = new ServiceFlow
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    DateTime = DateTime.Now,
        //                    InfoType = "start",
        //                    ServiceCode = "Pre-Defined Question",
        //                    StepNo = 1,
        //                    Attempt = 0,
        //                    botKey = botKey,
        //                    connectionId = connectionId,
        //                    status = 1,
        //                    answerText = answerText,
        //                    questionText = "Phone",
        //                    MenuId = 0,
        //                    keyWordQuesAnsId = null,
        //                    branchInfoId = botInfo.ApplicationUser?.branchId
        //                };

        //                await _botService.SaveServiceFlow(serviceData);

        //            }
        //            //var nextNode = await _botService.SendOTPByNodeId(botKey, connectionId, nodeId);
        //            var nextNode = await _botService.SendOTPByNodeId2(botKey, connectionId, nodeId);

        //            var result = new List<string>();

        //            var otpCode = await _botService.GetLastOTPByConnectionId(connectionId);

        //            var phoneNumber = await _botService.GetPhoneByConnectionId(connectionId);

        //            var quesAns = await _botService.GetKeywordQuesById(nodeId);

        //            if (quesAns.smsOtp == 1)
        //            {
        //                var otpmsg = "<p>Please enter otp received on your number</p>";
        //                var otpHtml = "";
        //                if (_configuration["Project:isLive"] == "YES")
        //                {
        //                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

        //                }
        //                else
        //                {
        //                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

        //                }

        //                result.Add(otpmsg);
        //                result.Add(otpHtml);
        //            }
        //            return Json(result);
        //        }
        //        else
        //        {
        //            var result = new List<string>();
        //            var msg = "";
        //            var node = _context.keyWordQuesAns
        //                .Where(x => x.question == "Do you have registered mobile?" && x.botKey == botKey)
        //                .FirstOrDefault();

        //            if (uhid != null)
        //            {
        //                msg = "UHID not match!";

        //            }
        //            else if (phone != null)
        //            {
        //                msg = "Phone number not match!";
        //            }

        //            var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-20px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

        //            var part2 = "";
        //            foreach (var btn in node.answer.Split(","))
        //            {

        //                var btnText = btn;

        //                part2 += "<button class='btn-group-message'" + " value='" + node.Id + "'" +
        //                    " onclick='handleButtonClick(this, " + node.Id + ")'>" + btnText?.Trim() + "</button>";
        //            }

        //            var part3 = "</div></div>";

        //            result.Add(msg);
        //            result.Add(node.question);
        //            result.Add(part1 + part2 + part3);

        //            return Json(result);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUhid(string uhid, string prefix, string phone, string nextNodeId, string botKey, string connectionId)
        {
            try
            {
                string apiData = "";
                string mobileno = prefix + phone;
                if (mobileno.Length > 11)
                {
                    mobileno = mobileno.Substring(mobileno.Length - 11);
                }
                if (uhid != null)
                {
                    apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=registrationno&value=" + uhid;
                }
                else if (phone != null)
                {
                    apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=mobileno&value=" + mobileno;
                }


                var token = await _botService.GetActiveToken();
                int nodeId = int.Parse(nextNodeId);
                string bearerToken = token.token;
                var data = await ApiCall.GetApiResponseAsync<IEnumerable<UhidVm>>(apiData, bearerToken);
                //var data = await _context.UserInfos.Where(x => x.Mobile.EndsWith(mobileno.Substring(Math.Max(0, mobileno.Length - 10)))).ToListAsync();

                var user = new List<Models.BotModels.UserInfo>();
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.facility == "EHD")
                        {
                            user.Add(new Models.BotModels.UserInfo
                            {
                                Id = Guid.NewGuid().ToString(),
                                FullName = item.patientName,
                                Mobile = "+88" + item.mobileNo,
                                UHID = item.registrationNo,
                                gender = item.gender,
                                dateOfBirth = item.dateofBirth,
                                Email = item.email,
                                branchInfoId = 1,
                            });

                        }
                        else if (item.facility == "EHC")
                        {
                            user.Add(new Models.BotModels.UserInfo
                            {
                                Id = Guid.NewGuid().ToString(),
                                FullName = item.patientName,
                                Mobile = "+88" + item.mobileNo,
                                UHID = item.registrationNo,
                                gender = item.gender,
                                dateOfBirth = item.dateofBirth,
                                Email = item.email,
                                branchInfoId = 2,
                            });
                        }

                    }
                    var saveUhid = await _botService.SaveUhidUserFromApi(user);

                    var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);

                    if (data.Any())
                    {
                        var lastItem = data.Last();
                        var answerText = lastItem.mobileNo;
                        var serviceData = new ServiceFlow
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateTime = DateTime.Now,
                            InfoType = "start",
                            ServiceCode = "Pre-Defined Question",
                            StepNo = 1,
                            Attempt = 0,
                            botKey = botKey,
                            connectionId = connectionId,
                            status = 1,
                            answerText = answerText,
                            questionText = "Phone",
                            MenuId = 0,
                            keyWordQuesAnsId = null,
                            branchInfoId = botInfo.ApplicationUser?.branchId
                        };

                        await _botService.SaveServiceFlow(serviceData);

                    }
                    //var nextNode = await _botService.SendOTPByNodeId(botKey, connectionId, nodeId);
                    var nextNode = await _botService.SendOTPByNodeId2(botKey, connectionId, nodeId);

                    var result = new List<string>();

                    var otpCode = await _botService.GetLastOTPByConnectionId(connectionId);

                    var phoneNumber = await _botService.GetPhoneByConnectionId(connectionId);

                    var quesAns = await _botService.GetKeywordQuesById(nodeId);
                    if (quesAns.smsOtp == 1)
                    {
                        var lastuser = data.Last();
                        var part1 = "<form id='frmBasic2'  class='formMobile' style= 'background-color: white;  padding: 8px; margin-left= -80px; margin-top= -10px;'>";
                        var part2 = "<div class='input-group' style='width: 100%;' id='email'>" +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Patient Name </span>" + "<span style='width: 60%; font-size: 11px;'> : " + lastuser.patientName + "</span>" + " </p> " +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Phone </span>" + "<span style='width: 190px; font-size: 11px;'> : " + lastuser.mobileNo + "</span>" + " </p> " +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Email </span>" + "<span style='width: 190px; font-size: 11px;'> : " + lastuser.email + "</span>" + " </p> " +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Date of Birth </span>" + "<span style='width: 190px; font-size: 11px;'> : " + lastuser.dateofBirth + "</span>" + " </p> " +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Gender </span>" + "<span style='width: 190px; font-size: 11px;'> : " + lastuser.gender + "</span>" + " </p> " +
                                                " <p style='width: 100%;'> " + "<span style='width: 40%; font-size: 12px; font-weight: bold'>Registration No </span>" + "<span style='width: 190px; font-size: 11px;'> : " + lastuser.registrationNo + "</span>" + " </p> " +


                                      "</div>" +


                                      //"<div class='input-group' style='width: 100%;' id='name'>" +
                                      //  "<div class='input-container'>" +
                                      //          //"<input type='text' style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control' value='" + lastuser.patientName + "' placeholder='" + lastuser.patientName + "' aria-label='Name'>" +
                                      //  "</div>" +
                                      //"</div>" +
                                      //"<div class='input-group' style='width: 100%;' id='phone'>" +
                                      //      "<div class='input-container'>" +

                                      //          //"<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control' value='" + lastuser.mobileNo + "' placeholder='" + lastuser.mobileNo + "' aria-label='Name'>" +
                                      //      "</div>" +
                                      // "</div>" +
                                      //"<div class='input-group' style='width: 100%;' id='email'>" +

                                      //"<div class='input-container'>" +

                                      ////"<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control' value='" + lastuser.email + "' placeholder='" + lastuser.email + "'>" +
                                      //"</div>" +
                                      //"</div>" +
                                      //"<div class='input-group' style='width: 100%;' id='dob'>" +
                                      //      "<div class='input-container'>" +

                                      //          //"<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control'  value='" + lastuser.dateofBirth + "' placeholder='" + lastuser.dateofBirth + "'>" +
                                      //      "</div>" +
                                      // "</div>" +
                                      // "<div class='input-group' style='width: 100%;' id='gender'>" +
                                      //      "<div class='input-container'>" +

                                      //          //"<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control'  value='" + lastuser.gender + "' placeholder='" + lastuser.gender + "'>" +
                                      //      "</div>" +
                                      // "</div>" +
                                      // "<div class='input-group' style='width: 100%;' id='uhid'>" +
                                      //      "<div class='input-container'>" +

                                      //          //"<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control'  value='" + lastuser.registrationNo + "' placeholder='" + lastuser.registrationNo + "'>" +
                                      //      "</div>" +
                                      // "</div>" +
                                      "<input type='button' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px; width: 100%' onclick='SubmitOtp(" + nodeId + ")' value='Ok' />";
                        var part4 = "</form>";




                        result.Add(part1 + part2 + part4);
                    }

                    return Json(result);
                }
                else
                {
                    var result = new List<string>();
                    var msg = "";
                    var node = _context.keyWordQuesAns
                        .Where(x => x.question == "Do you have registered mobile?" && x.botKey == botKey)
                        .FirstOrDefault();

                    if (uhid != null)
                    {
                        msg = "UHID not match!";

                    }
                    else if (phone != null)
                    {
                        msg = "Phone number not match!";
                    }

                    var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-20px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

                    var part2 = "";
                    foreach (var btn in node.answer.Split(","))
                    {

                        var btnText = btn;

                        part2 += "<button class='btn-group-message'" + " value='" + node.Id + "'" +
                            " onclick='handleButtonClick(this, " + node.Id + ")'>" + btnText?.Trim() + "</button>";
                    }

                    var part3 = "</div></div>";

                    result.Add(msg);
                    result.Add(node.question);
                    result.Add(part1 + part2 + part3);

                    return Json(result);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckApiMobile(string uhid, string prefix, string phone, string nextNodeId, string botKey, string connectionId)
        {
            try
            {
                string apiData = "";
                string mobileno = prefix + phone;
                if (mobileno.Length > 11)
                {
                    mobileno = mobileno.Substring(mobileno.Length - 11);
                }
                if (uhid != null)
                {
                    apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=registrationno&value=" + uhid;
                }
                else if (phone != null)
                {
                    apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=mobileno&value=" + mobileno;
                }


                var token = await _botService.GetActiveToken();
                int nodeId = int.Parse(nextNodeId);
                string bearerToken = token.token;
                var data = await ApiCall.GetApiResponseAsync<IEnumerable<UhidVm>>(apiData, bearerToken);
                //var data = await _context.UserInfos.Where(x => x.Mobile.EndsWith(mobileno.Substring(Math.Max(0, mobileno.Length - 10)))).ToListAsync();

                var user = new List<Models.BotModels.UserInfo>();
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.facility == "EHD")
                        {
                            user.Add(new Models.BotModels.UserInfo
                            {
                                Id = Guid.NewGuid().ToString(),
                                FullName = item.patientName,
                                Mobile = "+88" + item.mobileNo,
                                UHID = item.registrationNo,
                                gender = item.gender,
                                dateOfBirth = item.dateofBirth,
                                Email = item.email,
                                branchInfoId = 1,
                            });

                        }
                        else if (item.facility == "EHC")
                        {
                            user.Add(new Models.BotModels.UserInfo
                            {
                                Id = Guid.NewGuid().ToString(),
                                FullName = item.patientName,
                                Mobile = "+88" + item.mobileNo,
                                UHID = item.registrationNo,
                                gender = item.gender,
                                dateOfBirth = item.dateofBirth,
                                Email = item.email,
                                branchInfoId = 2,
                            });
                        }

                    }
                    var saveUhid = await _botService.SaveUhidUserFromApi(user);

                    var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);

                    if (data.Any())
                    {
                        var lastItem = data.Last();
                        var answerText = lastItem.mobileNo;
                        var serviceData = new ServiceFlow
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateTime = DateTime.Now,
                            InfoType = "start",
                            ServiceCode = "Pre-Defined Question",
                            StepNo = 1,
                            Attempt = 0,
                            botKey = botKey,
                            connectionId = connectionId,
                            status = 1,
                            answerText = answerText,
                            questionText = "Phone",
                            MenuId = 0,
                            keyWordQuesAnsId = null,
                            branchInfoId = botInfo.ApplicationUser?.branchId
                        };

                        await _botService.SaveServiceFlow(serviceData);

                    }
                    //var nextNode = await _botService.SendOTPByNodeId(botKey, connectionId, nodeId);
                    // var nextNode = await _botService.SendOTPByNodeId2(botKey, connectionId, nodeId);

                    var result = new List<string>();

                    var otpCode = await _botService.GetLastOTPByConnectionId(connectionId);

                    var phoneNumber = await _botService.GetPhoneByConnectionId(connectionId);

                    var quesAns = await _botService.GetKeywordQuesById(nodeId);
                    if (quesAns.smsOtp == 1)
                    {
                        var part1 = "<form id='frmBasic'  class='formMobile' style= 'background-color: white;  padding: 8px; margin-left= -80px; margin-top= -10px;'>" +
                            "<div class='input-group-prepend'>" +
                                                   "<span class='input-group-text' style='font-size: 11px; color: rgb(51, 51, 51); padding-bottom: 10px; line-height: 0.9;'>Select UHID</span>" +
                                               "</div>" +
                            "<div class='input-group' style='width: 100%;' id='name'>" +
                                       "<div class='input-container'>" +
                                             "<select class='custom-select form-control' id='uhid' name='uhid' style='width:80%;'>";
                        var part2 = "";
                        foreach (var item in data)
                        {
                            if (item.facility == "EHD" || item.facility == "EHC")
                            {

                                part2 += "<option value='" + item.registrationNo + "' selected>" + item.registrationNo + "</option>";


                            }


                        }
                        var part3 = "</select>" + "<input type='hidden' name='botKey' id='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' id='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' id='nextNodeId' value='" + nodeId + "' />" + "<input type='button' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px; width: 20%' onclick='SubmitUHID()' value='Ok' />" +
                                "</div> " + "</div>";
                        var part4 = "</form>";




                        result.Add(part1 + part2 + part3 + part4);
                    }

                    return Json(result);
                }
                else
                {
                    var result = new List<string>();
                    var msg = "";
                    var node = _context.keyWordQuesAns
                        .Where(x => x.question == "Do you have registered mobile?" && x.botKey == botKey)
                        .FirstOrDefault();

                    if (uhid != null)
                    {
                        msg = "UHID not match!";

                    }
                    else if (phone != null)
                    {
                        msg = "Phone number not match!";
                    }

                    var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-20px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

                    var part2 = "";
                    foreach (var btn in node.answer.Split(","))
                    {

                        var btnText = btn;

                        part2 += "<button class='btn-group-message'" + " value='" + node.Id + "'" +
                            " onclick='handleButtonClick(this, " + node.Id + ")'>" + btnText?.Trim() + "</button>";
                    }

                    var part3 = "</div></div>";

                    result.Add(msg);
                    result.Add(node.question);
                    result.Add(part1 + part2 + part3);

                    return Json(result);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> CheckApiMobile(string uhid, string prefix, string phone, string nextNodeId, string botKey, string connectionId)
        //{
        //    try
        //    {
        //        string apiData = "";
        //        string mobileno = prefix + phone;
        //        if (mobileno.Length > 11)
        //        {
        //            mobileno = mobileno.Substring(mobileno.Length - 11);
        //        }
        //        if (uhid != null)
        //        {
        //            apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=registrationno&value=" + uhid;
        //        }
        //        else if (phone != null)
        //        {
        //            apiData = "https://Applink.evercarebd.com:8018/api/Registration?type=mobileno&value=" + mobileno;
        //        }


        //        var token = await _botService.GetActiveToken();
        //        int nodeId = int.Parse(nextNodeId);
        //        string bearerToken = token.token;
        //        var data = await ApiCall.GetApiResponseAsync<IEnumerable<UhidVm>>(apiData, bearerToken);
        //        //var data = await _context.UserInfos.Where(x => x.Mobile.EndsWith(mobileno.Substring(Math.Max(0, mobileno.Length - 10)))).ToListAsync();

        //        var user = new List<Models.BotModels.UserInfo>();
        //        if (data.Count() > 0)
        //        {
        //            foreach (var item in data)
        //            {
        //                if (item.facility == "EHD")
        //                {
        //                    user.Add(new Models.BotModels.UserInfo
        //                    {
        //                        Id = Guid.NewGuid().ToString(),
        //                        FullName = item.patientName,
        //                        Mobile = item.mobileNo,
        //                        UHID = item.registrationNo,
        //                        gender = item.gender,
        //                        dateOfBirth = item.dateofBirth,
        //                        Email = item.email,
        //                        branchInfoId = 1,
        //                    });

        //                }
        //                else if (item.facility == "EHC")
        //                {
        //                    user.Add(new Models.BotModels.UserInfo
        //                    {
        //                        Id = Guid.NewGuid().ToString(),
        //                        FullName = item.patientName,
        //                        Mobile = item.mobileNo,
        //                        UHID = item.registrationNo,
        //                        gender = item.gender,
        //                        dateOfBirth = item.dateofBirth,
        //                        Email = item.email,
        //                        branchInfoId = 2,
        //                    });
        //                }

        //            }
        //            var saveUhid = await _botService.SaveUhidUserFromApi(user);

        //            var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);

        //            if (data.Any())
        //            {
        //                var lastItem = data.Last();
        //                var answerText = lastItem.mobileNo;
        //                var serviceData = new ServiceFlow
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    DateTime = DateTime.Now,
        //                    InfoType = "start",
        //                    ServiceCode = "Pre-Defined Question",
        //                    StepNo = 1,
        //                    Attempt = 0,
        //                    botKey = botKey,
        //                    connectionId = connectionId,
        //                    status = 1,
        //                    answerText = answerText,
        //                    questionText = "Phone",
        //                    MenuId = 0,
        //                    keyWordQuesAnsId = null,
        //                    branchInfoId = botInfo.ApplicationUser?.branchId
        //                };

        //                await _botService.SaveServiceFlow(serviceData);

        //            }
        //            //var nextNode = await _botService.SendOTPByNodeId(botKey, connectionId, nodeId);
        //            var nextNode = await _botService.SendOTPByNodeId2(botKey, connectionId, nodeId);

        //            var result = new List<string>();

        //            var otpCode = await _botService.GetLastOTPByConnectionId(connectionId);

        //            var phoneNumber = await _botService.GetPhoneByConnectionId(connectionId);

        //            var quesAns = await _botService.GetKeywordQuesById(nodeId);
        //            if (quesAns.smsOtp == 1)
        //            {
        //                var lastuser = data.Last();
        //                var part1 = "<form id='frmBasic'  class='formMobile' style= 'background-color: white;  padding: 8px; margin-left= -80px; margin-top= -10px;'>";
        //                var part2 = "<div class='input-group' style='width: 100%;' id='name'>" +
        //                               "<div class='input-container'>" +
        //                                      "<select class='custom-select form-control' id='inputGroupSelect04' name='numberCode' style='width:70px;'>" +
        //                                            "<option value='+880' selected>+880</option>" +
        //                                      "</select>" +
        //                               "</div>" +
        //                             "</div>" +
        //                             "<div class='input-group' style='width: 100%;' id='phone'>" +
        //                                   "<div class='input-container'>" +
        //                                       "<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control' value='" + lastuser.mobileNo + "' placeholder='" + lastuser.mobileNo + "' aria-label='Name'>" +
        //                                   "</div>" +
        //                              "</div>" +
        //                             "<div class='input-group' style='width: 100%;' id='email'>" +
        //                                   "<div class='input-container'>" +
        //                                       "<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control' value='" + lastuser.email + "' placeholder='" + lastuser.email + "'>" +
        //                                   "</div>" +
        //                              "</div>" +
        //                             "<div class='input-group' style='width: 100%;' id='dob'>" +
        //                                   "<div class='input-container'>" +
        //                                       "<input type='text'  style='border: 1px solid lightgrey;cursor: pointer;' readonly class='form-control'  value='" + lastuser.dateofBirth + "' placeholder='" + lastuser.dateofBirth + "'>" +
        //                                   "</div>" +
        //                              "</div>" +
        //                              "<input type='button' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px; width: 100%' onclick='SubmitOtp(" + nodeId + ")' value='Ok' />";
        //                var part4 = "</form>";




        //                result.Add(part1 + part2 + part4);
        //            }

        //            return Json(result);
        //        }
        //        else
        //        {
        //            var result = new List<string>();
        //            var msg = "";
        //            var node = _context.keyWordQuesAns
        //                .Where(x => x.question == "Do you have registered mobile?" && x.botKey == botKey)
        //                .FirstOrDefault();

        //            if (uhid != null)
        //            {
        //                msg = "UHID not match!";

        //            }
        //            else if (phone != null)
        //            {
        //                msg = "Phone number not match!";
        //            }

        //            var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-20px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

        //            var part2 = "";
        //            foreach (var btn in node.answer.Split(","))
        //            {

        //                var btnText = btn;

        //                part2 += "<button class='btn-group-message'" + " value='" + node.Id + "'" +
        //                    " onclick='handleButtonClick(this, " + node.Id + ")'>" + btnText?.Trim() + "</button>";
        //            }

        //            var part3 = "</div></div>";

        //            result.Add(msg);
        //            result.Add(node.question);
        //            result.Add(part1 + part2 + part3);

        //            return Json(result);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckOtp(string nextNodeId, string botKey, string connectionId)
        {
            try
            {

                int nodeId = int.Parse(nextNodeId);

                var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);

                //var nextNode = await _botService.SendOTPByNodeId2(botKey, connectionId, nodeId);

                var result = new List<string>();

                var otpCode = await _botService.GetLastOTPByConnectionId(connectionId);

                var phoneNumber = await _botService.GetPhoneByConnectionId(connectionId);

                var quesAns = await _botService.GetKeywordQuesById(nodeId);

                if (quesAns.smsOtp == 1)
                {
                    var otpmsg = "<p>Please enter otp received on your number</p>";
                    var otpHtml = "";
                    if (_configuration["Project:isLive"] == "YES")
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                    }
                    else
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                    }

                    result.Add(otpmsg);
                    result.Add(otpHtml);
                }
                return Json(result);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        #region Old Api Data Department, Doctor,  Specialisation 

        [HttpGet("/api/UpdateAllApiData")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateAllApiData()
        {
            string departmentsurl = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehd";
            string department2url = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehc";
            string doctorsurl = "https://Applink.evercarebd.com:8018/api/Employee/Ehd";
            string doctors2url = "https://Applink.evercarebd.com:8018/api/Employee/Ehc";
            string specialisationsurl = "https://Applink.evercarebd.com:8018/api/SpecialisationMaster";



            var token = await _botService.GetActiveToken();

            string bearerToken = token.token;

            try
            {
                var uniqueKey = DateTime.Now.Ticks.ToString();


                #region ApiDepartment
                var departments = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(departmentsurl, bearerToken);
                var department2 = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(department2url, bearerToken);

                var departmentData = new List<ApiDepartment>();

                foreach (var item in departments)
                {
                    departmentData.Add(new ApiDepartment
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });
                }

                foreach (var item in department2)
                {
                    departmentData.Add(new ApiDepartment
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });
                }

                var departmentResult = await _botService.SaveApiDepartment(departmentData);
                #endregion



                #region ApiDoctor
                var doctors = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctorsurl, bearerToken);
                var doctors2 = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctors2url, bearerToken);

                var doctorsData = new List<ApiDoctor>();


                // Process doctors from EHD
                foreach (var item in doctors)
                {
                    doctorsData.Add(new ApiDoctor
                    {
                        Id = 0,
                        code = item.code,
                        doctorId = item.id,
                        name = item.name,
                        specialization = item.specialization,
                        department = item.department,
                        gender = item.gender,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });


                }

                // Process doctors from EHC
                foreach (var item in doctors2)
                {
                    doctorsData.Add(new ApiDoctor
                    {
                        Id = 0,
                        doctorId = item.id,
                        code = item.code,
                        name = item.name,
                        specialization = item.specialization,
                        department = item.department,
                        gender = item.gender,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });


                }


                var doctorResult = await _botService.SaveApiDoctor(doctorsData);
                #endregion





                #region ApiSpecialisation
                var specialisation = await ApiCall.GetApiResponseAsync<List<SpecialisationVm>>(specialisationsurl, bearerToken);

                var specialisationData = new List<ApiSpecialisation>();

                foreach (var item in specialisation)
                {
                    specialisationData.Add(new ApiSpecialisation
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        uniqueKey = uniqueKey
                    });
                }

                var specialisationResult = await _botService.SaveApiSpecialisation(specialisationData);
                #endregion

                var combinedResult = new UpdateResult
                {
                    DepartmentResult = departmentResult,
                    DoctorResult = doctorResult,
                    SpecialisationResult = specialisationResult,
                    //SlotResult = slotResult,
                };
                var model = await _botService.GetAllMasterData();

                //if (token.phone != null)
                //{
                //    smsAPI.Single_Sms(token.phone, "Api Process Successfully Done!");
                //}

                if ((DateTime.Now.Date - token.expiryDate.Date).Days <= 10 && token.sentAlert == 0)
                {
                    smsAPI.Single_Sms(token.phone, "Api token will expire soon!");
                    await _botService.UpdateTokenMessageStatus();
                }

                return Json(combinedResult);

                //return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(null);
            }
        }
        #endregion



        [HttpGet("/api/GetPrevDoctor")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrevDoctor(int apiDocId, string botKey)
        {
            //var data = await _botService.GetPrevDoctor(apiDocId);

            var allDoc = await _botService.FetchDoctors(botKey);

            //var department = allDoc.Where(x => x.id == apiDocId).Select(x => x.department).FirstOrDefault();
            var specialization = allDoc.Where(x => x.id == apiDocId).Select(x => x.specialization).FirstOrDefault();

            var result = new DoctorWithSlotsVm();
            if (!string.IsNullOrEmpty(specialization))
            {
                //var specificDoct = allDoc.Where(x => x.department == department && x.id < apiDocId).LastOrDefault();
                var specificDoct = allDoc.Where(x => x.department == specialization && x.id < apiDocId).LastOrDefault();

                if (specificDoct != null)
                {
                    var slots = await _botService.Fetch7DaysSlot(botKey, specificDoct.id);

                    result = new DoctorWithSlotsVm
                    {
                        doctor = specificDoct,
                        slots = slots,
                        dates = slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct().ToList()
                    };
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return Ok(result);
        }

        [HttpGet("/api/GetNextDoctor")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNextDoctor(int apiDocId, string botKey)
        {
            //var data = await _botService.GetNextDoctor(apiDocId);

            var allDoc = await _botService.FetchDoctors(botKey);

            //var department = allDoc.Where(x => x.id == apiDocId).Select(x => x.department).FirstOrDefault();
            var specialization = allDoc.Where(x => x.id == apiDocId).Select(x => x.specialization).FirstOrDefault();


            var result = new DoctorWithSlotsVm();
            if (!string.IsNullOrEmpty(specialization))
            {
                //var specificDoct = allDoc.Where(x => x.department == department && x.id > apiDocId).OrderBy(x => x.id).FirstOrDefault();
                var specificDoct = allDoc.Where(x => x.specialization == specialization && x.id > apiDocId).OrderBy(x => x.id).FirstOrDefault();

                if (specificDoct != null)
                {
                    var slots = await _botService.Fetch7DaysSlot(botKey, specificDoct.id);

                    result = new DoctorWithSlotsVm
                    {
                        doctor = specificDoct,
                        slots = slots,
                        dates = slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct().ToList()
                    };
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return Ok(result);
        }





        [HttpGet("/api/GetPrevDoctor2")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrevDoctor2(int apiDocId, string botKey, string doctorKey)
        {

            var allDoc = await _botService.FetchDoctors2(botKey, apiDocId, doctorKey);


            var result = new DoctorWithSlotsVm();
            if (allDoc.Count > 0)
            {
                var specificDoct = allDoc.Where(x => x.id < apiDocId).LastOrDefault();

                if (specificDoct != null)
                {
                    var slots = await _botService.Fetch7DaysSlot(botKey, specificDoct.id);

                    result = new DoctorWithSlotsVm
                    {
                        doctor = specificDoct,
                        slots = slots,
                        dates = slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct().ToList()
                    };
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return Ok(result);
        }


        [HttpGet("/api/GetNextDoctor2")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNextDoctor2(int apiDocId, string botKey, string doctorKey)
        {

            var allDoc = await _botService.FetchDoctors2(botKey, apiDocId, doctorKey);



            var result = new DoctorWithSlotsVm();
            if (allDoc.Count > 0)
            {

                var specificDoct = allDoc.Where(x => x.id > apiDocId).OrderBy(x => x.id).FirstOrDefault();

                if (specificDoct != null)
                {
                    var slots = await _botService.Fetch7DaysSlot(botKey, specificDoct.id);

                    result = new DoctorWithSlotsVm
                    {
                        doctor = specificDoct,
                        slots = slots,
                        dates = slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct().ToList()
                    };
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            return Ok(result);
        }









        //[HttpGet("/api/UpdateApiData")]

        //[AllowAnonymous]
        //public async Task<IActionResult> UpdateApiData()
        //{
        //    return View();
        //}
        //[HttpGet("/api/UpdateApiDataEHD")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UpdateApiDataEHD()
        //{
        //    string departmentsurl = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehd";
        //     string doctorsurl = "https://Applink.evercarebd.com:8018/api/Employee/Ehd";
        //     string specialisationsurl = "https://Applink.evercarebd.com:8018/api/SpecialisationMaster";
        //    string baseDoctorSlotsUrl = "https://Applink.evercarebd.com:8018/api/DoctorSlot/EHD/";

        //    //var prevProcess = await _botService.GetLastProcess();

        //    //if ((DateTime.Now - Convert.ToDateTime(prevProcess.processDate)).Minutes > )
        //    //{

        //    //}
        //    var token = await _botService.GetActiveToken();

        //    string bearerToken = token.token;

        //    try
        //    {
        //        var uniqueKey = DateTime.Now.Ticks.ToString();


        //        #region ApiDepartment
        //        var departments = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(departmentsurl, bearerToken);

        //        var departmentData = new List<ApiDepartment>();

        //        foreach (var item in departments)
        //        {
        //            departmentData.Add(new ApiDepartment
        //            {
        //                Id = 0,
        //                code = item.code,
        //                name = item.name,
        //                date = DateTime.Now,
        //                facility = item.facility,
        //                uniqueKey = uniqueKey
        //            });
        //        }


        //        var departmentResult = await _botService.SaveApiDepartment(departmentData);
        //        #endregion



        //        #region ApiDoctor
        //        var doctors = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctorsurl, bearerToken);

        //        var apiData = new List<DoctorSlotData>();

        //        var doctorsData = new List<ApiDoctor>();


        //        // Process doctors from EHD
        //        foreach (var item in doctors)
        //        {
        //            var slotList = new List<DoctorSlotVm>();

        //            var doctorId = item.id + "/";

        //            for (DateTime i = DateTime.Now; i < DateTime.Now.AddDays(7); i = i.AddDays(1))
        //            {
        //                var doctorSlotUrl = $"{baseDoctorSlotsUrl}{doctorId}{i.ToString("yyyy-MM-dd")}";
        //                var doctorSlot = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(doctorSlotUrl, bearerToken);
        //                slotList.AddRange(doctorSlot);
        //            }

        //            apiData.Add(new DoctorSlotData
        //            {
        //                doctor = item,
        //                slots = slotList,
        //                uniqueKey = uniqueKey
        //            });
        //            //doctorsData.Add(new ApiDoctor
        //            //{
        //            //    Id = 0,
        //            //    code = item.code,
        //            //    doctorId = item.id,
        //            //    name = item.name,
        //            //    specialization = item.specialization,
        //            //    department = item.department,
        //            //    gender = item.gender,
        //            //    date = DateTime.Now,
        //            //    facility = item.facility,
        //            //    uniqueKey = uniqueKey
        //            //});


        //            //if (doctorSlot != null && doctorSlot.Any(x => !string.IsNullOrEmpty(x.doctorID)))
        //            //{
        //            //    foreach (var item2 in doctorSlot)
        //            //    {
        //            //        doctorsSlotData.Add(new Models.ApiModels.ApiDoctorSlot
        //            //        {
        //            //            Id = 0,
        //            //            slotId = item2.id,
        //            //            appointmentDate = item2.appointmentDate,
        //            //            slotFrom = item2.slotFrom,
        //            //            slotTo = item2.slotTo,
        //            //            doctorID = item2.doctorID,
        //            //            facility = item2.facility,
        //            //            remarks = item2.remarks
        //            //        });
        //            //    }
        //            //    var slotResult = await _botService.SaveApiDoctorSlot(doctorsSlotData);
        //            //}
        //        }

        //       var doctorResult =  await _botService.SaveAllDoctorSlotsFromApi(apiData);

        //        // Process doctors from EHC

        //        #endregion





        //        #region ApiSpecialisation
        //        var specialisation = await ApiCall.GetApiResponseAsync<List<SpecialisationVm>>(specialisationsurl, bearerToken);

        //        var specialisationData = new List<ApiSpecialisation>();

        //        foreach (var item in specialisation)
        //        {
        //            specialisationData.Add(new ApiSpecialisation
        //            {
        //                Id = 0,
        //                code = item.code,
        //                name = item.name,
        //                date = DateTime.Now,
        //                uniqueKey = uniqueKey
        //            });
        //        }

        //        var specialisationResult = await _botService.SaveApiSpecialisation(specialisationData);
        //        #endregion

        //        var combinedResult = new UpdateResult
        //        {
        //            DepartmentResult = departmentResult,
        //            DoctorResult = doctorResult,
        //            SpecialisationResult = specialisationResult,
        //            //SlotResult = slotResult,
        //        };
        //        //var model = await _botService.GetAllMasterData();

        //        //if (token.phone != null)
        //        //{
        //        //    smsAPI.Single_Sms(token.phone, "Api Process Successfully Done!");
        //        //}

        //        if ((DateTime.Now.Date - token.expiryDate.Date).Days <= 10 && token.sentAlert == 0)
        //        {
        //            smsAPI.Single_Sms(token.phone, "Api token will expire soon!");
        //            await _botService.UpdateTokenMessageStatus();
        //        }

        //        return Json(combinedResult);


        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred: " + ex.Message);
        //        return Json(null);
        //    }
        //}



        //[HttpGet("/api/UpdateApiDataEHC")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UpdateApiDataEHC()
        //{
        //     string department2url = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehc";
        //     string doctors2url = "https://Applink.evercarebd.com:8018/api/Employee/Ehc";
        //     string specialisationsurl = "https://Applink.evercarebd.com:8018/api/SpecialisationMaster";
        //     string baseDoctorSlots2Url = "https://Applink.evercarebd.com:8018/api/DoctorSlot/EHC/";


        //    var token = await _botService.GetActiveToken();
        //    string bearerToken = token.token;

        //    try
        //    {
        //         var uniqueKey = DateTime.Now.Ticks.ToString();

        //        #region ApiDepartment
        //         var department2 = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(department2url, bearerToken);
        //         var departmentData = new List<ApiDepartment>();


        //        foreach (var item in department2)
        //        {
        //            departmentData.Add(new ApiDepartment
        //            {
        //                Id = 0,
        //                code = item.code,
        //                name = item.name,
        //                date = DateTime.Now,
        //                facility = item.facility,
        //                uniqueKey = uniqueKey
        //            });
        //        }

        //        var departmentResult = await _botService.SaveApiDepartmentEHC(departmentData);
        //        #endregion



        //        #region ApiDoctor
        //        //var doctors2 = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctors2url, bearerToken);

        //        //var apiData2 = new List<DoctorSlotData>();
        //        //var doctorsData = new List<ApiDoctor>();


        //        //// Process doctors from EHC
        //        //try
        //        //{
        //        //    foreach (var item in doctors2)
        //        //    {
        //        //        var slotList2 = new List<DoctorSlotVm>();

        //        //        var doctorId2 = item.id + "/";

        //        //        for (DateTime i = DateTime.Now; i < DateTime.Now.AddDays(3); i = i.AddDays(1))
        //        //        {

        //        //                var doctorSlotUrl2 = $"{baseDoctorSlots2Url}{doctorId2}{i.ToString("yyyy-MM-dd")}";
        //        //                var doctorSlot2 = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(doctorSlotUrl2, bearerToken);
        //        //                Console.WriteLine(doctorSlotUrl2);
        //        //                slotList2.AddRange(doctorSlot2);

        //        //        }

        //        //        apiData2.Add(new DoctorSlotData
        //        //        {
        //        //            doctor = item,
        //        //            slots = slotList2,
        //        //            uniqueKey = uniqueKey
        //        //        });

        //        //        //if (item.id == 9204)
        //        //        //{
        //        //        //    Thread.Sleep(3000);
        //        //        //}
        //        //    }
        //        //}
        //        //catch (Exception ex)
        //        //{

        //        //}


        //        ////var doctorResult = await _botService.SaveApiDoctor(doctorsData);
        //        //var doctorResult = await _botService.SaveAllDoctorSlotsFromApiEhc(apiData2);
        //        #endregion

        //        #region ApiSpecialisation
        //        var specialisation = await ApiCall.GetApiResponseAsync<List<SpecialisationVm>>(specialisationsurl, bearerToken);

        //        var specialisationData = new List<ApiSpecialisation>();

        //        foreach (var item in specialisation)
        //        {
        //            specialisationData.Add(new ApiSpecialisation
        //            {
        //                Id = 0,
        //                code = item.code,
        //                name = item.name,
        //                date = DateTime.Now,
        //                uniqueKey = uniqueKey
        //            });
        //        }

        //        var specialisationResult = await _botService.SaveApiSpecialisation(specialisationData);
        //        #endregion


        //        var combinedResult = new UpdateResult
        //        {
        //            DepartmentResult = departmentResult,
        //            DoctorResult = null,
        //            SpecialisationResult = specialisation

        //        };
        //        //var model = await _botService.GetAllMasterData();

        //        //if (token.phone != null)
        //        //{
        //        //    smsAPI.Single_Sms(token.phone, "Api Process Successfully Done!");
        //        //}

        //        if ((DateTime.Now.Date - token.expiryDate.Date).Days <= 10 && token.sentAlert == 0)
        //        {
        //            smsAPI.Single_Sms(token.phone, "Api token will expire soon!");
        //            await _botService.UpdateTokenMessageStatus();
        //        }

        //        return Json(combinedResult);

        //        //return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred: " + ex.Message);
        //        return Json(null);
        //    }
        //}

        //[HttpGet("/api/UpdateApiDataSP")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UpdateApiDataSP()
        //{

        //    try
        //    {

        //        var model = await _botService.GetAllMasterData();

        //        return Json(model);

        //        //return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred: " + ex.Message);
        //        return Json(null);
        //    }
        //}

        #endregion
        public class UpdateResult
        {
            public object DepartmentResult { get; set; }
            public object DoctorResult { get; set; }
            public object SpecialisationResult { get; set; }
        }

        public class DepartmentVm
        {
            public string code { get; set; }
            public string name { get; set; }
            public string facility { get; set; }
        }
        public class DoctorVm
        {
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string department { get; set; }
            public string gender { get; set; }
            public string specialization { get; set; }
            public string facility { get; set; }
        }
        public class DoctorVm2
        {
            public int id { get; set; }
            public int ApiDoctorId { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string department { get; set; }
            public string gender { get; set; }
            public string specialization { get; set; }
            public string facility { get; set; }
        }
        public class SpecialisationVm
        {
            public string code { get; set; }
            public string name { get; set; }
        }
        public class UhidVm
        {
            public string registrationNo { get; set; }
            public string patientName { get; set; }
            public string mobileNo { get; set; }
            public string gender { get; set; }
            public string email { get; set; }
            public string dateofBirth { get; set; }
            public string facility { get; set; }
        }


        public class DoctorSlotVm
        {

            public int id { get; set; }
            public string appointmentDate { get; set; }
            public string slotFrom { get; set; }
            public string slotTo { get; set; }
            public string doctorID { get; set; }
            public string facility { get; set; }
            public string remarks { get; set; }
        }

        public class DoctorSlotData
        {
            public DoctorVm doctor { get; set; }
            public List<DoctorSlotVm> slots { get; set; }
            public string uniqueKey { get; set; }
        }

        public class EvercareTokenVm
        {
            public string token { get; set; }
            public int isActive { get; set; }
            public string phone { get; set; }
            public DateTime expiryDate { get; set; }
            public int sentAlert { get; set; }
            public DateTime? entryDate { get; set; }

            public IEnumerable<EvercareToken> evercareToken { get; set; }
        }
        public class ApiActivityLogVm
        {
            public IEnumerable<Models.ApiModelData.ApiActivityLog> apiActivityLogs { get; set; }
        }

        #region New Api Data Department, Doctor,  Specialisation 


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ApiAllDataList()
        {


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PullApiDepartment()
        {
            string departmentsurl = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehd";
            string department2url = "https://Applink.evercarebd.com:8018/api/DepartmentMain/Ehc";


            var token = await _botService.GetActiveToken();

            string bearerToken = token.token;

            try
            {
                var uniqueKey = DateTime.Now.Ticks.ToString();


                #region ApiDepartment
                var departments = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(departmentsurl, bearerToken);
                var department2 = await ApiCall.GetApiResponseAsync<List<DepartmentVm>>(department2url, bearerToken);

                var departmentData = new List<ApiDepartmentData>();

                foreach (var item in departments)
                {
                    departmentData.Add(new ApiDepartmentData
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });
                }

                foreach (var item in department2)
                {
                    departmentData.Add(new ApiDepartmentData
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });
                }

                var departmentResult = await _botService.SaveApiDepartmentData(departmentData);
                #endregion

                #region apiActivityLog

                var username = User.Identity.Name;

                var apiActivityLogs = new Models.ApiModelData.ApiActivityLog
                {
                    Id = 0,
                    type = "Pull",
                    DateTime = DateTime.Now,
                    createBy = username,
                    uniqueKey = uniqueKey,
                    jsonResponse = "Department: " + JsonConvert.SerializeObject(departmentData, Formatting.Indented)
                };




                var data = await _appointmentService.SaveApiActivityLog(apiActivityLogs);
                #endregion


                return Json(departmentResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PullApiDoctor()
        {

            string doctorsurl = "https://Applink.evercarebd.com:8018/api/Employee/Ehd";
            string doctors2url = "https://Applink.evercarebd.com:8018/api/Employee/Ehc";




            var token = await _botService.GetActiveToken();

            string bearerToken = token.token;

            try
            {
                var uniqueKey = DateTime.Now.Ticks.ToString();




                #region ApiDoctor
                var doctors = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctorsurl, bearerToken);
                var doctors2 = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctors2url, bearerToken);

                var doctorsData = new List<ApiDoctorData>();


                // Process doctors from EHD
                foreach (var item in doctors)
                {
                    doctorsData.Add(new ApiDoctorData
                    {
                        Id = 0,
                        code = item.code,
                        doctorId = item.id,
                        name = item.name,
                        specialization = item.specialization,
                        department = item.department,
                        gender = item.gender,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });


                }

                // Process doctors from EHC
                foreach (var item in doctors2)
                {
                    doctorsData.Add(new ApiDoctorData
                    {
                        Id = 0,
                        doctorId = item.id,
                        code = item.code,
                        name = item.name,
                        specialization = item.specialization,
                        department = item.department,
                        gender = item.gender,
                        date = DateTime.Now,
                        facility = item.facility,
                        uniqueKey = uniqueKey
                    });


                }


                var doctorResult = await _botService.SaveApiDoctorData(doctorsData);
                #endregion

                #region apiActivityLog

                var username = User.Identity.Name;

                var apiActivityLogs = new Models.ApiModelData.ApiActivityLog
                {
                    Id = 0,
                    type = "Pull",
                    DateTime = DateTime.Now,
                    createBy = username,
                    uniqueKey = uniqueKey,
                    jsonResponse = " Doctor: " + JsonConvert.SerializeObject(doctorsData),
                };




                var data = await _appointmentService.SaveApiActivityLog(apiActivityLogs);
                #endregion






                return Json(doctorResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PullApiSpecialization()
        {

            string specialisationsurl = "https://Applink.evercarebd.com:8018/api/SpecialisationMaster";



            var token = await _botService.GetActiveToken();

            string bearerToken = token.token;

            try
            {
                var uniqueKey = DateTime.Now.Ticks.ToString();




                #region ApiSpecialisation
                var specialisation = await ApiCall.GetApiResponseAsync<List<SpecialisationVm>>(specialisationsurl, bearerToken);

                var specialisationData = new List<ApiSpecialisationData>();

                foreach (var item in specialisation)
                {
                    specialisationData.Add(new ApiSpecialisationData
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        uniqueKey = uniqueKey
                    });
                }

                var specialisationResult = await _botService.SaveApiSpecialisationData(specialisationData);
                #endregion


                #region apiActivityLog

                var username = User.Identity.Name;

                var apiActivityLogs = new Models.ApiModelData.ApiActivityLog
                {
                    Id = 0,
                    type = "Pull",
                    DateTime = DateTime.Now,
                    createBy = username,
                    uniqueKey = uniqueKey,
                    jsonResponse = "Specialisation: " + JsonConvert.SerializeObject(specialisationData),
                };




                var data = await _appointmentService.SaveApiActivityLog(apiActivityLogs);
                #endregion


                return Json(specialisationResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(null);
            }
        }



        [HttpGet("/api/AllApiUpdateData")]
        [AllowAnonymous]
        public async Task<IActionResult> AllApiUpdateData()
        {


            try
            {
                var uniqueKey = DateTime.Now.Ticks.ToString();


                #region ApiDepartment
                var departments = await _appointmentService.GetAllApiDeparmentDataList();


                var departmentData = new List<ApiDepartment>();

                foreach (var item in departments)
                {
                    departmentData.Add(new ApiDepartment
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        facility = item.facility,
                        isDelete = item.isDelete,
                        uniqueKey = uniqueKey
                    });
                }



                var departmentResult = await _botService.SaveApiDepartment(departmentData);
                #endregion



                #region ApiDoctor
                var doctors = await _appointmentService.GetAllApiDoctorDataList();

                var doctorsData = new List<ApiDoctor>();


                // Process doctors from EHD
                foreach (var item in doctors)
                {
                    doctorsData.Add(new ApiDoctor
                    {
                        Id = 0,
                        code = item.code,
                        doctorId = item.doctorId,
                        name = item.name,
                        specialization = item.specialization,
                        department = item.department,
                        gender = item.gender,
                        date = DateTime.Now,
                        facility = item.facility,
                        isDelete = item.isDelete,
                        uniqueKey = uniqueKey
                    });


                }



                var doctorResult = await _botService.SaveApiDoctor(doctorsData);
                #endregion





                #region ApiSpecialisation
                var specialisation = await _appointmentService.GetAllApiSpecialisationDataList();

                var specialisationData = new List<ApiSpecialisation>();

                foreach (var item in specialisation)
                {
                    specialisationData.Add(new ApiSpecialisation
                    {
                        Id = 0,
                        code = item.code,
                        name = item.name,
                        date = DateTime.Now,
                        isDelete = item.isDelete,
                        uniqueKey = uniqueKey
                    });
                }

                var specialisationResult = await _botService.SaveApiSpecialisation(specialisationData);
                #endregion

                var combinedResult = new UpdateResult
                {
                    DepartmentResult = departmentResult,
                    DoctorResult = doctorResult,
                    SpecialisationResult = specialisationResult,
                    //SlotResult = slotResult,
                };
                var model = await _botService.GetAllMasterData();


                #region apiActivityLog

                var username = User.Identity.Name;

                var apiActivityLogs = new Models.ApiModelData.ApiActivityLog
                {
                    Id = 0,
                    type = "Process",
                    DateTime = DateTime.Now,
                    createBy = username,
                    uniqueKey = uniqueKey,
                    jsonResponse = "Department: " + JsonConvert.SerializeObject(departmentData, Formatting.Indented)
                    + ", Doctor: " + JsonConvert.SerializeObject(doctorsData) + ", Specialisation: " + JsonConvert.SerializeObject(specialisationData),
                };




                var data = await _appointmentService.SaveApiActivityLog(apiActivityLogs);
                #endregion



                return Json(combinedResult);

                //return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(null);
            }
        }
        #endregion

        #region Dashboard

        [HttpGet]
        public async Task<IActionResult> GetAllVisitorInformationWithStatusForPublic(string botKey)
        {
            var username = User.Identity.Name;


            var model = await _appointmentService.GetDepartmentInforamtionWithStatus(botKey);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApiInsertData()
        {

            var model = await _appointmentService.GetAllApiInsertDataList();

            return Json(model);
        }
        #endregion

        #region Api EvercareTokens

        [HttpGet]
        public async Task<IActionResult> ApiToken()
        {
            var model = new EvercareTokenVm
            {
                evercareToken = await _appointmentService.GetActiveTokenList()
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveApiToken(EvercareToken model)
        {

            var data = await _appointmentService.SaveEvercareToken(model);

            return Json(data);
        }
        #endregion


        #region Api  Activity Logs

        [HttpGet]
        public async Task<IActionResult> ApiActivityLog()
        {
            var model = new ApiActivityLogVm
            {
                apiActivityLogs = await _appointmentService.GetApiActivityLog()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveApiActivityLog(Models.ApiModelData.ApiActivityLog model)
        {

            var data = await _appointmentService.SaveApiActivityLog(model);

            return Json(data);
        }
        #endregion
    }
}
