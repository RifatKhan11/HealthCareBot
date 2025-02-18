using System;
using System.Collections.Generic;
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
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Microsoft.Extensions.Configuration;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class KnowledgeController : Controller
    {
        private readonly IKnowledgeService knowledgeService;
        private readonly IFacebookService facebookService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IquestionCategoryService questionCategoryService;
        private readonly IMenuService menuService;
        private readonly IDoctorInfoService _doctorInfoService;
        private readonly IBotService _botService;
        private readonly IAPIFunction smsAPI;
        private readonly IConfiguration _configuration;

        public KnowledgeController(IKnowledgeService knowledgeService, IBotService _botService,IDoctorInfoService doctorInfoService, IMenuService menuService, IFacebookService facebookService, IquestionCategoryService questionCategoryService, UserManager<ApplicationUser> userManager, IAPIFunction smsAPI, IConfiguration _configuration)
        {
            this.knowledgeService = knowledgeService;
            this.facebookService = facebookService;
            this.questionCategoryService = questionCategoryService;
            this.menuService = menuService;
            _userManager = userManager;
            _doctorInfoService = doctorInfoService;
            this._botService = _botService;
            this.smsAPI = smsAPI;
            this._configuration = _configuration;
        }


        // GET: Knowledge
        // GET: Knowledge/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            int FbPageId = 2;
            KnowledgeViewModel model = new KnowledgeViewModel
            {
                facebookPageId = FbPageId,
                keyWordQuesAns = await knowledgeService.GetAllKnowledgebyPageId(FbPageId),
                questionCategories=await questionCategoryService.GetAllquestionCAtegorybyFacebookId(FbPageId)
            };

            return View(model);
        }

        // POST: Knowledge/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(KnowledgeViewModel model)
        {
            //return Json(model);

            if (!ModelState.IsValid) // If Validation fail.
            {
                model.keyWordQuesAns = await knowledgeService.GetAllKnowledgebyPageId(model.facebookPageId);
                return View(model);
            }

            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            Guid newGuid = Guid.NewGuid();

            model.question = DataFilter.FilterUserString(model.question);
            model.answer = DataFilter.FilterUserString(model.answer);

            KeyWordQuesAns keyWordQuesAns = new KeyWordQuesAns // New Item To Save.
            {
                Id = model.Id,
                question = model.question,
                answer = model.answer,
                more = model.more,
                facebookPageId = 2,
                questionCategoryId = model.questioncategoryId,
                priority = model.priority,
                IsLoop = 1,
                isQuestion = model.isquestion,
                type = model.type,
                status = model.status,
                questionOrder = model.questionOrder,
                keyWord = model.keyword,
                questionKey = model.questionkey == null ? newGuid.ToString().ToUpper() : model.questionkey,
                keyWordQuesAnsId = model.keyWordQuesAnsId,
                botKey = botInfo.botKey,
                nodes = string.Join(",", model.nodes),
                nodeName = model.nodeName,
                branchInfoId = botInfo.branchInfoId,
            };


            await knowledgeService.SaveKnowledge(keyWordQuesAns); // Saving data In Here.

            return RedirectToAction(nameof(Index));
        }

        // GET: Knowledge/KnowledgeCreate

        [HttpGet]
        public async Task<IActionResult> KnowledgeCreate()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            ViewBag.botKey = botInfo.botKey;

            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            KnowledgeViewModel model = new KnowledgeViewModel
            {
                facebookPageId = FbPageId,
                keyWordQuesAns = await knowledgeService.GetKeyWordQuesAns(),
                questionCategories = await questionCategoryService.GetAllquestionCAtegorybyFacebookId(FbPageId)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateKnowledge(KnowledgeViewModel model)
        {
            var keyWordQuesAns = new KeyWordQuesAns
            {
                Id = model.Id,
                question = model.question,
                answer = model.answer,
                facebookPageId = model.facebookPageId,
                questionCategoryId = model.questioncategoryId,
                priority = model.priority,
                keyWordQuesAnsId = model.keyWordQuesAnsId,
                questionKey = model.questionkey,
                type = model.type,
                status = model.status,
                isQuestion = model.isquestion,
                nodes = string.Join(",", model.nodes)
            };

            await knowledgeService.SaveKnowledge(keyWordQuesAns);

            return RedirectToAction(nameof(KnowledgeCreate));
        }
        [HttpPost]
        public async Task<IActionResult> KnowledgeDelete(KnowledgeViewModel model)
        {
            if (model.Id != 0)
                await knowledgeService.KnowledgeDeleteById(model.Id);
            return RedirectToAction(nameof(KnowledgeCreate));
        }

        [HttpGet]
        public IActionResult GenerateQuestionKey()
        {
            var data = Guid.NewGuid().ToString().ToUpper();

            return Json(data);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(KnowledgeViewModel model)
        {
            if (model.Id != 0)
                await knowledgeService.DeleteKnowledgeById(model.Id);
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> getdatabyId(int Id)
        {
            var data = await menuService.GetMenusbyId(Id);

            return Json(data);
        }
        [AllowAnonymous]
        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> getDoctordatabyId(int Id)
        {
            var data = await _doctorInfoService.GetDoctorInfobyid(Id);

            return Json(data);
        }
        //[AllowAnonymous]
        //[Route("{combineId}/{msg}/{index}")]
        //[HttpGet]
        //public async Task<IActionResult> getDatabyFiltering(string combineId,string msg,int index)
        //{
        //    var data = await _doctorInfoService.GetDoctorInfobyid(Id);

        //    return Json(data);
        //}
        // [Route("global/api//GetquesrionbycatId/{id}")]
        [AllowAnonymous]
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetquesrionbycatId(int id)
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            return Json(await knowledgeService.GetAllKnowledgebyCatId(id,FbPageId));
        }
        [AllowAnonymous]
        //[Route]
        [HttpGet]
        public async Task<IActionResult> Getquesrion()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            return Json(await knowledgeService.GetAllKnowledgebyPageId(FbPageId));
        }







        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveInputGroup([FromForm] InputGroupViewModel model)
        {
            var botInfo = await _botService.GetChatBotInfoByBotKey(model.botKey[0]);

            for (int i = 0; i < model.parameterName.Length; i++)
            {
                var data = new ServiceFlow
                {
                    Id = Guid.NewGuid().ToString(),
                    DateTime = DateTime.Now,
                    InfoType = "start",
                    ServiceCode = "Pre-Defined Question",
                    StepNo = 1,
                    Attempt = 0,
                    botKey = model.botKey[0],
                    connectionId = model.connectionId[0],
                    status = 1,
                    answerText = model.parameterName[i] == "Phone" ? model.numberCode + model.valueText[i] : model.valueText[i],
                    questionText = model.parameterName[i],
                    MenuId = 0,
                    keyWordQuesAnsId = null,
                    branchInfoId = botInfo.ApplicationUser?.branchId
                };

                await _botService.SaveServiceFlow(data);
            }

            var nextNodeId = await _botService.SendOTPByNodeId(model.botKey[0], model.connectionId[0], model.nextNodeId[0]);

            var result = new List<string>();

            //Doctor Cards for Reschedule
            if (nextNodeId.ToString() == "448")
            {
                result = await _botService.GetRawMessageByNodeId(nextNodeId, model.connectionId[0], model.botKey[0]);
                //return Json(data);

                //var data = await _botService.SendNextMessageByNodeId(model.botKey[0], model.connectionId[0], nextNodeId);

                //if (data.Count > 0)
                //{
                //    foreach (var item in data)
                //    {
                //        result.Add(item.Replace("{\"msg\" : \"", "").Replace("\"}", ""));
                //    }
                //}

                //return Json(data);
            }

            var otpCode = await _botService.GetLastOTPByConnectionId(model.connectionId[0]);




            //if (nextNodeId > 0)
            //{
            //    var nextMessage = await _botService.SendNextMessageByNextNodeId(model.botKey[0], model.connectionId[0], model.nextNodeId[0]);
            //    foreach (var item in nextMessage)
            //    {
            //        result.Add(item.Replace("{\"msg\" : \"", "").Replace("\"}", ""));
            //    }
            //}

            var phoneNumber = await _botService.GetPhoneByConnectionId(model.connectionId[0]);

            var quesAns = await _botService.GetKeywordQuesById(model.nextNodeId[0]);

            if (quesAns.smsOtp == 1)
            {
                var otpmsg = "<p>Please enter otp received on your number</p>";
                var otpHtml = "";
                if (_configuration["Project:isLive"] == "YES")
                {
                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                    //smsAPI.Single_Sms(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                }
                else
                {
                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                    ////await this.SendSMSAsync(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                }

                result.Add(otpmsg);
                result.Add(otpHtml);
            }

            //if (quesAns.nodes == "447")
            //{
            //    var otpHtml = "<div class=\"bluedualcard drdtal drppt \" style=\"margin-bottom: 5px; width: 100%; flex: 0 0 100%; max-width: 320px; min-height: 150px;\"><div class=\"drdtal-top\" style=\"margin-top: 10px;\"><div class=\" drdtal-topleft\"><img src=\"/static/doctor-male.png\" alt=\"\" width=\"60px\"></div><div class=\"drdtal-topright\"><p>  Appointment with</p><p class=\"drname\">Dr. LUTFUL AZIZ PAIN CLINIC CONSULTANT  </p><span style=\"font-size: 11px; width: 100%; float: left; margin-bottom: 2px;\">Acute Pain Service (Aps)</span><small style=\"color: rgb(255, 255, 255); margin-bottom: 2px;\"></small><p style=\"font-size: 13px; margin-top: 5px; float: left; margin-right: 3px;\"><i class=\"material-icons\" style=\"font-size: 13px; margin-top: 2px; float: left; margin-right: 3px;\">schedule</i>11:00 AM, 16-01-2024 </p></div></div><a class=\"btn btn-btm btnbooking\" style=\"background: rgb(127, 62, 152);\"><div class=\"yellowdate\" style=\"width: 50%; float: left; padding: 9px 0px; -webkit-box-flex: 1; flex: 1 1 auto; text-align: center; display: block; border-right: 1px solid rgb(84, 34, 103); font-size: 16px; background: rgb(233, 30, 99);\">  Cancel</div><div class=\"yellow_time\" style=\"padding: 9px 0px; width: 50%; float: left; -webkit-box-flex: 1; flex: 1 1 auto; font-size: 16px;\">   Reschedule</div></a></div>";
            //    result.Add(otpHtml);
            //}



            if (true)
            {

            }


            //foreach (var item in nextMessage)
            //{
            //    result.Add(item.Replace("{\"msg\" : \"", "").Replace("\"}", ""));
            //}
            return Json(result);
        }


        public async Task<IActionResult> AllAppointmentList()
        {
            var username = User.Identity.Name;

            var botInfo = await _botService.GetBotInfoByUserName(username);

            var data = new AppointMentManagementVm
            {
                AppoinmentInfos = await _botService.GetAllAppointment(botInfo.botKey)
            };

            return View(data);
        }



        public async Task<IActionResult> OngoingAppointmentList()
        {
            var username = User.Identity.Name;

            var botInfo = await _botService.GetBotInfoByUserName(username);

            var data = new AppointMentManagementVm
            {
                AppoinmentInfos = await _botService.GetOngoingAppointment(botInfo.botKey)
            };

            return View(data);
        }

        public async Task<IActionResult> ConfirmAppointmentList()
        {
            var username = User.Identity.Name;

            var botInfo = await _botService.GetBotInfoByUserName(username);

            var data = new AppointMentManagementVm
            {
                AppoinmentInfos = await _botService.GetConfirmedAppointment(botInfo.botKey)
            };

            return View(data);
        }

        public async Task<IActionResult> AddRescheduleDoctor(int scheduleId)
        {
            var data = await _botService.AddRescheduleDoctor(scheduleId);

            return Ok(data);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAndValidateOTP(string otp, string uhid)
        {
            var data = await _botService.CheckAndValidateOTP(otp);

            var saveUser = await _botService.SaveUserByOTP(otp, uhid);

            var result = new List<string>();

            if (saveUser == "user exist")
            {
                //result.Add("You are already a member!");
            }
            if (saveUser == "schedule not found")
            {
                var conId = await _botService.GetConnectionIdByOTP(otp);
                var botkey = await _botService.GetBotKeyByOTP(otp);

                var messages = await _botService.GetRawMessageByNodeId(501, conId, botkey);

                result.AddRange(messages);

            }
            else
            {
                foreach (var item in data)
                {
                    result.Add(item.Replace("{\"msg\" : \"", "").Replace("\"}", ""));
                }
            }


            return Json(result);
        }
        
        
        
        [HttpGet]
        public async Task<IActionResult> GetGroupMasters()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            InputGroupMasterViewModel model = new InputGroupMasterViewModel
            {
                Menus = await _botService.GetAllMenusByBotKey(botInfo.botKey)
            };

            return View(model);
        }

        [HttpGet("/global/api/GetAllGroupMasters")]
        public async Task<IActionResult> GetAllGroupMasters()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            var model = await knowledgeService.GroupMasters(botInfo.botKey);

            return Json(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateGroupMasters(InputGroupMasterViewModel model)
        {
            var userName= HttpContext.User.Identity.Name;
            var botInfo = await _botService.GetBotInfoByUserName(userName);
            var InputGroupMasters = new InputGroupMaster
            {
                Id = model.Id,
                name = model.Name,
                status = model.Status,
                menuId = model.MenuId,
                botKey= botInfo.botKey

            };
            await knowledgeService.SaveGroupMaster(InputGroupMasters);
            return RedirectToAction(nameof(GetGroupMasters));
        }

        [HttpGet("/global/api/DeleteGroupMaster")]
        public async Task<IActionResult> DeleteGroupMaster(int masterId)
        {
            var data = await knowledgeService.DeleteGroupMasterById(masterId);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupDetails(int masId)
        {

            var data = await knowledgeService.GetInputGrpDetrailsByMasterId(masId);

            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroupDetails( InputGroupMasterViewModel model)
        {

            var data = new InputGroupDetail
            {
                Id = model.Id,
                inputName = model.InputName,
                parameterName=model.ParaMeter,
                inputType=model.InputType,
                placeHolder=model.PlaceHolder,
                status=model.Status,
                masterId=model.masterId




            };
            var result = await knowledgeService.SaveGroupDetails(data);
            return Json(result);
        }

        [HttpGet("/global/api/GetInputDetailByMasterId")]
        public async Task<IActionResult> GetInputDetailByMasterId(int masterId)
        {
            var data = await knowledgeService.GroupDetails(masterId);

            return Json(data);

        }
        [HttpGet("/global/api/DeleteGroupDetails")]
        public async Task<IActionResult> DeleteGroupDetails(int DetailsId)
        {
            var data = await knowledgeService.DeleteGroupDetailsById(DetailsId);
            return Json(data);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBotKnowledge(string botKey)
        {
            //var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            BotKnowledgeViewModel model = new BotKnowledgeViewModel
            {
                BotKnowledgeS = await _botService.GetKnowledgeByBotKey(botKey),
                KeyWordQuesAnsS = await _botService.GetAllBotQuestionsByBotKey(botKey),
            };

            return View(model);
        }

        [HttpPost("/api/Knowledge/CreateBotKnowledge")]
        public async Task<IActionResult> CreateBotKnowledge([FromForm] BotKnowledgeViewModel model)
        {
            var userName = HttpContext.User.Identity.Name;
            var botInfo = await _botService.GetBotInfoByUserName(userName);
            var data = new BotKnowledge
            {
                Id = model.Id,
                question = model.Questiontext,
                textReply = model.TextReply,
                keyWordQuesAnsId = model.KeyWordQuesAnsId,
                status = model.Status,
                botKey = botInfo.botKey
            };
            var result = await knowledgeService.SaveBotKnowledge(data);

            return Json(result);
        }
        [HttpGet("/global/api/GetAllBotKnowledge")]
        public async Task<IActionResult> GetAllBotKnowledge()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            var model = await knowledgeService.BotKnowledge(botInfo.botKey);

            return Json(model);
        }


        [HttpGet("/global/api/GetKeywordQuestionAnsById")]
        public async Task<IActionResult> GetKeywordQuestionAnsById(int? id)
        {
            var data = await _botService.GetKeywordQuestionAnsById(id);

            return Json(data);
        }
        [HttpGet("/global/api/DeleteBotKnowledge")]
        public async Task<IActionResult> DeleteBotKnowledge(int bkId)
        {
            var data = await knowledgeService.DeleteBotKnowledgeById(bkId);
            return Json(data);
        }
        [HttpGet("/global/api/GetAllKeywordQuesAns")]
        public async Task<IActionResult> GetAllKeywordQuesAns()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            var data = await _botService.GetAllKeywordQuesAns(botInfo.botKey);

            return Json(data);
        }



        public async Task<IActionResult> KeywordQuestions()
        {
            var botInfo = await _botService.GetBotInfoByUserName(User.Identity.Name);

            var model = new BotKnowledgeViewModel
            {
                KeyWordQuesAnsS = await _botService.GetAllKeywordQuesAns(botInfo.botKey),
                DepartmentInfos = await _botService.GetAllDepartmentInfo(botInfo.botKey),
                DoctorInfos = await _botService.GetALlDoctorInfo(botInfo.botKey)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateKeywordQues(BotKnowledgeViewModel model)
        {
            var data = await _botService.SaveOrUpdateKeywordQues(model);

            return Json(data);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddMessageLogByQuestionId(int id)
        {
            var data = await _botService.AddMessageLogByQuestionId(id);

            return Json(data);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetNextMessageByNodeId(int nextNodeId, string connectionId, string botKey)
        {
            try
            {
                var result = await _botService.GetRawMessageByNodeId(nextNodeId, connectionId, botKey);

                return Json(result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SaveDoctorNameInServiceFlow(int nextNodeId, string connectionId, string botKey, string doctorName, int scheduleId)
        {
            try
            {
                var result = await _botService.SaveDoctorNameInServiceFlow(nextNodeId, connectionId, botKey, doctorName, scheduleId);

                return Json(result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CancelAppointmentById(int scheduleId, string connectionId, string botKey)
        {
            try
            {
                var result = await _botService.CancelAppointmentById(scheduleId, connectionId, botKey);

                return Json(result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorByDoctorApiId(int id, string date)
        {
            var data = await _botService.GetDoctorApiId(id, date);

            return Json(data);
        }
    }
}