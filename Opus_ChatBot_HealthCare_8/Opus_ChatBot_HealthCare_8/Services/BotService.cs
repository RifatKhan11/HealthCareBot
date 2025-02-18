using System.Collections.Immutable;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.ApiModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using static Opus_ChatBot_HealthCare_8.Controllers.HomeController;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class BotService : IBotService
    {
        private readonly IServiceFlowService serviceFlowService;
        private readonly IBotFlowService botFlowService;
        private readonly IUserInfoService userInfoService;
        private readonly IOTPService oTPService;
        private readonly IPassportInfoService passportInfoService;
        private readonly IBankInfoService bankInfoService;
        private readonly IFacebookService facebookService;
        private readonly IKeyWordQuesService keyWordQuesService;
        private readonly IKnowledgeService knowledgeService;
        private readonly IMenuService menuService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly IQueriesService queriesService;
        private readonly IquestionCategoryService iquestionCategoryService;
        private readonly IDoctorInfoService _doctorInfoService;
        private readonly ApplicationDbContext _context;
        private readonly SmtpClient smtpClient;
        private readonly IAPIFunction smsAPI;
        private readonly IConfiguration _configuration;
        private readonly IDapper _dapper;

        public BotService(IServiceFlowService serviceFlowService, ApplicationDbContext _context, IquestionCategoryService iquestionCategoryService, IQueriesService queriesService, IQuestionReplayService questionReplayService, IMenuService menuService, IKnowledgeService knowledgeService, IBotFlowService botFlowService, IUserInfoService userInfoService, IOTPService oTPService, IPassportInfoService passportInfoService
            , IBankInfoService bankInfoService, IFacebookService facebookService, IKeyWordQuesService keyWordQuesService, IDoctorInfoService doctorInfoService, IAPIFunction smsAPI, IConfiguration _configuration, IDapper dapper)
        {
            this.serviceFlowService = serviceFlowService;
            this.botFlowService = botFlowService;
            this.userInfoService = userInfoService;
            this.oTPService = oTPService;
            this.passportInfoService = passportInfoService;
            this.bankInfoService = bankInfoService;
            this._context = _context;
            this.facebookService = facebookService;
            this.keyWordQuesService = keyWordQuesService;
            this.knowledgeService = knowledgeService;
            this.menuService = menuService;
            this.questionReplayService = questionReplayService;
            this.queriesService = queriesService;
            this.iquestionCategoryService = iquestionCategoryService;
            _doctorInfoService = doctorInfoService;
            smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("opustestmail@gmail.com", "Opus1234"),
                EnableSsl = true
            };
            this.smsAPI = smsAPI;
            this._configuration = _configuration;

            this._dapper = dapper;
        }




        public async Task<List<string>> CustomMessageGenerator(string senderId, string pageId, string message, string postback, string userId, string botKey, string connectionId, string messagejson, Models.BotModels.ConnectionInfo connectionInfo)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            message = message.Trim();


            dynamic Data = (dynamic)null;

            Data = JsonConvert.DeserializeObject(messagejson);

            int questionId = 0;
            int slotId = 0;

            DateTime? appointDate = null;

            if (messagejson.IndexOf("qId") > 0)
            {
                questionId = Data.qId;
            }
            else if (messagejson.IndexOf("dateTxt") > 0)
            {
                appointDate = Data.dateTxt;

                var sf = new ServiceFlow
                {
                    Id = Guid.NewGuid().ToString(),
                    questionText = "AppointDate",
                    answerText = appointDate.ToString(),
                    botKey = botKey,
                    connectionId = connectionId,
                    InfoType = "start",
                    ServiceCode = "Pre-Defined Question",
                    StepNo = 1,
                    Attempt = 0,
                    status = 1,
                    MenuId = 0,
                    DateTime = DateTime.Now,
                    branchInfoId = user.branchId
                };

                _context.ServiceFlows.Add(sf);
                await _context.SaveChangesAsync();
            }
            else if (messagejson.IndexOf("slotId") > 0)
            {
                slotId = (int)Data.slotId;

                var slot = await _context.TimePeriods.Where(x => x.Id == slotId).AsNoTracking().FirstOrDefaultAsync();

                var sf = new ServiceFlow
                {
                    Id = Guid.NewGuid().ToString(),
                    questionText = "TimeSlot",
                    answerText = slot.timeSlot.ToString(),
                    botKey = botKey,
                    connectionId = connectionId,
                    InfoType = "start",
                    ServiceCode = "Pre-Defined Question",
                    StepNo = 1,
                    Attempt = 0,
                    status = 1,
                    MenuId = 0,
                    DateTime = DateTime.Now,
                    branchInfoId = user.branchId
                };

                _context.ServiceFlows.Add(sf);
                await _context.SaveChangesAsync();
            }



            int falsestep = 0;
            string combinedId = pageId + senderId;
            int FFbPageId = await facebookService.GetFacebookpageId(pageId);
            List<string> messages = new List<string>();
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);


            var exceptMessages = new List<string>() { "menu", "Doctor search by department", "Search by doctor", "Doctor search by specializations", };

            //Custom Message Free Text Start
            //var wrapperDetails = await _context.ConnectionInfos.Where(x => x.userId == userId).AsNoTracking().LastOrDefaultAsync();

            var questionsByKeyword = await GetQuestions(postback, message, botKey, connectionId, questionId, connectionInfo);

            if (!exceptMessages.Contains(message))
            {
                if (connectionInfo.isTextMsg == 0)
                {
                    messages = await GetResponseByTypingText(message, connectionId, botKey, userId);

                    if (messages.Count() > 0 && connectionInfo.wrapperDetailsId == 1 && connectionInfo.isTextMsg == 0)
                    {
                        connectionInfo.isTextMsg = 1;
                        _context.ConnectionInfos.Update(connectionInfo);
                        await _context.SaveChangesAsync();

                        postback = "Typing Text Department";
                    }
                    else if (messages.Count() > 0 && connectionInfo.wrapperDetails?.heading == "Book Appointment with Specializations" && connectionInfo.isTextMsg == 0)
                    {
                        connectionInfo.isTextMsg = 1;
                        _context.ConnectionInfos.Update(connectionInfo);
                        await _context.SaveChangesAsync();

                        postback = "Typing Text Specializations";
                    }
                    else if (messages.Count() > 0 && connectionInfo.wrapperDetailsId == 2 && connectionInfo.isTextMsg == 0)
                    {
                        connectionInfo.isTextMsg = 1;
                        _context.ConnectionInfos.Update(connectionInfo);
                        await _context.SaveChangesAsync();
                        postback = "Typing Text Doctor";
                    }
                    else
                    {
                        if (connectionInfo != null)
                        {
                            connectionInfo.isTextMsg = 1;
                            _context.ConnectionInfos.Update(connectionInfo);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            else
            {
                if (connectionInfo != null)
                {
                    if (exceptMessages.Contains(message))
                    {
                        connectionInfo.isTextMsg = 0;
                    }
                    else
                    {
                        connectionInfo.isTextMsg = 1;
                    }
                    _context.ConnectionInfos.Update(connectionInfo);
                    await _context.SaveChangesAsync();
                }

            }



            //if (questionsByKeyword.Count() == 0 || (questionsByKeyword.Count() > 0 && questionsByKeyword[0].Id == 173) || (questionsByKeyword.Count() > 0 && questionsByKeyword[0].Id == 164))
            //{
            //    if (wrapperDetails?.wrapperDetailsId != 1 && wrapperDetails?.wrapperDetailsId != 2)
            //    {
            //        messages = await GetResponseByTypingText(message, connectionId, botKey, userId);

            //        if (messages.Count() > 0 && wrapperDetails.wrapperDetailsId == 1)
            //        {
            //            postback = "Typing Text Department";
            //        }
            //        else if (messages.Count() > 0 && wrapperDetails.wrapperDetailsId == 2)
            //        {
            //            postback = "Typing Text Doctor";
            //        }
            //        else
            //        {

            //        }
            //    }
            //}

            bool hasSlot = true;



            var docId2 = await _context.DoctorInfos.Where(x => x.name == message && x.branchInfoId == user.branchId).AsNoTracking().LastOrDefaultAsync();


            DateTime currentDate = DateTime.Now;
            #region Slots Api Call
            if (docId2 != null && docId2?.ApiDoctorId != null && currentDate != null)
            {
                var doctorId = docId2.ApiDoctorId;
                var date = Convert.ToDateTime(currentDate).ToString("yyyy-MM-dd");
                var branch = docId2.branchInfoId == 1 ? "EHD" : "EHC";


                var token = await GetActiveToken();
                string bearerToken = token.token;
                string baseDoctorSlots2Url = "https://Applink.evercarebd.com:8018/api/DoctorSlot";

                var doctorSlotUrl2 = $"{baseDoctorSlots2Url}/{branch}/{doctorId}/{date}";
                var doctorSlot2 = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(doctorSlotUrl2, bearerToken);

                if (doctorSlot2.Count == 1)
                {
                    hasSlot = false;
                    var qdata1 = await _context.MessageLogs.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).LastOrDefaultAsync();

                    qdata1.nextNodeId = user.branchId == 1 ? 501 : 1984;
                    _context.Entry(qdata1).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    if (user.branchId == 1)
                    {
                        //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.Id == 501).AsNoTracking().ToListAsync();
                        questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.questionKey == "9935BCAA-B80B-48B9-9BBF-47524E184EB2").AsNoTracking().ToListAsync();

                    }
                    else
                    {
                        //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.Id == 1984).AsNoTracking().ToListAsync();
                        questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.questionKey == "77DECB27-3A4C-4658-BC85-96BCD69BDAC8").AsNoTracking().ToListAsync();

                    }
                }
                else
                {
                    var qdata1 = await _context.MessageLogs.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).LastOrDefaultAsync();

                    //qdata1.nextNodeId = user.branchId == 1 ? 497 : 1982;
                    //qdata1.nextNodeId = user.branchId == 1 ? 390 : 1885;
                    qdata1.nextNodeId = user.branchId == 1 ? 198 : 1858;
                    _context.Entry(qdata1).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var questionkey = await _context.keyWordQuesAns.Where(x => x.Id == qdata1.nextNodeId).Select(x => x.questionKey).AsNoTracking().FirstOrDefaultAsync();
                    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.questionKey == questionkey).AsNoTracking().ToListAsync();




                    int stepNo = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.status == 0 && x.branchInfoId == user.branchId).AsNoTracking().OrderByDescending(x => x.StepNo).Select(x => x.StepNo).FirstOrDefaultAsync();




                    var sflow = new ServiceFlow
                    {
                        Id = Guid.NewGuid().ToString(),
                        InfoType = "start",
                        ServiceCode = "Pre-Defined Question",
                        connectionId = connectionId,
                        DateTime = DateTime.Now,
                        Attempt = 0,
                        StepNo = stepNo + 1,
                        botKey = botKey,
                        status = 0,
                        questionText = "Enter Dr. Name", //this is question for now
                        answerText = message,
                        MenuId = 0,
                        keyWordQuesAnsId = 374,
                        branchInfoId = user.branchId
                    };


                    try
                    {
                        _context.ServiceFlows.Add(sflow);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {

                    }



                }

            }



            #endregion




            if (connectionInfo?.isTextMsg == 1 && hasSlot)
            {
                foreach (var item in questionsByKeyword)
                {
                    //Log Message
                    var msglog = new MessageLog
                    {
                        Id = 0,
                        botKey = botKey,
                        connectionId = connectionId,
                        message = message,
                        menuId = null,
                        Type = message == "menu" ? "Menu" : "text",
                        rawMessage = message,
                        entryDate = DateTime.Now,
                        KeyWordQuesAnsId = null,
                        nextNodeId = item.nextNodeId,
                        branchInfoId = user.branchId
                    };

                    _context.MessageLogs.Add(msglog);
                    await _context.SaveChangesAsync();




                    #region Insert Into ServiceFlow

                    string question = null;

                    if (await _context.DoctorInfos.Where(x => x.name == message && x.branchInfoId == user.branchId).AsNoTracking().LastOrDefaultAsync() != null)
                    {
                        question = "Enter Dr. Name";


                        //var qdata1 = await _context.MessageLogs.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).LastOrDefaultAsync();

                        //qdata1.nextNodeId = user.branchId == 1 ? 497 : 1982;
                        //_context.Entry(qdata1).State = EntityState.Modified;
                        //await _context.SaveChangesAsync();
                    }
                    if (await _context.DoctorSpecializations.Where(x => x.name == message.ToLower().Trim() && x.branchInfoId == user.branchId).AsNoTracking().LastOrDefaultAsync() != null)
                    {
                        question = "Please enter specializations name";
                    }
                    //else if (await _context.DepartmentInfos.Where(x => x.departmentName == message && x.branchInfoId == user.branchId).AsNoTracking().LastOrDefaultAsync() != null)
                    //{
                    //    question = "Please enter department name";
                    //}

                    else if (message == "Male" || message == "Female")
                    {
                        question = "Gender";
                    }

                    if (item.isQuestion == 1 || item.Id == 496 || item.Id == 1981)
                    //if (item.isQuestion == 1)
                    {
                        int stepNo = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.status == 0 && x.branchInfoId == user.branchId).AsNoTracking().OrderByDescending(x => x.StepNo).Select(x => x.StepNo).FirstOrDefaultAsync();

                        int? qId = await _context.keyWordQuesAns.Where(x => x.branchInfoId == user.branchId && x.isDelete != 1 && x.answer.Replace(@"\r\n", "").Trim().Replace(@"\r\n", "") == item.answer.Replace(@"\r\n", "").Trim().Replace(@"\r\n", "")).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync();




                        var sflow = new ServiceFlow
                        {
                            Id = Guid.NewGuid().ToString(),
                            InfoType = "start",
                            ServiceCode = "Pre-Defined Question",
                            connectionId = connectionId,
                            DateTime = DateTime.Now,
                            Attempt = 0,
                            StepNo = stepNo + 1,
                            botKey = botKey,
                            status = 0,
                            questionText = item.question == "NEXT7DAYS" ? "TimeSlot" : item.question, //this is question for now
                            answerText = message,
                            MenuId = 0,
                            keyWordQuesAnsId = qId == 0 ? null : qId,
                            branchInfoId = user.branchId
                        };

                        sflow.questionText = question != null ? question : sflow.questionText;

                        try
                        {
                            _context.ServiceFlows.Add(sflow);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    #endregion
                }
            }



            //Push to doctor flow by 193
            if (postback == "docSearch" && hasSlot)
            {
                try
                {


                    var qdata1 = await _context.MessageLogs.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).LastOrDefaultAsync();

                    //qdata.nextNodeId = 371;
                    //qdata1.nextNodeId = user.branchId == 1 ? 497 : 1982;
                    // qdata1.nextNodeId = user.branchId == 1 ? 390 : 1885;
                    qdata1.nextNodeId = user.branchId == 1 ? 198 : 1858;

                    _context.Entry(qdata1).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }




            if (postback != null && postback != "")
            {
                if (postback == "docSearch")
                {
                    /*Generate Doctor Cards*/
                    var part3 = "Doctor Not Available";
                    var part0 = "<div id='single-doc-box'><div class='dr-box' style='align-content: center;background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -27px !important;  margin-top: -10px; padding: 10px;  height: 400px'>";

                    var part1 = "";

                    var doctors = new List<DoctorVm>();


                    //doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.Department.departmentName == message && x.branchInfoId == user.branchId).ToListAsync();


                    #region FetchDoctors
                    var allDoctors = await FetchDoctors(botKey);
                    //doctors = allDoctors.Where(x => x.department == message).ToList();
                    doctors = allDoctors.Where(x => x.specialization == message).ToList();


                    #endregion

                    if (doctors.Count == 0)
                    {
                        // doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId).ToListAsync();
                        messages.Add("{\"msg\" : \"" + part3 + "\"}");
                    }
                    else
                    {
                        if (messages.Where(x => x.Contains("Please select the doctor for appointment")).Count() == 0)
                        {
                            messages.Add("{\"msg\" : \"" + "Please select the doctor for appointment" + "\"}");
                        }


                        foreach (var doc in doctors.Take(1).ToList())
                        {
                            var slots = await Fetch7DaysSlot(botKey, doc.id);

                            var dateList = "";
                            foreach (var date in slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct())
                            {
                                var strDate = Convert.ToDateTime(date).ToString("dd MMM");

                                dateList += "<div class='doc-avalday-new' onclick='clickOnAppointDate(this)' data-tooltip='Available'>" + strDate + " <input type='hidden' class='app-date' value='" + Convert.ToDateTime(date).ToString("yyyy/MM/dd") + "' /></div>";
                            }

                            //part1 += "<div id='drBoxApiId_" + doc.id + "' onclick='handleDrCardClick(this)' class='graycard drdtal' style='align-content: center; background: white;border-radius: 6px; box-shadow: 0 0 5px 2px rgba(0,0,0,.2)!important;border-top: 2px solid blue;padding: 10px;'><div class='drdtal-top'><div class='loader'></div><div class='drdtal-topleft' style='width:75px;float:left;padding:5px;-webkit-box-flex:1;position:relative'><span class='spangender'>M</span><img class='doc-pic' src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p class='drname'>" + doc.name + "</p><p style='display: none' id='drApiId_" + doc.id + "' class='drApiId'> " + doc.id + "</p><span class='doc-subhead'>" + doc.department + "</span></div></div><div class='doc-aval'><span class='doc-avaltxt'>Availability</span><div class='doc-avalday' data-tooltip='Available'>m</div><div class='doc-avalday' data-tooltip='Available'>t</div><div class='doc-avalday' data-tooltip='Available'>W</div><div class='doc-avalday' data-tooltip='Available'>T</div><div class='doc-avalday doc-avalday-not' data-tooltip='Not Available'>F</div><div class='doc-avalday' data-tooltip='Available'>S</div><div class='doc-avalday' data-tooltip='Available'>S</div></div><a class='btn btn-btm' href='javascript:void(0);' style='margin-left:50px;margin-top: 10px'>Request Appointment</a></div>";
                            part1 += "<div id='drBoxApiId_" + doc.id + "' onclick='handleDrCardClick(this)' class='graycard drdtal' style='align-content: center; background: white;border-radius: 6px; box-shadow: 0 0 5px 2px rgba(0,0,0,.2)!important;border-top: 2px solid blue;padding: 10px;'><div class='drdtal-top'><div class='loader'></div><div class='drdtal-topleft' style='width:75px;float:left;padding:5px;-webkit-box-flex:1;position:relative'><span class='spangender'>M</span><img class='doc-pic' src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p class='drname'>" + doc.name + "</p><p style='display: none' id='drApiId_" + doc.id + "' class='drApiId'> " + doc.id + "</p><span class='doc-subhead'>" + doc.department + "</span></div></div><div class='doc-aval'><span class='doc-avaltxt'>Availability</span>" + dateList + "</div><a class='btn btn-btm' href='javascript:void(0);' style='margin-left:50px;margin-top: 10px'>Request Appointment</a></div>";
                        }

                        var part2 = "</div><button class='dr-box-left' onclick='drboxLeft(" + doctors.FirstOrDefault()?.id + ")'><</button><button class='dr-box-right' onclick='drboxRight(" + doctors.FirstOrDefault()?.id + ")'>></button></div>";

                        messages.Add("{\"msg\" : \"" + part0 + part1 + part2 + "\"}");
                        /*Generate Doctor Cards*/
                    }


                }

            }
            else
            {
                messages = await SendMessage(questionsByKeyword, botKey, connectionId);
            }



            if (message == "Navigation Menu")
            {
                var menuId = await _context.botRackInfoDetails.Where(y => y.firstMessage == "Navigation Menu").AsNoTracking().Select(y => y.menuId).FirstOrDefaultAsync();
                var menuItems = await _context.Menus.Where(x => x.Id == menuId).AsNoTracking().ToListAsync();

                string btndata = string.Empty;

                foreach (Menu m in menuItems)
                {
                    btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
                }
                messages.Add("{ \"msg\":\"Please Choose One <br>" + btndata + "</div>\"}");
            }

            if (appointDate != null)
            {
                if (user.branchId == 1)
                {
                    messages = await SendNextMessageByNodeId(botKey, connectionId, 212);
                }
                else
                {
                    messages = await SendNextMessageByNodeId(botKey, connectionId, 1870);
                }

            }

            if (slotId > 0)
            {
                if (user.branchId == 1)
                {
                    messages = await SendNextMessageByNodeId(botKey, connectionId, 490);
                }
                else
                {
                    messages = await SendNextMessageByNodeId(botKey, connectionId, 1975);
                }
            }

            //Custom Message Free Text End




            //if (serviceFlow == null)
            //{
            //    falsestep = 1;
            //    ServiceFlow data = new ServiceFlow
            //    {
            //        Id = combinedId,
            //        InfoType = "pasportOrref",
            //        ServiceCode = "passport",
            //        StepNo = 2,
            //        DateTime = DateTime.Now,
            //        Attempt = 0


            //    };
            //    serviceFlow = await serviceFlowService.SaveServiceFlow(data);
            //}



            //Typing Message Start
            if (messages.Count() == 0)
            {
                var data = new UnKnownKeyWordQuestion
                {
                    Id = 0,
                    autoNumber = Guid.NewGuid().ToString(),
                    botKey = botKey,
                    connectionId = connectionId,
                    entryDate = DateTime.Now,
                    question = message,
                    type = message == "menu" ? "menu" : "text"
                };

                _context.unKnownKeyWordQuestions.Add(data);
                await _context.SaveChangesAsync();

                messages = await this.GetResponseByTypingText(message, connectionId, botKey, userId);


            }
            //Typing Message End









            if (serviceFlow != null)
            {
                if (serviceFlow.StepNo == 0)
                {

                    userInfoService.UpdateUserInfo(combinedId, "Passport", postback);
                    messages.Add("{\"msg\" : \"What's your passport number or application reference number?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);

                }

                else if (serviceFlow.StepNo == 2)
                {
                    if (InfoValidation.CheckPassportOrRef(message) == "unknown")
                    {
                        if (falsestep == 1)
                        {
                            messages = await QuesReplayService(senderId, pageId, message, postback, userId, botKey, connectionId);
                        }
                        else
                        {
                            messages.Add(" { \"msg\" : \"What's your passport number or application reference number?\"}");
                        }

                    }
                    else
                    {
                        userInfoService.UpdateUserInfo(combinedId, "Passport", message);


                        PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(message);
                        if (passportInfo == null)
                        {
                            var lstkeyWordQuesAns = await knowledgeService.GetAllKnowledgebyPagebywordfbid(message, FFbPageId);

                            if (lstkeyWordQuesAns.Count() > 0)
                            {
                                messages = await QuesReplayService(senderId, pageId, message, postback, userId, botKey, connectionId);
                            }
                            else
                            {
                                messages = await QuesReplayService(senderId, pageId, message, postback, userId, botKey, connectionId);
                            }


                        }
                        else
                        {
                            messages.Add("{ \"msg\" : \"Hello, " + passportInfo.name + ".\"}");
                            messages.Add("{ \"msg\" : \"What's your date of birth (yyyy-mm-dd)?\"}");
                            serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);

                        }

                    }
                }
                else if (serviceFlow.StepNo == 3)
                {
                    var data = userInfoService.GetuserInfo(combinedId);
                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(data.passport);
                    DateTime dateTime = DateTime.Parse(message);
                    if (passportInfo?.dob != dateTime)
                    {
                        messages.Add("{ \"msg\" : \"Please enter the date correctly (yyyy-mm-dd)?\"}");
                        PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                        {
                            passportNo = passportInfo.passportNo,
                            refNo = passportInfo.refNo,
                            date = DateTime.Now,
                            status = "Entered Invalid Date of Birth"
                        };
                        await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
                    }
                    else
                    {
                        messages.Add("{ \"msg\" : \"When will your passport expire (yyyy-mm-dd)?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);

                    }
                }
                else if (serviceFlow.StepNo == 4)
                {
                    var data1 = userInfoService.GetuserInfo(combinedId);
                    PassportInfo passportInfo1 = await passportInfoService.GetPassportInfoByPasspoertIds(data1.passport);
                    DateTime dateTime = DateTime.Parse(message);
                    if (passportInfo1?.expireDate != dateTime)
                    {
                        messages.Add("{ \"msg\" : \"Please enter the date correctly (yyyy-mm-dd). \"}");
                        PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                        {
                            passportNo = passportInfo1.passportNo,
                            refNo = passportInfo1.refNo,
                            date = DateTime.Now,
                            status = "Entered Invalid Passport Expire Date"
                        };
                        await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
                    }
                    else
                    {
                        var data = userInfoService.GetuserInfo(combinedId);
                        PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(data.passport);
                        string status = passportInfo.status;
                        if (passportInfo.expectedDeliveryDate == null)
                        {
                            if (passportInfo.currentContact.Contains("null"))
                            {
                                if (passportInfo.remarks.Contains("null") || passportInfo.remarks == null)
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Thanks for taking our service.\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }

                                }
                                else
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Remarks :  <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.remarks.Contains("null") || passportInfo.remarks == null)
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }

                                }
                                else
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }

                                }

                            }
                        }
                        else
                        {
                            if (passportInfo.currentContact.Contains("null"))
                            {
                                if (passportInfo.remarks.Contains("null") || passportInfo.remarks == null)
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }

                                }
                                else
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Remarks :<span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");

                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Thanks for taking our service\"}");

                                        }
                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.remarks.Contains("null") || passportInfo.remarks == null)
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }

                                    }

                                }
                                else
                                {
                                    if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks : <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }
                                    else
                                    {
                                        if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Remarks <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                        else
                                        {
                                            messages.Add("{ \"msg\" : \" Your application  reference no :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Reason :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>Remarks: <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>Expected Delivery Date :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>Current Contact  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>Thanks for taking our service\"}");
                                        }
                                    }

                                }

                            }
                        }
                        if (passportInfo.status.Contains("null") || passportInfo.status == null)
                        {

                        }
                        else
                        {


                            string[] ppvalues = new string[] { "a", "b", "c", "d" };
                            ppvalues = passportInfo.status.Split("(");
                            string btndata = string.Empty;

                            var datamenu = await GetData(ppvalues[0], pageId);
                            if (datamenu.Count() > 0)
                            {

                                foreach (Menu m in datamenu)
                                {

                                    btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
                                }
                                messages.Add("{ \"msg\":\"<br>Please Select Menu from Below <br>" + btndata + "</div>\"}");
                            }
                            else
                            {
                                int id = await menuService.GetMenusId(ppvalues[0], FFbPageId);

                                if (id > 0)
                                {
                                    var qdata = await questionReplayService.Getanswerbymenuid(Convert.ToInt32(id));
                                    if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                                    {
                                        messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                                        btndata += "<button onclick = 'ClickedComplain(" + passportInfo.Id + ")' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Complain</button> <button onclick = 'ClickedSuggestion(" + passportInfo.Id + ")' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Suggestion</button>";
                                        messages.Add("{ \"msg\":\"<br>" + btndata + "</div>\"}");
                                    }
                                }

                            }
                        }

                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                        CloseService(combinedId);
                        PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                        {
                            passportNo = passportInfo.passportNo,
                            refNo = passportInfo.refNo,
                            date = DateTime.Now,
                            status = "Successfully Get all Information"
                        };
                        await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
                    }

                }
            }

            return messages;
        }




        public async Task<List<KeyWordQuesAns>> GetQuestions(string postback, string message, string botKey, string connectionId, int qId, Models.BotModels.ConnectionInfo connectionInfo)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            var questionsByKeyword = new List<KeyWordQuesAns>();


            if (Convert.ToInt32(qId) > 0)
            {
                int? questionId = Convert.ToInt32(qId);
                questionsByKeyword = this.responseMessagesByMenuId(botKey, message, questionId);
            }


            try
            {
                if (message != null && message != "menu" && message != "Navigation Menu" && questionsByKeyword.Count() == 0)
                {
                    var lastMsg = await _context.MessageLogs.AsNoTracking().Include(x => x.KeyWordQuesAns).Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();


                    if (lastMsg != null && lastMsg?.KeyWordQuesAnsId != null && await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && lastMsg.KeyWordQuesAns.isQuestion == 1 && x.branchInfoId == user.branchId).CountAsync() > 0)
                    {
                        questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && (x.keyWordQuesAnsId == lastMsg.KeyWordQuesAnsId) && x.status == 1 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();
                        questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.questionKey == questionsByKeyword.FirstOrDefault().questionKey && x.status == 1 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();
                    }
                    else
                    {

                        questionsByKeyword = await (from q in _context.keyWordQuesAns
                                                    join k in _context.KeyWordInfos
                                                    on q.Id equals k.KeyWordQuesAnsId
                                                    where q.isDelete != 1 && q.question.ToLower() == message.Trim() && k.botKey == botKey && q.status == 1
                                                    && q.type != 6 && q.branchInfoId == user.branchId
                                                    orderby q.priority
                                                    select q).Take(1).ToListAsync();

                        if (questionsByKeyword.Count() == 0)
                        {
                            var keyword = await _context.KeyWordInfos.Where(x => x.nameEn.ToLower().Contains(message.ToLower()) && x.botKey == botKey && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                            if (keyword != null)
                            {
                                var uniqueKey = await _context.keyWordQuesAns.Where(x => x.Id == keyword.KeyWordQuesAnsId && x.branchInfoId == user.branchId).AsNoTracking().Select(x => x.questionKey).FirstOrDefaultAsync();

                                questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.questionKey == uniqueKey && x.isDelete != 1 && x.status == 1 && x.type != 6 && x.branchInfoId == user.branchId).ToListAsync();

                            }

                            if (questionsByKeyword.Count() > 0)
                            {
                                //questionsByKeyword = await _context.keyWordQuesAns.Where(q => q.questionKey == questionsByKeyword.FirstOrDefault().questionKey && q.botKey == botKey && q.status == 1 && q.type != 6)
                                //                            .OrderBy(x => x.questionOrder).ToListAsync();
                            }
                        }
                        else
                        {
                            questionsByKeyword = await _context.keyWordQuesAns.Where(q => q.isDelete != 1 && q.branchInfoId == user.branchId && q.questionKey == questionsByKeyword.FirstOrDefault().questionKey && q.botKey == botKey && q.status == 1 && q.type != 6)
                                                            .OrderBy(x => x.questionOrder).ToListAsync();
                        }

                        if (questionsByKeyword.Count() == 0)
                        {
                            var prevQuestion = await (from q in _context.keyWordQuesAns
                                                      join k in _context.ServiceFlows
                                                      on q.Id equals k.keyWordQuesAnsId
                                                      where q.isDelete != 1 && q.branchInfoId == user.branchId && k.connectionId == connectionId && k.botKey == botKey && k.answerText != null
                                                      select k).OrderByDescending(x => x.StepNo).FirstOrDefaultAsync();

                            if (prevQuestion != null)
                            {
                                if (prevQuestion.questionText == "Please enter doctor name" && prevQuestion.answerText != null)
                                {
                                    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.refName == "SPECIFICDOCTOR").AsNoTracking().ToListAsync();
                                }
                                else if (prevQuestion.questionText == "Please enter department name" && prevQuestion.answerText != null)
                                {
                                    //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.question.ToLower().Trim() == "Neurology").AsNoTracking().ToListAsync();
                                    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.departmentId != null).AsNoTracking().ToListAsync();
                                }
                                //else if (prevQuestion.questionText == "Please enter specializations name" && prevQuestion.answerText != null)
                                //{
                                //    //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.question.ToLower().Trim() == "Neurology").AsNoTracking().ToListAsync();

                                //    var textArr = new List<BotKnowledge>();
                                //    textArr = await _context.BotKnowledges.AsNoTracking().Where(x => x.botKey == botKey && x.branchInfoId == user.branchId).ToListAsync();

                                //    var data = new List<TextSimilarityVm>();
                                //    foreach (var text in textArr)
                                //    {
                                //        double similarity = TextMatcher.CalculateLevenshteinSimilarity(message.ToLower(), text.question.ToLower());

                                //        if (text.question.ToLower().IndexOf(message.ToLower()) >= 0)
                                //        {
                                //            similarity = 1;
                                //        }



                                //        if (similarity >= 0.50 && similarity <= 1) // Check if similarity is within range
                                //        {
                                //            data.Add(new TextSimilarityVm
                                //            {
                                //                text = text.question,
                                //                percentage = similarity,
                                //                KnowledgeId = text.Id
                                //            });

                                //            if (similarity == 1) // Stop if exact match
                                //            {
                                //                break;
                                //            }
                                //        }
                                //    }

                                //    var maxPercentageKnowledgeId = data.OrderByDescending(x => x.percentage).FirstOrDefault()?.KnowledgeId;


                                //    //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.specializationId != null).AsNoTracking().ToListAsync();
                                //    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.specializationId != null).AsNoTracking().ToListAsync();
                                //}
                                else if (prevQuestion.questionText == "Please enter specializations name" && prevQuestion.answerText != null)
                                {
                                    var keyWordQuesAnsdata = _context.keyWordQuesAns.Where(x => x.specializationId != null).Select(x => x.Id).ToList();
                                    var textArr = await _context.BotKnowledges
                                        .AsNoTracking()
                                        .Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && keyWordQuesAnsdata.Contains((int)x.keyWordQuesAnsId))
                                        .ToListAsync();

                                    var data = new List<TextSimilarityVm>();
                                    foreach (var text in textArr)
                                    {
                                        double similarity = TextMatcher.CalculateLevenshteinSimilarity(message.ToLower(), text.question.ToLower());

                                        if (text.question.ToLower().IndexOf(message.ToLower()) >= 0)
                                        {
                                            similarity = 1;
                                        }

                                        if (similarity >= 0.70 && similarity <= 1) // Check if similarity is within range
                                        {
                                            data.Add(new TextSimilarityVm
                                            {
                                                text = text.question,
                                                percentage = similarity,
                                                KnowledgeId = (int)text.keyWordQuesAnsId
                                            });

                                            if (similarity == 1) // Stop if exact match
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    // Get the KnowledgeId with the highest percentage
                                    var maxPercentageKnowledgeId = data.OrderByDescending(x => x.percentage).ToList();

                                    // Fix the query to filter based on maxPercentageKnowledgeId correctly
                                    if (maxPercentageKnowledgeId.Count > 0)
                                    {
                                        var maxKnowledgeIds = maxPercentageKnowledgeId.Select(x => x.KnowledgeId).ToList();

                                        questionsByKeyword = await _context.keyWordQuesAns
                                            .Where(x => x.isDelete != 1
                                                    && x.branchInfoId == user.branchId
                                                    && maxKnowledgeIds.Contains(x.Id))
                                            .AsNoTracking()
                                            .ToListAsync();
                                    }
                                    else
                                    {
                                        questionsByKeyword = new List<KeyWordQuesAns>(); // Or appropriate fallback
                                    }
                                }

                                else
                                {
                                    //questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.keyWordQuesAnsId == prevQuestion.keyWordQuesAnsId).AsNoTracking().ToListAsync();
                                    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.branchInfoId == user.branchId && x.Id == lastMsg.nextNodeId).AsNoTracking().ToListAsync();
                                }
                            }
                            if (questionsByKeyword.Count() > 0)
                            {
                                questionsByKeyword = await _context.keyWordQuesAns.Where(q => q.isDelete != 1 && q.branchInfoId == user.branchId && q.questionKey == questionsByKeyword.FirstOrDefault().questionKey && q.botKey == botKey && q.status == 1 && q.type != 6)
                                                            .OrderBy(x => x.questionOrder).ToListAsync();
                            }
                        }
                    }



                    if (lastMsg != null && lastMsg.nextNodeId != null)
                    {
                        questionsByKeyword = await this.SendNextKeyWordQuestionsByNodeId(botKey, connectionId, lastMsg.nextNodeId);
                    }



                }

                return questionsByKeyword.OrderBy(x => x.questionOrder).ToList();
            }
            catch (Exception ex)
            {
                return questionsByKeyword.OrderBy(x => x.questionOrder).ToList();
            }

        }




        public List<KeyWordQuesAns> responseMessagesByMenuId(string botKey, string message, int? questionId)
        {
            var user = _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefault();

            var lastMessage = message;
            string nextNodeId = "";

            if (_context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId).Select(x => x.nodes) != null)
            {
                //var answer = _context.keyWordQuesAns
                //            .Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId)
                //            .Select(x => x.answer)
                //            .AsNoTracking()
                //            .FirstOrDefault();
                //if (answer != null)
                //{
                //    // Split the answer string and find the position of the last message
                //    var messageArray = answer.Split(",");
                //    var msgPosition = messageArray.ToList().IndexOf(lastMessage);
                //}
                ////var msgposition = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId).Select(x => x.answer).AsNoTracking().FirstOrDefault().Split(",").IndexOf(lastMessage);


                //if (msgposition >= 0)
                //{
                //    nextNodeId = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId).Select(x => x.nodes).AsNoTracking().FirstOrDefault().Split(",")[msgposition].Trim();
                //}

                var answer = _context.keyWordQuesAns
                 .Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId)
                 .Select(x => x.answer)
                 .AsNoTracking()
                 .FirstOrDefault();

                if (answer != null)
                {
                    // Split the answer string and find the position of the last message
                    var messageArray = answer.Split(",");
                    var msgPosition = messageArray.ToList().IndexOf(lastMessage);

                    // Check if msgPosition is valid (i.e., it's >= 0)
                    if (msgPosition >= 0)
                    {
                        // Get the next node ID based on the position
                        nextNodeId = _context.keyWordQuesAns
                           .Where(x => x.isDelete != 1 && x.Id == questionId && x.branchInfoId == user.branchId)
                           .Select(x => x.nodes)
                           .AsNoTracking()
                           .FirstOrDefault();

                        // Split the nodes and get the node at the msgPosition, trimming any extra spaces
                        if (nextNodeId != null)
                        {
                            var nextNodeArray = nextNodeId.Split(",");
                            nextNodeId = nextNodeArray[msgPosition].Trim();
                        }
                    }
                }

            }

                //var data = _context.QuestionNavigations.Where(x => x.requestQuestionId == questionId && x.botKey == botKey).AsNoTracking().Select(x => x.responseQuestion).ToList();
                var data = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == questionId && x.botKey == botKey && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefault();

                if (nextNodeId != "")
                {
                    data.nextNodeId = Convert.ToInt32(nextNodeId.Trim());
                }

                if (data.nextNodeId != null)
                {
                    var nextNode = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == data.nextNodeId && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefault();

                    var result = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.questionKey == nextNode.questionKey && x.branchInfoId == user.branchId).AsNoTracking().ToList();

                    return result;
                }
                return new List<KeyWordQuesAns>();
            }



        public List<KeyWordQuesAns> responseMessagesByNodeId(string botKey, int? nodeId)
        {
            var user = _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefault();

            var data = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.Id == nodeId && x.botKey == botKey && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefault();

            if (data != null)
            {
                var result = _context.keyWordQuesAns.Where(x => x.isDelete != 1 && x.questionKey == data.questionKey && x.branchInfoId == user.branchId).AsNoTracking().ToList();

                return result;
            }
            else
            {
                return new List<KeyWordQuesAns>();
            }
        }


        public async Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack, string userId, string botKey, string connectionId)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();


            string combinedId = pageId + senderId;
            List<string> Messages = new List<string>();
            int id = await facebookService.GetFacebookpageId(pageId);
            if (InfoValidation.CheckConfirmationNew(message))
            {

                var messa = await QuesReplayServiceK(senderId, pageId, message, postBack, userId, botKey, connectionId);
                foreach (string ms in messa)
                {
                    Messages.Add(ms);
                }
                // Messages.Add(messa.FirstOrDefault());

            }
            else
            {
                if (message == "menu" || message == "Menu" || message == "মেনু")
                {

                    //var data = await menuService.GetMenusByParrentId(0, id);
                    var data = await GetMenusByBotKey(0, id, botKey);
                    string btndata = string.Empty;

                    //foreach (Menu m in data)
                    //{
                    //    btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                    //}
                    //Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                    if (message.Contains("menu") || message.Contains("Menu"))
                    {
                        foreach (Menu m in data)
                        {
                            btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + -1 + ")' class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                        }
                        Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                    }
                    else
                    {
                        foreach (Menu m in data)
                        {
                            btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + 0 + ")' class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                        }
                        Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                    }

                }
                else
                {
                    var menudata = await menuService.GetMenusbyname(message, id);
                    if (menudata.Count() > 0)
                    {

                        var data = await menuService.GetMenusByParrentId(menudata.FirstOrDefault().Id, id);
                        string btndata = string.Empty;
                        if (data.Count() > 0)
                        {
                            foreach (Menu m in data)
                            {
                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                            }
                            Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                        }
                        else
                        {
                            //var qdata = await questionReplayService.Getanswerbymenuid(menudata.FirstOrDefault().Id);

                            //if (qdata.AnswerTextEN != string.Empty || qdata.AnswerTextEN != null)
                            //{
                            //    Messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                            //}
                            var qdata = await _doctorInfoService.GetDoctorListbymenuid(menudata.FirstOrDefault().Id);
                            if (qdata.Count() > 0)
                            {
                                foreach (DoctorInfo d in qdata)
                                {
                                    btndata += @"<div>" +
                                                    "<span>" + d.name + ", " + d.designationName + ",</span><br /><span>" + d.departmentName + "</span><br />" +
                                                    "<button onclick =ClickedAppointment(" + d.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Appointment</button>" +
                                                "</div><p>____________________________</p>";
                                }
                                Messages.Add("{ \"msg\":\"" + btndata + "</div>\"}");
                            }
                            else
                            {

                                data = await menuService.GetMenusByParrentId(0, id);
                                btndata = string.Empty;
                                if (data.Count() > 0)
                                {
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                    }
                                    Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                                }


                            }
                        }

                        //var messa = await QuesReplayServiceK(senderId, pageId, message, postBack);
                        //Messages.Add(messa.FirstOrDefault());
                    }
                    else
                    {

                        UnKnownKeyWordQuestion unKnownKeyWordQuestion = new UnKnownKeyWordQuestion
                        {
                            question = message,
                            autoNumber = userId,
                            botKey = botKey,
                            connectionId = connectionId,
                            type = "menu"
                        };
                        await keyWordQuesService.Saveunknownquestion(unKnownKeyWordQuestion);

                        var messa = await QuesReplayServiceK(senderId, pageId, message, postBack, userId, botKey, connectionId);
                        foreach (string ms in messa)
                        {
                            Messages.Add(ms);
                        }
                        // Messages.Add(messa.ToList());
                    }

                }
            }
            //else
            //{
            //    if (message.ToLower() != "yes" || message.ToLower() != "no" || message != "না" || message != "হ্যাঁ")
            //    {
            //        UnKnownKeyWordQuestion unKnownKeyWordQuestion = new UnKnownKeyWordQuestion
            //        {
            //            question = message,

            //        };
            //        await keyWordQuesService.Saveunknownquestion(unKnownKeyWordQuestion);
            //    }
            //    var messa=  await QuesReplayServiceK(senderId, pageId, message, postBack);
            //    Messages.Add(messa.FirstOrDefault());
            //}



            return Messages;

        }








        public async Task<List<string>> QuesReplayServiceK(string senderId, string pageId, string message, string postBack, string userId, string botKey, string connectionId)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            string combinedId = pageId + senderId;
            List<string> Messages = new List<string>();
            int id = await facebookService.GetFacebookpageId(pageId);
            if (postBack == "")
            {
                KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(id, message, combinedId, senderId);
                int keywordcount = keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageIdcount(id, message, combinedId, senderId);

                if (temp.question != null && temp.question != "")
                {

                    string tm1 = new String(message.Where(Char.IsLetter).ToArray());
                    string tm2 = new String(temp.question.Where(Char.IsLetter).ToArray());
                    if (tm1.ToLower() == tm2.ToLower()) // Exact Match
                    {
                        if (temp.answer.Contains("Are you looking for a doctor") || temp.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || temp.answer == "?")
                        {

                            Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' class='btn-menu' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + -1 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + 0 + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        }
                        else if (temp.answer.Contains("menu") || temp.answer.Contains("Menu") || temp.answer.Contains("মেনু"))
                        {
                            int FbPageId = await facebookService.GetFacebookpageId(pageId);
                            var data = await menuService.GetMenusByParrentId(0, FbPageId);
                            string btndata = string.Empty;


                            if (temp.answer.Contains("menu") || temp.answer.Contains("Menu"))
                            {
                                foreach (Menu m in data)
                                {
                                    btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + -1 + ")' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                }
                                Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                            }
                            else
                            {
                                foreach (Menu m in data)
                                {
                                    btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + 0 + ")' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                }
                                Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                            }

                            // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        }
                        else
                        {
                            if (temp.answer.Contains("?"))
                            {
                                Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                // Messages.Add("{ \"msg\":\"<br><br><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                //string response = "{ \"msg\":\"" + temp.answer + "\",\"postback\":\"KWA;" + temp.Id + "\"}";
                                //Messages.Add(response);
                            }
                            else
                            {
                                Messages.Add("{ \"msg\":\"" + temp.answer + "\"}");
                            }

                        }
                        //Messages.Add("{ \"msg\":\"" + temp.answer + ".\"}");
                        if (temp.more != "" && temp.more != null)
                            Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");
                    }
                    else
                    {
                        if (keywordcount == 1)
                        {
                            Messages.Add("{ \"msg\":\"" + temp.answer + "\"}");
                        }
                        else
                        {
                            Messages.Add("{ \"msg\":\" Did you mean," + temp.question + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                        }

                        // Messages.Add("{ \"msg\":\"<br><br><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        //string response = "{ \"msg\":\"" + temp.question + "\",\"postback\":\"KWA;" + temp.Id + "\"}";
                        //Messages.Add(response);
                    }
                }
                else
                {
                    //string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                    //Messages.Add(response);
                    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                    string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                    UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                    {
                        fbPageId = FbPageId,
                        userId = "",
                        userName = "",
                        userQuestion = message
                    };
                    await queriesService.SaveUserQueries(userQueriesViewModel);
                    // Messages.Add("{ \"msg\":\"Are you looking for a doctor?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                    //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                    //Messages.Add(response);
                    var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                    string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                    Messages.Add(response);
                    Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                }
            }
            else
            {
                if (InfoValidation.CheckConfirmation(message))
                {
                    string[] QRSpiltData = postBack.Split(";");
                    KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(Int32.Parse(QRSpiltData[1]));
                    if (temp.IsLoop == 1)
                    {
                        //KeyWordQuesAns tempy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);
                        KeyWordQuesAns tempy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedIDcatid((int)temp.questionCategoryId, temp.Id, message);

                        if (tempy != null)
                        {
                            if (tempy.answer.Contains("Are you looking for a doctor") || tempy.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || tempy.answer == "?")
                            {

                                Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + -1 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + 0 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            }
                            else if (tempy.answer.Contains("menu") || tempy.answer.Contains("Menu") || tempy.answer.Contains("মেনু"))
                            {
                                int FbPageId = await facebookService.GetFacebookpageId(pageId);
                                var data = await menuService.GetMenusByParrentId(0, FbPageId);
                                string btndata = string.Empty;


                                if (tempy.answer.Contains("menu") || tempy.answer.Contains("Menu"))
                                {
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                    }
                                    Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                                }
                                else
                                {
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                    }
                                    Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                                }

                                // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            }
                            else
                            {
                                if (tempy.answer.Contains("?"))
                                {
                                    Messages.Add("{ \"msg\":\"" + tempy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");

                                }
                                else
                                {

                                    Messages.Add("{ \"msg\":\"" + tempy.answer + "\"}");
                                }

                            }

                            if (tempy.more != "" && tempy.more != null)
                                Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");

                            //Messages.Add("{ \"msg\":\" Did you mean? (yes or no)\"}");
                            //string response = "{ \"msg\":\"" + tempy.question + "\",\"postback\":\"KWA;" + tempy.Id + "\"}";
                            //Messages.Add(response);
                        }
                        else
                        {
                            //string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                            //Messages.Add(response);
                            var text = await keyWordQuesService.getunknowquestion();
                            int FbPageId = await facebookService.GetFacebookpageId(pageId);
                            string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                            UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                            {
                                fbPageId = FbPageId,
                                userId = "",
                                userName = "",
                                userQuestion = text.question
                            };
                            await queriesService.SaveUserQueries(userQueriesViewModel);
                            // await queriesService.SaveUserQueries(userQueriesViewModel);
                            // Messages.Add("{ \"msg\":\"Are you looking for a doctor?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                            //Messages.Add(response);
                            var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                            string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                            Messages.Add(response);
                            Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                            // Messages.Add("{ \"msg\":\"Are you looking for a doctor?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        }
                    }
                    else
                    {

                        if (temp.answer.Contains("Are you looking for a doctor") || temp.answer == "?")
                        {

                            Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1,-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                        }
                        {
                            Messages.Add("{ \"msg\":\"" + temp.answer + "\"}");

                        }



                        if (temp.more != "" && temp.more != null)
                            Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");
                    }

                }
                else
                {
                    if (postBack != "")
                    {
                        string[] QRSpiltData = postBack.Split(";");
                        KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(Int32.Parse(QRSpiltData[1]));
                        if (temp.IsLoop == 1)
                        {
                            //KeyWordQuesAns tempy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);
                            KeyWordQuesAns tempy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedIDcatid((int)temp.questionCategoryId, temp.Id, message);

                            if (tempy != null)
                            {
                                if (tempy.answer.Contains("Are you looking for a doctor") || tempy.answer == "?")
                                {

                                    Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + -1 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }
                                else if (tempy.answer.Contains("menu") || tempy.answer.Contains("Menu") || tempy.answer.Contains("মেনু"))
                                {
                                    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                                    var data = await menuService.GetMenusByParrentId(0, FbPageId);
                                    string btndata = string.Empty;


                                    if (tempy.answer.Contains("menu") || tempy.answer.Contains("Menu"))
                                    {
                                        foreach (Menu m in data)
                                        {
                                            btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                        }
                                        Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                                    }
                                    else
                                    {
                                        foreach (Menu m in data)
                                        {
                                            btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                        }
                                        Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                                    }

                                    // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                else
                                {
                                    if (tempy.answer.Contains("?"))
                                    {
                                        Messages.Add("{ \"msg\":\"" + tempy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                        // Messages.Add("{ \"msg\":\"<br><br><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                        //string response1 = "{ \"msg\":\"" + tempy.answer + "\",\"postback\":\"KWA;" + tempy.Id + "\"}";
                                        //Messages.Add(response1);
                                    }
                                    else
                                    {

                                        Messages.Add("{ \"msg\":\"" + tempy.answer + "\"}");
                                    }

                                }

                                if (tempy.more != "" && tempy.more != null)
                                    Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");

                                //Messages.Add("{ \"msg\":\" Did you mean? (yes or no)\"}");
                                //string response = "{ \"msg\":\"" + tempy.question + "\",\"postback\":\"KWA;" + tempy.Id + "\"}";
                                //Messages.Add(response);
                            }
                            else
                            {
                                //string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                                //Messages.Add(response);
                                var text = await keyWordQuesService.getunknowquestion();
                                int FbPageId = await facebookService.GetFacebookpageId(pageId);
                                string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                                UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                                {
                                    fbPageId = FbPageId,
                                    userId = "",
                                    userName = "",
                                    userQuestion = text.question
                                };
                                await queriesService.SaveUserQueries(userQueriesViewModel);
                                // Messages.Add("{ \"msg\":\"Are you looking for a doctor?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                                //Messages.Add(response);
                                var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                                string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                                Messages.Add(response);
                                Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                            }
                        }
                        else
                        {

                            //KeyWordQuesAns tempyy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedIDcatid((int)temp.questionCategoryId, temp.Id, message);
                            KeyWordQuesAns tempyy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedIDNewcatid(temp.Id, message);
                            if (tempyy != null)
                            {
                                if (message.ToLower() == "no")
                                {
                                    Messages.Add("{ \"msg\":\" Did you mean," + tempyy.question + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }
                                else if (message.ToLower() == "না")
                                {
                                    Messages.Add("{ \"msg\":\" Did you mean," + tempyy.question + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }
                                else
                                {
                                    if (tempyy.answer.Contains("Are you looking for a doctor") || tempyy.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || tempyy.answer == "?")
                                    {

                                        Messages.Add("{ \"msg\":\"" + tempyy.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0,-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                    }
                                    else
                                    {

                                        if (tempyy.answer.Contains("?"))
                                        {
                                            Messages.Add("{ \"msg\":\"" + tempyy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");

                                        }
                                        else
                                        {
                                            Messages.Add("{ \"msg\":\"" + tempyy.answer + "\"}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                                if (message.ToLower() == "no")
                                {
                                    var text = await keyWordQuesService.getunknowquestion();
                                    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                                    string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                                    UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                                    {
                                        fbPageId = FbPageId,
                                        userId = "",
                                        userName = "",
                                        userQuestion = text.question
                                    };
                                    await queriesService.SaveUserQueries(userQueriesViewModel);

                                    //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                                    string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                                    Messages.Add(response);
                                    Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }
                                if (message.ToLower() == "না")
                                {
                                    var text = await keyWordQuesService.getunknowquestion();
                                    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                                    string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                                    UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                                    {
                                        fbPageId = FbPageId,
                                        userId = "",
                                        userName = "",
                                        userQuestion = text.question
                                    };
                                    await queriesService.SaveUserQueries(userQueriesViewModel);

                                    //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                                    string response = "{ \"msg\" : \"" + lastgret.NameBn + "\"}";
                                    Messages.Add(response);
                                    Messages.Add("{ \"msg\":\"" + "আপনি কি পুলিশ ক্লিয়ারেন্সের জন্য আবেদন করেছেন?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }
                                //else
                                //{
                                //    Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                //}

                            }



                            if (temp.more != "" && temp.more != null)
                                Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");
                        }
                    }
                    else
                    {
                        KeyWordQuesAns temp = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);

                        if (temp.question != null && temp.question != "")
                        {

                            if (temp.answer.Contains("?") && temp.IsLoop == 1)
                            {
                                Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");

                            }
                            else
                            {
                                Messages.Add("{ \"msg\":\" Did you mean," + temp.question + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                            }



                        }
                        else
                        {
                            var text = await keyWordQuesService.getunknowquestion();
                            int FbPageId = await facebookService.GetFacebookpageId(pageId);
                            string accessToken = await facebookService.GetAccessTokenById(FbPageId);


                            UserQueriesViewModel userQueriesViewModel = new UserQueriesViewModel
                            {
                                fbPageId = FbPageId,
                                userId = "",
                                userName = "",
                                userQuestion = text.question
                            };
                            await queriesService.SaveUserQueries(userQueriesViewModel);
                            //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                            //Messages.Add(response);
                            var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                            string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                            Messages.Add(response);
                            Messages.Add("{ \"msg\":\"" + "Are you looking for a doctor?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");

                        }

                    }


                }
            }

            return Messages;
        }









        public async Task<IEnumerable<Menu>> GetData(string parentId, string pageid)
        {

            int FbPageId = await facebookService.GetFacebookpageId(pageid);
            var data = await menuService.GetMenusByParrentname(parentId, FbPageId);
            return data;


        }






        public int CloseService(string combinedId)
        {
            serviceFlowService.CLearServiceData(combinedId);
            botFlowService.UpdateFlow(combinedId, "default");

            return 1;
        }




        public async Task<IEnumerable<Menu>> GetMenusByBotKey(int parrentId, int faceBookPageId, string botKey)
        {
            if (parrentId == -1)
            {

                parrentId = 0;
            }
            return await _context.Menus.Where(x => x.ParrentId == parrentId && x.botKey == botKey).ToListAsync();
        }



        public async Task<bool> SaveMenu(MenuViewModel model, int faceBookPageId, string botKey)
        {
            try
            {
                var entity = new Menu
                {
                    MenuName = model.MenuName,
                    MenuNameEN = model.MenuNameEN,
                    FacebookPageId = faceBookPageId,
                    IsLast = (model.IsLast == "on" ? true : false),
                    menuType = model.menuType,
                    ParrentId = model.ParrentMenuId,
                    botKey = botKey,
                    responseApi = model.responseAPI
                };
                _context.Menus.Add(entity);

                return 1 == await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool RenameMenu(MenuViewModel model, string botKey)
        {
            try
            {
                Menu menu = _context.Menus.Find(model.ParrentMenuId);
                menu.MenuName = model.MenuName;
                menu.MenuNameEN = model.MenuNameEN;
                menu.menuType = model.menuType;
                menu.responseApi = model.responseAPI;
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            throw new NotImplementedException();
        }


        public async Task<ChatbotInfo> GetBotInfoByUserName(string username)
        {
            var data = await _context.ChatbotInfos.Include(x => x.ApplicationUser).Where(x => x.ApplicationUser.UserName == username).FirstOrDefaultAsync();

            return data;
        }

        public async Task<KeyWordQuestionDetailVm> GetKeywordQuestionAnsById(int? id)
        {
            var data = await _context.keyWordQuesAns.Where(x => x.Id == id).AsNoTracking().Select(x => new KeyWordQuestionDetailVm
            {
                question = x.question,
                answer = x.answer,
                priority = x.priority,
                questionKey = x.questionKey,
                type = x.type == 1 ? "Text" : x.type == 2 ? "Button" : x.type == 3 ? "Button Group" : x.type == 4 ? "Html" : x.type == 5 ? "IFrame Url" : x.type == 6 ? "Card Group" : x.type == 7 ? "Input Group" : x.type == 110 ? "Ref" : "",
                orderNo = x.questionOrder,
                status = x.status == 1 ? "Active" : "Inactive",
                isQuestion = x.isQuestion == 1 ? "Yes" : "No",
                nextNode = x.nextNodeId == null ? "" : _context.keyWordQuesAns.Where(y => y.Id == x.nextNodeId).Select(y => y.question).AsNoTracking().FirstOrDefault(),
                SmsOTP = x.smsOtp == 1 ? "Yes" : "No",
                Department = x.departmentId != null ? _context.DepartmentInfos.Where(y => y.Id == x.departmentId).AsNoTracking().Select(y => y.departmentName).FirstOrDefault() : "",
                Doctor = x.doctorId != null ? _context.DoctorInfos.Where(y => y.Id == x.doctorId).AsNoTracking().Select(y => y.name).FirstOrDefault() : "",
                Reference = x.refName == null ? "" : x.refName
            }).FirstOrDefaultAsync();

            return data;
        }


        public async Task<IEnumerable<Menu>> GetDataByBotKey(int parrentId, int faceBookPageId, string botKey)
        {
            if (parrentId == -1)
            {

                parrentId = 0;
            }
            return await _context.Menus.Where(x => x.ParrentId == parrentId && x.botKey == botKey).ToListAsync();
        }



        public async Task<bool> SaveLastGreetings(LastGrettings model)
        {
            if (model.Id == 0)
            {
                _context.lastGrettings.Add(model);
            }
            else
            {
                _context.lastGrettings.Update(model);
            }

            return 1 == await _context.SaveChangesAsync();
        }



        public async Task<bool> SaveMessageLog(MessageLog model)
        {
            var keywordQuestionAns = await _context.keyWordQuesAns.Where(x => x.question.Trim().ToLower() == model.rawMessage.ToLower().Trim()).AsNoTracking().OrderBy(x => x.priority).FirstOrDefaultAsync();

            if (keywordQuestionAns?.isQuestion == 1)
            {
                model.KeyWordQuesAnsId = keywordQuestionAns.Id;
            }

            if (model.Id == 0)
            {
                _context.MessageLogs.Add(model);
            }
            else
            {
                _context.MessageLogs.Update(model);
            }

            return 1 == await _context.SaveChangesAsync();
        }


        public async Task<Menu> GetMenuByIdAndBotKey(int menuId, string botKey)
        {
            var data = await _context.Menus.Where(x => x.Id == menuId && x.botKey == botKey).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }



        public async Task<List<string>> CustomInputMessageGenerator(int menuId, string botKey, string connectionId)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            List<string> messages = new List<string>();

            var flows = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.botKey == botKey && x.status == 1 && x.branchInfoId == user.branchId).AsNoTracking().OrderBy(x => x.StepNo).ToListAsync();
            var data = await _context.MenuReaders.Where(x => x.menuId == menuId && x.botKey == botKey && !flows.Select(y => y.questionText).Contains(x.message) && x.branchInfoId == user.branchId).OrderBy(x => x.stepNo).AsNoTracking().FirstOrDefaultAsync();

            string responseApi = await _context.Menus.Where(x => x.Id == menuId && x.branchInfoId == user.branchId).Select(x => x.responseApi).AsNoTracking().LastOrDefaultAsync();

            if (flows.Count() > 0 && data == null)
            {
                for (int i = 0; i < flows.Count(); i++)
                {
                    string oldText = "[" + i.ToString() + "]";
                    string newText = flows[i].answerText;

                    responseApi = responseApi.Replace(oldText.Trim(), newText.Trim());
                }

                if (await CallApiAsync(responseApi) != null)
                {
                    var resMsg = await CallApiAsync(responseApi);
                    messages.Add("{ \"msg\":" + resMsg + "}");
                }
            }




            if (data != null)
            {
                messages.Add("{ \"msg\":\"" + data.message + "\"}");

                var serviceflow = new ServiceFlow
                {
                    Id = Guid.NewGuid().ToString(),
                    botKey = botKey,
                    connectionId = connectionId,
                    DateTime = DateTime.Now,
                    InfoType = "start",
                    StepNo = data.stepNo,
                    status = 0,
                    questionText = data.message,
                    Attempt = 0,
                    ServiceCode = "Pre-Defined Question",
                    MenuId = menuId,
                    keyWordQuesAnsId = await _context.keyWordQuesAns.Where(x => x.question.Trim() == data.message.Trim() && x.branchInfoId == user.branchId).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync(),
                    branchInfoId = user.branchId
                };

                _context.ServiceFlows.Add(serviceflow);

                await _context.SaveChangesAsync();
            }


            return messages;
        }

        public async Task<ServiceFlow> GetPendingQuestion(string botKey, string connectionId, string message)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            if (await _context.DoctorInfos.Where(x => x.name == message && x.branchInfoId == user.branchId && x.isDelete != 1).AsNoTracking().LastOrDefaultAsync() != null)
            {
                var data = await _context.ServiceFlows.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && x.questionText == "Enter Dr. Name" && x.connectionId == connectionId && x.status == 0).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                return data;
            }
            else if (await _context.DoctorSpecializations.Where(x => x.name == message && x.branchInfoId == user.branchId && x.isDelete != 1).AsNoTracking().LastOrDefaultAsync() != null)
            {
                var data = await _context.ServiceFlows.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && x.questionText == "Please enter specializations name" && x.connectionId == connectionId && x.status == 0).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                return data;
            }
            //else if (await _context.DepartmentInfos.Where(x => x.departmentName == message && x.branchInfoId == user.branchId).AsNoTracking().LastOrDefaultAsync() != null)
            //{
            //    var data = await _context.ServiceFlows.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && x.questionText == "Please enter department name" && x.connectionId == connectionId && x.status == 0).OrderBy(x => x.DateTime).LastOrDefaultAsync();

            //    return data;
            //}

            else
            {
                var data = await _context.ServiceFlows.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && x.connectionId == connectionId && x.status == 0).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                return data;
            }
        }


        static async Task<string> CallApiAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse.ToString();
                }
                else
                {
                    Console.WriteLine("Error calling the API. Status code: " + response.StatusCode);
                    return null; // or throw an exception, or handle the error in another way
                }
            }
        }



        public async Task<bool> SaveServiceFlow(ServiceFlow model)
        {
            try
            {
                var data = await _context.ServiceFlows.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();

                if (data != null)
                {
                    _context.ServiceFlows.Update(model);
                }
                else
                {
                    _context.ServiceFlows.Add(model);
                }


                return 1 == await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<string>> SendNextMessageByNextNodeId(string botKey, string connectionId, int? nextNodeId)
        {

            var result = await this.SendNextMessageByNodeId(botKey, connectionId, nextNodeId);

            return result;
        }




        public async Task<List<string>> SendNextMessageByNodeId(string botKey, string connectionId, int? nodeId)
        {
            //var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            var questionsByKeyword = this.responseMessagesByNodeId(botKey, nodeId);

            var result = await SendMessage(questionsByKeyword, botKey, connectionId);


            return result;
        }


        public async Task<List<KeyWordQuesAns>> SendNextKeyWordQuestionsByNodeId(string botKey, string connectionId, int? nodeId)
        {
            var questionsByKeyword = this.responseMessagesByNodeId(botKey, nodeId);

            return questionsByKeyword;
        }



        public async Task<string> GetLastOTPByConnectionId(string connectionId)
        {
            var data = await _context.OTPCodes.Where(x => x.connectionId == connectionId).AsNoTracking().Select(x => x.otpCode).LastOrDefaultAsync();

            return data;
        }

        public async Task<int?> SendOTPByNodeId(string botKey, string connectionId, int? nodeId)
        {
            var node = await _context.keyWordQuesAns.Where(x => x.Id == nodeId).AsNoTracking().FirstOrDefaultAsync();

            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            if (Convert.ToInt32(node.smsOtp) == 1)
            {
                var otpCode = this.GenerateRandomOTP(4);

                var otpData = new OTPCode
                {
                    Id = 0,
                    botKey = botKey,
                    branchInfoId = user.branchId,
                    connectionId = connectionId,
                    entryDate = DateTime.Now,
                    expireTime = DateTime.Now.AddMinutes(5),
                    otpCode = otpCode.ToString(),
                    parameterName = "OTPCode",
                    refMenuId = node.nextNodeId
                };

                try
                {
                    _context.OTPCodes.Add(otpData);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var phoneNumber = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && (x.questionText == "Phone") && x.answerText != null).AsNoTracking().Select(x => x.answerText).LastOrDefaultAsync();

                var otpHtml = "";

                if (phoneNumber != null)
                {
                    //Send OTP

                    if (_configuration["Project:isLive"] == "YES")
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                        smsAPI.Single_Sms(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                    }
                    else
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";
                        smsAPI.Single_Sms(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");

                        //await this.SendSMSAsync(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                    }
                }

                var otpmsg = "<p>Please enter otp received on your number</p>";
            }

            return node.nextNodeId == null ? Convert.ToInt32(node.nodes) : node.nextNodeId;
        }


        public async Task<int?> SendOTPByNodeId2(string botKey, string connectionId, int? nodeId)
        {
            var node = await _context.keyWordQuesAns.Where(x => x.Id == nodeId).AsNoTracking().FirstOrDefaultAsync();

            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            if (Convert.ToInt32(node.smsOtp) == 1)
            {
                var otpCode = this.GenerateRandomOTP(4);

                var otpData = new OTPCode
                {
                    Id = 0,
                    botKey = botKey,
                    branchInfoId = user.branchId,
                    connectionId = connectionId,
                    entryDate = DateTime.Now,
                    expireTime = DateTime.Now.AddMinutes(5),
                    otpCode = otpCode.ToString(),
                    parameterName = "OTPCode",
                    refMenuId = node.nextNodeId
                };

                try
                {
                    _context.OTPCodes.Add(otpData);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //var token = await GetActiveToken();
                //string bearerToken = token.token;
                //string baseUrl = "https://Applink.evercarebd.com:8018/api/Registration?type=mobileno&value=";

                //var baseUrl2 = $"{baseUrl}";
                //var url = await ApiCall.GetApiResponseAsync<List<patientViewModel>>(baseUrl2, bearerToken);

                var phoneNumber = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && (x.questionText == "Phone") && x.answerText != null).AsNoTracking().Select(x => x.answerText).LastOrDefaultAsync();




                var otpHtml = "";

                if (phoneNumber != null)
                {
                    //Send OTP

                    if (_configuration["Project:isLive"] == "YES")
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                        smsAPI.Single_Sms(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                    }
                    else
                    {
                        otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick='SubmitOTPCode()' value='Submit' />";

                        //await this.SendSMSAsync(phoneNumber, "Your OTP Code is " + otpCode + ". Keep it secure. It is valid for 5 minutes only.");
                    }
                }

                //var otpmsg = "<p>Please enter otp received on your number</p>";
            }

            return node.nextNodeId == null ? Convert.ToInt32(node.nodes) : node.nextNodeId;
        }




        public int GenerateRandomOTP(int length)
        {
            Random random = new Random();
            if (length < 1 || length > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length should be between 1 and 9 inclusive for a 4-digit OTP.");
            }

            int min = (int)Math.Pow(10, length - 1);
            int max = (int)Math.Pow(10, length) - 1;

            int otp = random.Next(min, max + 1);

            return otp;
        }




        public async Task<string> SendSMSAsync(string mobile, string message)
        {
            try
            {
                if (mobile.Length < 11)
                {
                    int numOfZeros = 11 - mobile.Length;
                    mobile = new string('0', numOfZeros) + mobile;
                }
                string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&destination=88{0}&source=8809617611359&message={1}", mobile, message);
                //string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&destination=88{0}&source=8809617611359&message={1}", mobile, message);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) return "success";
                return "fail";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<bool> SendHTMLEmail(string mailTo, string subject, string htmlMessage)
        {
            try
            {
                var emailMessage = new MailMessage();
                emailMessage.From = new MailAddress(_configuration["Email:Email"], "EVERCARE HOSPITAL | BANGLADESH");
                emailMessage.To.Add(new MailAddress(mailTo));

                emailMessage.Subject = subject;
                emailMessage.Body = htmlMessage; // Set the HTML content here
                emailMessage.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(_configuration["Email:Host"]);
                smtp.Credentials = new NetworkCredential(_configuration["Email:Email"], _configuration["Email:Password"]);

                smtp.Port = int.Parse(_configuration["Email:Port"]);
                smtp.EnableSsl = true;
                smtp.Send(emailMessage);

                await Task.CompletedTask;

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> SendEMAIL(string mailTo, string subject, string message)
        {
            try
            {
                var emailMessage = new MailMessage();
                emailMessage.From = new MailAddress("no-reply@opus-bd.com", "EVERCARE HOSPITAL | BANGLADESH");
                emailMessage.To.Add(new MailAddress(mailTo));

                emailMessage.Subject = subject;
                emailMessage.Body = message;
                emailMessage.IsBodyHtml = true;


                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Credentials = new NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(emailMessage);



                await Task.CompletedTask;

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
        }





        public async Task<List<string>> CheckAndValidateOTP(string otp)
        {
            //IGlobal globalService = new GlobalService();
            //var u = globalService.GetOnlineUsers();
            var result = new List<string>();

            var isValid = await _context.OTPCodes.Where(x => x.otpCode == otp).AsNoTracking().LastOrDefaultAsync();

            var lastmsg = await _context.MessageLogs.Where(x => x.connectionId == isValid.connectionId).AsNoTracking().OrderBy(x => x.Id).LastOrDefaultAsync();

            if (isValid != null)
            {
                try
                {
                    result = await this.SendNextMessageByNextNodeId(isValid.botKey, isValid.connectionId, lastmsg.nextNodeId);

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return result;
        }

        public async Task<IEnumerable<Menu>> GetAllMenusByBotKey(string botKey)
        {
            var data = await _context.Menus.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<IEnumerable<BotKnowledge>> GetAllBotKnowledgeByBotKey(string botKey)
        {
            var data = await _context.BotKnowledges.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<KeyWordQuesAns> GetKeywordQuesById(int? id)
        {
            var data = await _context.keyWordQuesAns.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }

        public async Task<IEnumerable<AppoinmentInfo>> GetAllAppointment(string botKey)
        {
            //var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey).Include(x => x.doctorInfo).AsNoTracking().ToListAsync();
            var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<IEnumerable<AppoinmentInfo>> GetOngoingAppointment(string botKey)
        {
            //var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey && x.status == 1).Include(x => x.doctorInfo).AsNoTracking().ToListAsync();
            var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey && x.status == 1).AsNoTracking().ToListAsync();

            return data;
        }

        public async Task<IEnumerable<AppoinmentInfo>> GetConfirmedAppointment(string botKey)
        {
            //var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey && x.status == 3).Include(x => x.doctorInfo).AsNoTracking().ToListAsync();
            var data = await _context.AppoinmentInfos.Where(x => x.botKey == botKey && x.status == 3).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<IEnumerable<BotKnowledge>> GetKnowledgeByBotKey(string botKey)
        {

            {
                var data = await _context.BotKnowledges.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();
                return data;
            }
        }


        public async Task<int> AddRescheduleDoctor(int scheduleId)
        {
            var schedule = await _context.AppoinmentInfos.Where(x => x.Id == scheduleId).AsNoTracking().FirstOrDefaultAsync();

            var doctor = await _context.DoctorInfos.FindAsync(schedule.doctorInfoId);

            var data = new ServiceFlow
            {
                botKey = schedule.botKey,
                connectionId = null,
                status = 1,
                questionText = "Enter Dr. Name",
                answerText = doctor.name,
                MenuId = 0,
                branchInfoId = doctor.branchInfoId
            };

            _context.ServiceFlows.Add(data);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuReader>> GetMenuReaderByMenuId(int menuId)
        {
            var data = await _context.MenuReaders.Where(x => x.menuId == menuId).OrderBy(x => x.stepNo).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<int> DeleteMenuReader(int id)
        {
            var data = await _context.MenuReaders.FindAsync(id);

            _context.MenuReaders.Remove(data);

            var result = await _context.SaveChangesAsync();


            return result;
        }



        public async Task<bool> SaveMenuReader(MenuReader model)
        {
            if (model.Id == 0)
            {
                _context.MenuReaders.Add(model);
            }
            else
            {
                _context.MenuReaders.Update(model);
            }

            return 1 == await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetPreMessageByWrapperDetailsId(int id)
        {
            var data = await _context.WrapperMessages.Where(x => x.BotWrapperDetailId == id && x.isDelete != 1).OrderBy(x => x.orderNo).Select(x => x.message).AsNoTracking().ToListAsync();

            return data;
        }

        public async Task<int?> GetQuestionByText(string text)
        {
            var data = await _context.keyWordQuesAns.Where(x => "{\"msg\" : \"" + x.answer.Trim() + "\"}" == text).Select(x => x.Id).FirstOrDefaultAsync();

            return data;
        }


        public async Task<int> SaveConnectionInfo(Models.BotModels.ConnectionInfo model)
        {
            if (model.Id == 0)
            {
                _context.ConnectionInfos.Add(model);
                await _context.SaveChangesAsync();
            }
            return 1;
        }





        public async Task<List<string>> SendMessage(List<KeyWordQuesAns> questionsByKeyword, string botKey, string connectionId)

        {
            //forced to departments
            //if (questionsByKeyword.FirstOrDefault()?.Id == 164)
            //{
            //    questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.Id == 1503).AsNoTracking().ToListAsync();
            //}

            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();


            var messages = new List<string>();

            if (questionsByKeyword.Count() > 0)
            {
                var departmentCards = "";
                var doctorCards = "";
                var specializationsCards = "";

                var doctors = new List<DoctorInfo>();
                var departments = new List<DepartmentInfo>();
                var specializations = new List<DoctorSpecialization>();

                foreach (var item in questionsByKeyword)
                {
                    if (item.type == 1)
                    {
                        messages.Add("{\"msg\" : \"" + item.answer?.Trim() + "\"}");
                        //If it is question, save it in serviceFlow

                        if (item.isQuestion == 1)
                        {
                            int stepNo = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.status == 0 && x.branchInfoId == user.branchId).AsNoTracking().OrderByDescending(x => x.StepNo).Select(x => x.StepNo).FirstOrDefaultAsync();

                            int? qId = await _context.keyWordQuesAns.Where(x => x.branchInfoId == user.branchId && x.answer.Replace(@"\r\n", "").Trim().Replace(@"\r\n", "") == item.answer.Replace(@"\r\n", "").Trim().Replace(@"\r\n", "")).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync();


                            var sflow = new ServiceFlow
                            {
                                Id = Guid.NewGuid().ToString(),
                                InfoType = "start",
                                ServiceCode = "Pre-Defined Question",
                                connectionId = connectionId,
                                DateTime = DateTime.Now,
                                Attempt = 0,
                                StepNo = stepNo + 1,
                                botKey = botKey,
                                status = 0,
                                questionText = item.answer, //this is question for now
                                answerText = null,
                                MenuId = 0,
                                keyWordQuesAnsId = qId == 0 ? null : qId,
                                branchInfoId = user.branchId
                            };

                            try
                            {
                                _context.ServiceFlows.Add(sflow);
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else if (item.type == 2)
                    {
                        var tt = "<button type='button'>My Btn</button>";
                        messages.Add("{\"msg\" : \"" + tt + "\"}");
                    }
                    else if (item.type == 3)
                    {
                        var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-20px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

                        var part2 = "";
                        foreach (var btn in item.answer.Split(","))
                        {
                            //part2 += "<button class='btn-group-message'" +
                            //    " onclick='handleButtonClick(" + btn.Trim() + ")'>" + btn.Trim() + "</button>";

                            var btnText = btn;
                            if (btn?.Trim() == "Male")
                            {
                                btnText = "<img src=\'https://chatbot.hlthclub.in/static/male.svg\' width=\'32px\' alt=\'sunil\' style=\'float: left;\'>" + "<span style='padding: 15px 10px;'>Male </span>";
                            }
                            if (btn?.Trim() == "Female")
                            {
                                btnText = "<img src=\'https://chatbot.hlthclub.in/static/female.svg\' width=\'32px\' alt=\'sunil\' style=\'float: left;\'>" + "<span style='padding: 15px 10px;'>Female</span>";
                            }
                            //if(btn?.Trim() == "New Appointment" || btn?.Trim() == "Book Appointment" || btn?.Trim() == "Book Appointment with Doctor" || btn?.Trim() == "Book Appointment with department")
                            //{
                            //    btnText = "<span onclick='SubmitInputField2(this)'>" + btnText?.Trim() + "</span>";

                            //}

                            part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                            " onclick='handleButtonClick(this, " + item.Id + ")'>" + btnText?.Trim() + "</button>";


                        }

                        var part3 = "</div></div>";

                        messages.Add("{\"msg\" : \"" + part1 + part2 + part3 + "\"}");
                    }
                    else if (item.type == 4)
                    {

                        if (item.refName == "APPOINTMENTCONFIRMATION")
                        {
                            var doctorName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && (x.questionText == "Enter Dr. Name" || x.questionText == "Doctor search by department")).Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var uhid = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Uhid").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var name = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "FullName").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var email = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Email").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var phone = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Phone").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var drdesignation = await _context.DoctorInfos.Where(x => x.name == doctorName && x.branchInfoId == user.branchId).Select(x => x.designationName).AsNoTracking().LastOrDefaultAsync();
                            var AppointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "AppointDate").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var TimeSlot = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "TimeSlot").Select(x => x.answerText).AsNoTracking().LastOrDefaultAsync();
                            var userWithUhid = await _context.UserInfos.Where(x => x.UHID == uhid).FirstOrDefaultAsync();
                            var part1 = "";
                            if (userWithUhid != null)
                            {
                                part1 = "<div style='float: left; width: 300px;'><div class='bluedualcard3 graycard whitebluecard drdtal drppt' style='width: 100%; flex: 0 0 100%; max-width: 320px; padding: 0px;'><div class='drdtal-top' style='margin-top: 10px; '><div class='drdtal-topleft'><img src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p style='font-size: 11px;'>Appointment with</p><p class='drname'>" + doctorName + "</p><p class='uhidData' style='font-size: 12px;'>" + "<b>" + "UHID: " + "</b>" + uhid + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Name : " + "</b>" + userWithUhid.FullName + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Email : " + "</b>" + userWithUhid.Email + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Phone : " + "</b>" + userWithUhid.Mobile + "</p><span style='font-size: 11px; width: 100%; float: left; margin-bottom: 2px; color: rgb(34, 34, 34); '>" + drdesignation + "</span></div></div><a class='btn btn-btm btnbooking1' style='background: rgb(0, 35, 85);  width: 100%; padding-bottom: 5px;'><div class='yellowdate' style='width: 50%; float: left; padding: 7px 5px 5px; -webkit-box-flex: 1; flex: 1 1 auto; text-align: center; display: block; margin-bottom: 3px; font-size: 12px; border-right: 1px solid rgb(84, 34, 103); '><strong style='display: block; font-size: 16px; margin-bottom: 5px; '>  " + Convert.ToDateTime(AppointDate).DayOfWeek.ToString() + "  </strong>" + Convert.ToDateTime(AppointDate).ToString("dd-MMM-yyyy") + "</div><div class='yellow_time' style='font-size: 22px; padding: 12px 0px 12px; width: 50%; float: right; -webkit-box- flex: 1; flex: 1 1 auto; '><span class='glyphicon glyphicon-time' style='position: relative; margin-top: 4px; font-size: 22px; top: 5px; '></span> " + TimeSlot + "</div></a></div></div>";

                            }
                            else
                            {
                                part1 = "<div style='float: left; width: 300px;'><div class='bluedualcard3 graycard whitebluecard drdtal drppt' style='width: 100%; flex: 0 0 100%; max-width: 320px; padding: 0px;'><div class='drdtal-top' style='margin-top: 10px; '><div class='drdtal-topleft'><img src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p style='font-size: 11px;'>Appointment with</p><p class='drname'>" + doctorName + "</p><p class='uhidData' style='font-size: 12px;'>" + "<b>" + "UHID: " + "</b>" + uhid + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Name : " + "</b>" + name + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Email : " + "</b>" + email + "</p><p class='' style='font-size: 12px;'>" + "<b>" + "Phone : " + "</b>" + phone + "</p><span style='font-size: 11px; width: 100%; float: left; margin-bottom: 2px; color: rgb(34, 34, 34); '>" + drdesignation + "</span></div></div><a class='btn btn-btm btnbooking1' style='background: rgb(0, 35, 85);  width: 100%; padding-bottom: 5px;'><div class='yellowdate' style='width: 50%; float: left; padding: 7px 5px 5px; -webkit-box-flex: 1; flex: 1 1 auto; text-align: center; display: block; margin-bottom: 3px; font-size: 12px; border-right: 1px solid rgb(84, 34, 103); '><strong style='display: block; font-size: 16px; margin-bottom: 5px; '>  " + Convert.ToDateTime(AppointDate).DayOfWeek.ToString() + "  </strong>" + Convert.ToDateTime(AppointDate).ToString("dd-MMM-yyyy") + "</div><div class='yellow_time' style='font-size: 22px; padding: 12px 0px 12px; width: 50%; float: right; -webkit-box- flex: 1; flex: 1 1 auto; '><span class='glyphicon glyphicon-time' style='position: relative; margin-top: 4px; font-size: 22px; top: 5px; '></span> " + TimeSlot + "</div></a></div></div>";

                            }

                            messages.Add("{\"msg\" : \"" + part1 + "\"}");
                        }
                        else
                        {
                            var part1 = item.answer?.Trim();
                            messages.Add("{\"msg\" : \"" + part1 + "\"}");
                        }

                    }
                    else if (item.type == 5)
                    {
                        var part1 = "<div style='width: 100%'><iframe width='100%' height='250' frameborder='0' scrolling='no' marginheight='0' marginwidth='0' src='";

                        var part2 = item.answer?.Trim();

                        var part3 = ";output=embed'><a href='https://www.maps.ie/population/'>Population calculator map</a></iframe></div>";

                        messages.Add("{\"msg\" : \"" + part1 + part2 + part3 + "\"}");
                    }
                    else if (item.type == 6)
                    {
                        var part1 = "<div class='container'><div class='autoplay' style='display: flex;justify-content: center;overflow-x: scroll;'>";

                        var part2 = "";

                        var cards = await _context.CardGroupDetails.Where(x => x.cardGroupMaster.menuId == item.Id && x.branchInfoId == user.branchId && x.status == 1).AsNoTracking().OrderBy(x => x.sortOrder).ToListAsync();

                        foreach (var card in cards)
                        {
                            part2 += "<div class='card-box'><img src='/chatbox/" + card.cardThumb + "' alt='img' style='width: 100px;'><h4 class='card-title'>" + card.cardTitle + "</h4></div>";
                        }

                        var part3 = "</div></div>";

                        if (cards.Count() > 0)
                        {
                            messages.Add("{\"msg\" : \"" + part1 + part2 + part3 + "\"}");
                        }
                    }
                    else if (item.type == 7)
                    {
                        var part1 = "<form id='frmBasic' class='frmBasic' style= 'background-color: white;  padding: 8px;'>";

                        var part2 = "";
                        var part3 = "";
                        var part5 = "";
                        var part6 = "";
                        var inpGroups = await _context.InputGroupDetails.Include(x => x.master).Where(x => x.master.menuId == item.Id && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();
                        var inpMaster = await _context.InputGroupMasters.Where(x => x.menuId == item.Id && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();


                        if (item.refName == "RregisteredMobile")
                        {
                            part2 += "<div class='formMobile'>" + "<div class='input-group' style='width: 100%;' id='phone'>" +
                                               "<div class='input-group-prepend'>" +
                                                   "<span class='input-group-text' style='font-size: 11px; color: rgb(51, 51, 51); padding-bottom: 10px; line-height: 0.9;'>Enter Phone Number</span>" +
                                               "</div>" +
                                               "<div class='input-container'>" +
                                                "<select class='custom-select form-control' id='inputGroupSelect04' name='numberCode' style='width:70px;'>" +
                                        "<option value='+880' selected>+880</option><option value='+91'>+91</option><option value='+971'>+971</option>" +
                                        "</select>" +
                                               "<input type='text' maxlength='10' minlength='10' onkeyup='validatePhone(this)' style ='border: 1px solid lightgrey;cursor: pointer;' class='form-control' id='valueText' name='valueText' placeholder='Enter your mobile number' aria-label='Name' aria-describedby='basic-addon1' required><a id='phonebtn' onclick='SubmitInputMobileField(" + item.Id + "," + item.nextNodeId + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='phonebtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                "</div>" +
                                               "<input type='hidden' name='botKey' id='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' id='connectionId' value='" + connectionId + "' />" + "<input type='hidden' id='nextNodeId' name='nextNodeId' value='" + item.Id + "' />" +
                                               "<div class='input-group-prepend'>" + "<span class='input-group-text' style='font-size: 11px; color: rgb(205, 9, 9); padding-bottom: 10px; line-height: 0.9; padding-top: 8px;'>(Do not put 0 before mobile number)</span>" + "</div>" +
                                           "</div>" + "</div>";
                        }
                        else if (item.refName == "haveUHID")
                        {
                            part2 += "<div class='formMobile'>" + "<div class='input-group' style='width: 100%;' id='uhid'>" +
                                               "<div class='input-group-prepend'>" +
                                                   "<span class='input-group-text' style='font-size: 11px; color: rgb(51, 51, 51); padding-bottom: 10px; line-height: 0.9;'>Enter Your UHID</span>" +
                                               "</div>" +
                                               "<div class='input-container'>" +

                                               "<input type='text' style ='border: 1px solid lightgrey;cursor: pointer;' class='form-control uhidData' name='valueText' id='valueText' placeholder='Enter your UHID' aria-label='Name' aria-describedby='basic-addon1' required><a id='phonebtn' onclick='SubmitInputUhidField(" + item.Id + "," + item.nextNodeId + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='phonebtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                "</div>" +
                                                "<input type='hidden' name='botKey' id='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' id='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' id='nextNodeId' value='" + item.Id + "' />" +
                                               "<div class='input-group-prepend'>" + "<span class='input-group-text' style='font-size: 11px; color: rgb(205, 9, 9); padding-bottom: 10px; line-height: 0.9; padding-top: 8px;'></span>" + "</div>" +
                                           "</div>" + "</div>";
                        }
                        else
                        {

                            if (inpMaster.Id == 6 || inpMaster.Id == 2 || inpMaster.Id == 8 || inpMaster.Id == 10)
                            {
                                foreach (var inp in inpGroups)
                                {

                                    part2 += "<div class='formMobile'>" + "<div class='input-group' style='width: 100%;'>" +
                                               "<div class='input-group-prepend'>" +
                                                   "<span class='input-group-text' style='font-size: 11px; color: rgb(51, 51, 51); padding-bottom: 10px; line-height: 0.9;'>Enter Phone Number</span>" +
                                               "</div>" +
                                               "<div class='input-container'>" +
                                                "<select class='custom-select form-control' id='inputGroupSelect04' name='numberCode' style='width:70px;'>" +
                                        "<option value='+880' selected>+880</option><option value='+91'>+91</option><option value='+971'>+971</option>" +
                                        "</select>" +
                                               "<input type='text' maxlength='10' minlength='10' onkeyup='validatePhone(this)' style ='border: 1px solid lightgrey;cursor: pointer;' class='form-control' id='phoneValue' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='Name' aria-describedby='basic-addon1' required><a id='phonebtn' onclick='SubmitInputData(" + item.Id + "," + item.nextNodeId + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='phonebtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                "</div>" +
                                               "<input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                               "<div class='input-group-prepend'>" + "<span class='input-group-text' style='font-size: 11px; color: rgb(205, 9, 9); padding-bottom: 10px; line-height: 0.9; padding-top: 8px;'>(Do not put 0 before mobile number)</span>" + "</div>" +
                                           "</div>" + "</div>";


                                }
                            }
                            else
                            {
                                foreach (var inp in inpGroups)
                                {
                                    part5 = "<div class='formMobile'>" + "<div class='stepitem'>" +
                                       "<p id='step1'>(1/4)</p>" + "<p id='step2' style='display:none;'>(2/4)</p>" + "<p id='step3'  style='display:none;'>(3/4)</p>" + "<p id='step4'  style='display:none;'>(4/4)</p>" +
                                       "</div>";

                                    if ((inp.masterId == 1 || inp.masterId == 7) && inp.inputName == "Full Name")
                                    {
                                        part2 += "<div class='input-group' style='width: 100%;' id='name'>" +
                                                                                   "<div class='input-container'>" +
                                                                                   "<input type='text' style='border: 1px solid lightgrey;cursor: pointer;' class='form-control' id='nameValue' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='Name' aria-describedby='basic-addon1' oninput='validateInput()' required><a id='namebtn' onclick='SubmitInputField(1,  " + item.Id + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='namebtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                                                    "</div>" +
                                                                                   "<input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                                                               "</div>";
                                    }
                                    else if ((inp.masterId == 1 || inp.masterId == 7) && inp.inputName == "Phone")
                                    {
                                        part2 += "<div class='input-group' style='width: 100%; display: none;' id='phone'>" +
                                               "<div class='input-container'>" +
                                                "<select class='custom-select form-control' id='inputGroupSelect04' style='width:70px;' name='numberCode'>" +
                                        "<option value='+880' selected>+880</option><option value='+91'>+91</option><option value='+971'>+971</option>" +
                                        "</select>" +
                                               "<input type='text' maxlength='10' onkeyup='validatePhone(this)' style='border: 1px solid lightgrey;cursor: pointer;' class='form-control' id='phoneValue' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='Name' aria-describedby='basic-addon1' required><a id='phonebtn' onclick='SubmitInputField(2, " + item.Id + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='phonebtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                "</div>" +
                                               "<input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                               "<div class='input-group-prepend'>" + "<span class='input-group-text' style='font-size: 11px; color: rgb(205, 9, 9); padding-bottom: 10px; line-height: 0.9; padding-top: 8px;'>(Do not put 0 before mobile number)</span>" + "</div>" +
                                           "</div>";
                                    }
                                    else if ((inp.masterId == 1 || inp.masterId == 7) && inp.inputName == "Email")
                                    {
                                        part2 += "<div class='input-group' style='width: 100%; display: none;' id='email'>" +
                                                                                   "<div class='input-container'>" +
                                                                                   "<input type='text' onkeyup='validateEmail(this)' style='border: 1px solid lightgrey;cursor: pointer;' class='form-control' id='emailValue' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='email' aria-describedby='basic-addon1' required><a id='emailbtn' onclick='SubmitInputField(3, " + item.Id + ")' value='Submit' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='emailbtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                                                    "</div>" +
                                                                                   "<input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                                                               "</div>";
                                    }
                                    else
                                    {
                                        part2 += "<div class='input-group' style='width: 100%; display: none;' id='dob'>" +
                                                                                   "<div class='input-container'>" +
                                                                                   "<input type='date' id='dobValue' style='border: 1px solid lightgrey;cursor: pointer;' class='form-control' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='date' aria-describedby='basic-addon1'  max='2020-12-31' required><a id='dobbtn' onclick='SubmitInputDataField(" + item.Id + "," + item.nextNodeId + ")' type='button' class='btn btn-info btn-md' style='border-radius: 0px; height: 40px;'><span class='glyphicon glyphicon-chevron-right' style='color: white;padding-top: 5px;'></span></a><a id='dobbtn2' class='btn btn-success btn-md' style='border-radius: 0px; height: 40px; display:none;'><span class='glyphicon glyphicon-ok' style='color: white;padding-top: 5px;'></span></a>" +
                                                                                    "</div>" +
                                                                                   "<input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                                                               "</div>";
                                    }

                                    part6 = "</div>";

                                }
                                // part3 += "<input type='button' style='height: 40px; margin: 5px; border-radius:4px; display: none;' onclick='SubmitInputData(" + item.Id + "," + item.nextNodeId + ")' value='Submit' class='btn btn-info btn-md mt-2' />";

                            }
                        }


                        var part4 = "</form>";

                        //if(item.refName== "NoRregisteredMobile")
                        //{
                        //    messages.Add("{\"msg\" : \"" + part1 + part2 + part6 + part3 + part4 + "\"}");

                        //}
                        if (item.refName == "RregisteredMobile")
                        {
                            messages.Add("{\"msg\" : \"" + part1 + part2 + part6 + part3 + part4 + "\"}");

                        }
                        else if (item.refName == "haveUHID")
                        {
                            messages.Add("{\"msg\" : \"" + part1 + part2 + part6 + part3 + part4 + "\"}");

                        }
                        else
                        {
                            messages.Add("{\"msg\" : \"" + part5 + part1 + part2 + part6 + part3 + part4 + "\"}");

                        }


                    }

                    else if (item.type == 101)
                    {
                        var part0 = "<div><div class='dr-box' style='align-content: center;background-color: white !important;bottom: -10px !important;margin-bottom: -10px !important;width: 400px !important;margin-left: -10px !important;margin-top: -10px;padding: 10px;' >";

                        var part1 = "";

                        var doctorName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Please enter doctor name").OrderByDescending(x => x.DateTime).Select(x => x.answerText).Take(1).AsNoTracking().FirstOrDefaultAsync();
                        var departmentName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Please enter department name").OrderByDescending(x => x.DateTime).Select(x => x.answerText).Take(1).AsNoTracking().FirstOrDefaultAsync();
                        var specializationsName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.questionText == "Please enter specializations name").OrderByDescending(x => x.DateTime).Select(x => x.answerText).Take(1).AsNoTracking().FirstOrDefaultAsync();

                        var part2 = "";



                        if (item.departmentId != null)
                        {
                            if (item.departmentId != null && departmentName != null)
                            {
                                //var serviceFlowQuestions = await _context.keyWordQuesAns.Where(x => x.departmentId != null && x.branchInfoId == user.branchId && x.question.ToLower().Trim().Contains(departmentName.ToLower().Trim())).ToListAsync();
                                //departments = await _context.DepartmentInfos.Where(x => serviceFlowQuestions.Select(y => y.departmentId).Contains(x.Id) && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

                                var serviceFlowQuestions = await _context.keyWordQuesAns
                                                                    .Where(x => x.departmentId != null
                                                                                && x.branchInfoId == user.branchId
                                                                                && x.question.ToLower().Trim().Contains(departmentName.ToLower().Trim()))
                                                                    .OrderByDescending(x => x.question.ToLower().Trim() == departmentName.ToLower().Trim()) // Exact matches first
                                                                    .ThenBy(x => x.question.Length) // Shorter terms first
                                                                    .ThenBy(x => x.question) // Alphabetical order as secondary criteria
                                                                    .ToListAsync();

                                var departmentIds = serviceFlowQuestions.Select(y => y.departmentId).Distinct().ToList();

                                departments = await _context.DepartmentInfos
                                                       .Where(x => departmentIds.Contains(x.Id) && x.branchInfoId == user.branchId)
                                                       .OrderByDescending(x => serviceFlowQuestions.Any(y => y.departmentId == x.Id && y.question.ToLower().Trim() == departmentName.ToLower().Trim())) // Prioritize exact match
                                                       .ThenBy(x => x.departmentName.Length) // Shorter names first
                                                       .ThenBy(x => x.departmentName) // Alphabetical order
                                                       .AsNoTracking()
                                                       .ToListAsync();


                            }
                            else
                            {
                                //doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.DepartmentId == item.departmentId).ToListAsync();
                                departments = await _context.DepartmentInfos.Where(x => x.botKey == botKey && x.Id == item.departmentId && x.branchInfoId == user.branchId).ToListAsync();
                                //messages.Add("{\"msg\" : \"" + "Choose a department for appointment." + "\"}");
                            }

                        }
                        else if (item.specializationId != null)
                        {
                            if (item.specializationId != null && specializationsName != null)
                            {

                                var keyWordQuesAnsdata = _context.keyWordQuesAns.Where(x => x.specializationId != null).Select(x => x.Id).ToList();
                                var textArr = await _context.BotKnowledges
                                    .AsNoTracking()
                                    .Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && keyWordQuesAnsdata.Contains((int)x.keyWordQuesAnsId))
                                    .ToListAsync();

                                var data = new List<TextSimilarityVm>();
                                foreach (var text in textArr)
                                {
                                    double similarity = TextMatcher.CalculateLevenshteinSimilarity(specializationsName.ToLower(), text.question.ToLower());

                                    if (text.question.ToLower().IndexOf(specializationsName.ToLower()) >= 0)
                                    {
                                        similarity = 1;
                                    }

                                    if (similarity >= 0.70 && similarity <= 1) // Check if similarity is within range
                                    {
                                        data.Add(new TextSimilarityVm
                                        {
                                            text = text.question,
                                            percentage = similarity,
                                            KnowledgeId = (int)text.keyWordQuesAnsId
                                        });

                                        if (similarity == 1) // Stop if exact match
                                        {
                                            break;
                                        }
                                    }
                                }

                                // Get the KnowledgeId with the highest percentage
                                var maxPercentageKnowledgeId = data.OrderByDescending(x => x.percentage).ToList();

                                // Fix the query to filter based on maxPercentageKnowledgeId correctly
                                if (maxPercentageKnowledgeId.Count > 0)
                                {
                                    var maxKnowledgeIds = maxPercentageKnowledgeId.Select(x => x.KnowledgeId).ToList();

                                    questionsByKeyword = await _context.keyWordQuesAns
                                        .Where(x => x.isDelete != 1
                                                && x.branchInfoId == user.branchId
                                                && maxKnowledgeIds.Contains(x.Id))
                                        .AsNoTracking()
                                        .ToListAsync();
                                }
                                else
                                {
                                    questionsByKeyword = new List<KeyWordQuesAns>(); // Or appropriate fallback
                                }

                                var specializationIds = questionsByKeyword.Select(y => y.specializationId).Distinct().ToList();

                                specializations = await _context.DoctorSpecializations
                                                       .Where(x => specializationIds.Contains(x.Id) && x.branchInfoId == user.branchId)
                                                       .OrderByDescending(x => questionsByKeyword.Any(y => y.specializationId == x.Id && y.question.ToLower().Trim() == specializationsName.ToLower().Trim())) // Prioritize exact match
                                                       .ThenBy(x => x.name.Length) // Shorter names first 
                                                       .AsNoTracking()
                                                       .ToListAsync();


                            }
                            else
                            {
                                specializations = await _context.DoctorSpecializations.Where(x => x.botKey == botKey && x.Id == item.specializationId && x.branchInfoId == user.branchId).ToListAsync();
                                //messages.Add("{\"msg\" : \"" + "Choose a department for appointment." + "\"}");
                            }

                        }

                        else if (doctorName != null)
                        {
                            var serviceFlowQuestions = await _context.keyWordQuesAns.Where(x => x.doctorId != null && x.branchInfoId == user.branchId && x.question.ToLower().Trim().Contains(doctorName.ToLower().Trim())).ToListAsync();

                            doctors = await _context.DoctorInfos.Where(x => serviceFlowQuestions.Select(y => y.doctorId).Contains(x.Id) && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

                        }
                        else if (item.doctorId != null)
                        {
                            doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.Id == item.doctorId && x.branchInfoId == user.branchId && x.isDelete != 1).ToListAsync();

                            //messages.Add("{\"msg\" : \"" + "Choose the doctor for appointment." + "\"}");
                        }
                        else if (item.refName == "SPECIFICDOCTOR")
                        {
                            string searchTxt = await _context.ServiceFlows.Where(x => x.answerText != null && x.questionText == "Please enter doctor name" && x.branchInfoId == user.branchId && x.connectionId == connectionId).AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).LastOrDefaultAsync();

                            doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.name.Contains(searchTxt) && x.branchInfoId == user.branchId && x.isDelete != 1).ToListAsync();

                            messages.Add("{\"msg\" : \"" + "Choose the doctor for appointment." + "\"}");
                        }
                        else
                        {
                            if (item.Id == 173 || item.Id == 1503 || item.Id == 1833 || item.Id == 1988)
                            {

                                //departments = await _context.DepartmentInfos.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId).ToListAsync();

                                var facility = "";
                                if (user.branchId == 1)
                                {
                                    facility = "EHD";

                                }
                                else
                                {
                                    facility = "EHC";
                                }
                                var departmentNamesWithDoctors = await _context.ApiDoctors
                                                                        .Where(x => x.facility == facility)
                                                                        .Select(x => x.department)
                                                                        .Distinct()
                                                                        .ToListAsync();

                                departments = await _context.DepartmentInfos
                                                     .Where(x => x.botKey == botKey && x.branchInfoId == user.branchId &&
                                                                 departmentNamesWithDoctors.Contains(x.departmentName))
                                                     .ToListAsync();
                            }
                            else
                            {
                                doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId).ToListAsync();
                            }
                        }



                        if (doctors.Count() == 0 && item.departmentId == null && item.specializationId == null && item.Id != 1503 && item.Id != 1988)
                        {
                            doctors = await _context.DoctorInfos.Where(x => x.botKey == botKey && x.branchInfoId == user.branchId && x.isDelete != 1).ToListAsync();
                        }




                        //if (departments.Count() > 0)
                        //{
                        //    foreach (var dept in departments)
                        //    {
                        //        departmentCards += "<div onclick='handleDeptCardClick(this)' class='graycard drdtal' style='background: white;box-shadow: 0 0 5px 2px rgba(0,0,0,.2) !important; border-radius: 6px;border-top: 2px solid blue;padding: 10px; width: 280px !important;' ><div class='drdtal-top'> <div class='drdtal-topright' style='text-align: center;'><img class='' src='/" + dept.thumbUrl + "' alt='' width='60px' style='margin-bottom: 10px;margin-top: -6px;'><p class='deptName' style='text-align: center;'>" + dept.departmentName + "</p><small></small> <a class='btn btn-btm btn-sm' href='javascript:void(0);' style='margin:1px 10px;width: 95%;'>Request Appointment</a></div></div></div>";
                        //    }

                        //    part2 = "</div><button class='dr-box-left' onclick='deptboxLeft()'><</button><button class='dr-box-right' onclick='deptboxRight()'>></button></div>";

                        //}

                        if (specializations.Count() > 0)
                        {
                            foreach (var spec in specializations)
                            {
                                var specName = spec.name;
                                specializationsCards += "<div class='graycard drdtal specboxData' style='background: white;box-shadow: 0 0 5px 2px rgba(0,0,0,.2) !important; border-radius: 6px;border-top: 2px solid blue;padding: 10px; width: 280px !important;' ><div class='drdtal-top'> <div class='drdtal-topright' style='text-align: center;'><p class='specName' style='text-align: center;'>" + spec.name + "</p><small></small> <a class='btn btn-btm btn-sm' id='specCardbtn' onclick='handleSpecCardClick(this)' style='margin:1px 10px;width: 95%;'>Request Appointment <p class='specName' style='text-align: center;display:none;'>" + spec.name + "</p></a></div></div></div>";
                            }

                            part2 = "</div><button class='dr-box-left' onclick='specboxLeft()'><</button><button class='dr-box-right' onclick='specboxRight()'>></button></div>";

                        }

                        else if (doctors.Count() > 0)
                        {


                            foreach (var doc in doctors.Take(1).ToList())
                            {
                                var slots = await Fetch7DaysSlot(botKey, (int)doc.ApiDoctorId);

                                var dateList = "";
                                foreach (var date in slots.Where(x => x.appointmentDate != "").Select(x => x.appointmentDate).Distinct())
                                {
                                    var strDate = Convert.ToDateTime(date).ToString("dd MMM");

                                    dateList += "<div class='doc-avalday-new' data-tooltip='Available'>" + strDate + "</div>";
                                }

                                doctorCards += "<div id='drBoxApiId_" + (int)doc.ApiDoctorId + "' onclick='handleDrCardClick(this)' class='graycard drdtal' style='align-content: center; background: white;border-radius: 6px; box-shadow: 0 0 5px 2px rgba(0,0,0,.2)!important;border-top: 2px solid blue;padding: 10px;'><div class='drdtal-top'><div class='loader'></div><div class='drdtal-topleft' style='width:75px;float:left;padding:5px;-webkit-box-flex:1;position:relative'><span class='spangender'>M</span><img class='doc-pic' src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p class='drname'>" + doc.name + "</p><p style='display: none' id='drApiId_" + (int)doc.ApiDoctorId + "' class='drApiId'> " + (int)doc.ApiDoctorId + "</p><span class='doc-subhead'>" + doc.departmentName + "</span></div></div><div class='doc-aval'><span class='doc-avaltxt'>Availability</span>" + dateList + "</div><a class='btn btn-btm' href='javascript:void(0);' style='margin-left:50px;margin-top: 10px'>Request Appointment</a></div>";
                            }



                            //part2 = "</div><button class='dr-box-left' onclick='drboxLeft(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'><</button><button class='dr-box-right' onclick='drboxRight(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'>></button></div>";
                            part2 = "</div><button class='dr-box-left' onclick='drboxLeft2(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'><</button><button class='dr-box-right' onclick='drboxRight2(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'>></button></div>";









                            //foreach (var doc in doctors)
                            //{
                            //    doctorCards += "<div onclick='handleDrCardClick(this)' class='graycard drdtal' style='align-content: center; background: white; box-shadow: 0 0 5px 2px rgba(0,0,0,.2)!important;border-radius: 6px;border-top: 2px solid blue;padding: 10px; width: 300px !important;'><div class='drdtal-top'><div class='drdtal-topleft' style='width:75px;float:left;padding:5px;-webkit-box-flex:1;position:relative'><span class='spangender'>M</span><img class='doc-pic' src='https://chatbot.hlthclub.in/static/doctor-male.png' alt='' width='60px'></div><div class='drdtal-topright'><p class='drname'>" + doc.name + "</p><p style='display: none' class='drApiId'> " + doc.ApiDoctorId + "</p><span class='doc-subhead'>" + doc.departmentName + "</span></div></div><div class='doc-aval'><span class='doc-avaltxt'>Availability</span><div class='doc-avalday' data-tooltip='Available'>m</div><div class='doc-avalday' data-tooltip='Available'>t</div><div class='doc-avalday' data-tooltip='Available'>W</div><div class='doc-avalday' data-tooltip='Available'>T</div><div class='doc-avalday doc-avalday-not' data-tooltip='Not Available'>F</div><div class='doc-avalday' data-tooltip='Available'>S</div><div class='doc-avalday' data-tooltip='Available'>S</div></div><a class='btn btn-btm' href='javascript:void(0);' style='margin:30px 10px;width: 95%;'>Request Appointment</a></div>";
                            //}
                            //part2 = "</div><button class='dr-box-left' onclick='drboxLeft()'><</button><button class='dr-box-right' onclick='drboxRight()'>></button></div>";

                        }
                        else
                        {

                        }




                        //messages.Add("{\"msg\" : \"" + part0 + part1 + part2 + "\"}");


                        int stepNo = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).AsNoTracking().OrderByDescending(x => x.StepNo).Select(x => x.StepNo).FirstOrDefaultAsync();

                        if (item.answer != null)
                        {
                            int qId = await _context.keyWordQuesAns.Where(x => x.answer.Trim() == item.answer.Trim() && x.branchInfoId == user.branchId).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync();

                            var sflow = new ServiceFlow
                            {
                                Id = Guid.NewGuid().ToString(),
                                InfoType = "start",
                                ServiceCode = "Pre-Defined Question",
                                connectionId = connectionId,
                                DateTime = DateTime.Now,
                                Attempt = 0,
                                StepNo = stepNo + 1,
                                botKey = botKey,
                                status = 0,
                                questionText = item.answer, //this is question for now
                                answerText = null,
                                MenuId = 0,
                                keyWordQuesAnsId = qId,
                                branchInfoId = user.branchId
                            };

                            try
                            {
                                _context.ServiceFlows.Add(sflow);
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {

                            }
                        }






                    }
                    else if (item.type == 110)
                    {
                        if (item.refName == "NEXT7DAYS")
                        {
                            var doctorServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && (x.answerText != null && (x.questionText == "Enter Dr. Name" || x.questionText == "Please enter doctor name" || x.questionText == "Doctor search by department"))).AsNoTracking().LastOrDefaultAsync();
                            var doctorWeeks = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfo.name == doctorServiceFlow.answerText && x.branchInfoId == user.branchId).Select(x => x.weeks.name).AsNoTracking().Distinct().ToListAsync();
                            DateTime currentDate = DateTime.Now;
                            List<string> next7Days = new List<string>();
                            for (int i = 1; i < 30; i++)
                            {
                                DateTime nextDate = currentDate.AddDays(i);

                                if (doctorWeeks.Contains(nextDate.DayOfWeek.ToString()))
                                {
                                    next7Days.Add(nextDate.ToString("yyyy-MM-dd"));
                                }

                                if (next7Days.Count() >= 7)
                                {
                                    break;
                                }
                            }

                            var part1 = "<div class='NEXT7DAYS' style='display: inline-block; margin: 1px;'>";

                            var part2 = "";
                            foreach (var btn in next7Days)
                            {
                                part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                    " onclick='handleAppointmentDateClick(this, " + item.Id + ")'>" + btn?.Trim() + "</button>";
                            }

                            var part3 = "</div>";

                            messages.Add("{\"msg\" : \"" + "<p>Please select the date for appointment</p>" + "\"}");
                            messages.Add("{\"msg\" : \"" + part1 + part2 + part3 + "\"}");
                        }



                        if (item.refName == "TIMESLOTS")
                        {
                            var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && (x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr"))).AsNoTracking().OrderBy(x => x.DateTime).LastOrDefaultAsync();

                            var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText && x.botKey == botKey && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                            //var appointment = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id).Select(x => x.time).AsNoTracking().FirstOrDefaultAsync();
                            var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.branchInfoId == user.branchId && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();

                            int weekId = 1;

                            #region Slots Api Call
                            if (docInfo != null && appointDate != null)
                            {
                                var doctorId = docInfo.ApiDoctorId;
                                var date = Convert.ToDateTime(appointDate).ToString("yyyy-MM-dd");
                                var branch = docInfo.branchInfoId == 1 ? "EHD" : "EHC";

                                var weekDay = Convert.ToDateTime(appointDate).DayOfWeek.ToString();
                                weekId = _context.Weeks.Where(x => x.name == weekDay && x.branchInfoId == docInfo.branchInfoId).AsNoTracking().Select(x => x.Id).FirstOrDefault();

                                var token = await GetActiveToken();
                                string bearerToken = token.token;
                                string baseDoctorSlots2Url = "https://Applink.evercarebd.com:8018/api/DoctorSlot";

                                var doctorSlotUrl2 = $"{baseDoctorSlots2Url}/{branch}/{doctorId}/{date}";
                                var doctorSlot2 = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(doctorSlotUrl2, bearerToken);
                                InsertDoctorSlots(doctorSlot2, docInfo, Convert.ToDateTime(date));
                            }

                            #endregion

                            var bookedTime = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id && x.branchInfoId == user.branchId && Convert.ToDateTime(x.date).Date == Convert.ToDateTime(appointDate).Date).Select(x => x.time).AsNoTracking().ToListAsync();

                            var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id && x.weeksId == weekId && x.branchInfoId == user.branchId && !bookedTime.Contains(x.timePeriod.timeSlot)).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.timeSlot).Select(x => x.timePeriod).AsNoTracking().Distinct().ToListAsync();

                            var doctorName = "doctorName";

                            var part0 = "<div class='w3-bar w3-black' style='background: #8EE9F5; margin-left: -9px; margin-right: -50px;margin-bottom: 10px;'><button style='background: #7EA7F3; width: 115px; height: 50px;border-radius:0px; border: none;' class='w3-bar-item w3-button' onclick='openshift(1, " + item.Id + ")'>Morning</button><button style='background: #8EE9F5;width: 115px; height: 50px;border-radius:0px; border: none;' class='btn btn-info' onclick='openshift(2, " + item.Id + ")'>Evening</button><button style='background: #F5918E;width: 116px; height: 50px;border-radius:0px; border: none;' class='w3-bar-item w3-button' onclick='ChangeSchedule(489 , " + connectionId + ", " + botKey + ")'>Change Date</button></div>";


                            var part0_1 = "<div id='morning' class='slotTime'>";
                            var part0_2 = "<div class='TIMESLOTS' style='display: inline-block; margin: 1px;'>";

                            var part0_3 = "";

                            foreach (var btn in slots)
                            {
                                if (btn.shiftPeriod == "Morning")
                                {
                                    part0_3 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                        " onclick='handleTimeSlotClick(this, " + item.Id + "," + btn.Id + ")'>" + btn.timeSlot?.Trim() + "</button>";
                                }
                            }
                            var part0_4 = "</div></div>";


                            var part1_1 = "<div id='evening' class='slotTime' style='display: none;' >";
                            var part1_2 = "<div class='TIMESLOTS' style='display: inline-block; margin: 1px;'>";

                            var part1_3 = "";

                            foreach (var btn in slots)
                            {
                                if (btn.shiftPeriod == "Evening")
                                {
                                    part1_3 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                        " onclick='handleTimeSlotClick(this, " + item.Id + "," + btn.Id + ")'>" + btn.timeSlot?.Trim() + "</button>";
                                }
                            }

                            var part1_4 = "</div></div>";



                            messages.Add("{\"msg\" : \"" + "Please select the time slot for appointment." + "\"}");
                            messages.Add("{\"msg\" : \"" + part0 + part0_1 + part0_2 + part0_3 + part0_4 + part1_1 + part1_2 + part1_3 + part1_4 + "\"}");

                        }

                        if (item.refName == "RESCHEDULE")
                        {
                            var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).AsNoTracking().Where(x => x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr")).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                            var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText && x.botKey == botKey && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                            //var appointment = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id).Select(x => x.time).AsNoTracking().FirstOrDefaultAsync();

                            var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                            var bookedTime = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id && x.branchInfoId == user.branchId && Convert.ToDateTime(x.date).Date == Convert.ToDateTime(appointDate).Date).Select(x => x.time).AsNoTracking().ToListAsync();

                            var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id && x.branchInfoId == user.branchId && !bookedTime.Contains(x.timePeriod.timeSlot)).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.sortOrder).Select(x => x.timePeriod).AsNoTracking().ToListAsync();


                            var part1 = "<div class='TIMESLOTS' style='display: inline-block; margin: 1px;'>";

                            var part2 = "";

                            foreach (var btn in slots)
                            {
                                part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                    " onclick='handleTimeSlotClick(this, " + item.Id + "," + btn.Id + ")'>" + btn.timeSlot?.Trim() + "</button>";
                            }

                            var part3 = "</div>";

                            messages.Add("{\"msg\" : \"" + "Please select the time slot for appointment." + "\"}");
                            messages.Add("{\"msg\" : \"" + part1 + part2 + part3 + "\"}");
                        }

                        if (item.refName == "APPOINTMENTCONFIRMATION")
                        {
                            //var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId).AsNoTracking().Where(x => x.questionText == "Enter Dr. Name").OrderBy(x => x.DateTime).LastOrDefaultAsync();

                            //var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText.Trim() && x.botKey == botKey).FirstOrDefaultAsync();

                            //var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.sortOrder).Select(x => x.timePeriod).AsNoTracking().ToListAsync();

                            var phoneNumber = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "Phone").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                            var uhid = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "Uhid").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                            var doctorName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && (x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr"))).AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                            var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                            var timeSlot = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "TimeSlot").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();


                            var docInfo = await _context.DoctorInfos.Include(x => x.Department).Include(x => x.doctorSpecialization).Where(x => x.name == doctorName.Trim() && x.branchInfoId == user.branchId && x.botKey == botKey).FirstOrDefaultAsync();
                            //var userInfo = await _context.UserInfos.Where(x => x.Mobile == phoneNumber.Trim() && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                            //var userInfo = await _context.UserInfos.Where(x => x.UHID == uhid.Trim() && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                            var userInfoId = "";

                            if (uhid != null)
                            {
                                var userInfo = await _context.UserInfos.Where(x => x.UHID == uhid.Trim() && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                                userInfoId = userInfo.Id;
                            }
                            else
                            {
                                var userInfos = await _context.UserInfos.Where(x => x.Mobile == phoneNumber.Trim() && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                                userInfoId = userInfos.Id;
                            }


                            var docapiId = docInfo.ApiDoctorId;
                            var appointData = new AppoinmentInfo
                            {
                                Id = 0,
                                // doctorInfoId = docInfo.Id,
                                doctorInfoId = docapiId,
                                userInfoId = userInfoId,
                                date = Convert.ToDateTime(appointDate),
                                botKey = botKey,
                                time = timeSlot,
                                status = 0,
                                isVerified = 0,
                                appointStatus = "Application",
                                entryby = "mehedi",
                                entryDate = DateTime.Now,
                                branchInfoId = user.branchId,
                                departmentName = docInfo.departmentName,
                                designationName = docInfo.designationName,
                                doctorName = docInfo.name,
                                specializationsName = docInfo.doctorSpecialization.name,
                            };
                            var appointment = await _context.AppoinmentInfos.Where(x => x.doctorName == appointData.doctorName && x.date.Value.Date == Convert.ToDateTime(appointDate).Date).FirstOrDefaultAsync();
                            if (appointment == null)
                            {


                                _context.AppoinmentInfos.Add(appointData);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {

                                // appointData.Id = appointment.Id;
                                //appointData.time = timeSlot;
                                appointment.time = timeSlot;
                                _context.AppoinmentInfos.Update(appointment);
                                await _context.SaveChangesAsync();
                            }



                            var chatData = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

                            string htmlContent = "<table>" +
                                                    "<tbody>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Full Name</td><td>: " + chatData.Where(x => x.questionText == "FullName").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Date of Birth</td><td>: " + chatData.Where(x => x.questionText == "DateOfBirth").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Sex</td><td>: " + chatData.Where(x => x.questionText == "Gender").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Mobile</td><td>: " + chatData.Where(x => x.questionText == "Phone").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Email</td><td style='font-size:10px;'>: " + chatData.Where(x => x.questionText == "Email").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Doctor</td><td>: " + doctorName + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: center' colspan='2'>Preferred Date and Time for Appointment</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Date</td><td>: " + Convert.ToDateTime(appointDate).ToString("dd-MMM-yyyy") + "</td></tr>" +
                                                            "<tr><td style='font-weight: bold; text-align: right;'>Time</td><td>: " + timeSlot + "</td></tr>" +
                            "</tbody>" +
                            "</table>";
                            var reply = "Thank you for your request. Our customer care officer will contact you for appointment confirmation";
                            //var reply = "Thanks for booking an appointment with us. Your booking details: Booking No. 2863523, " + doctorName + "," + docInfo.Department?.departmentName + "; Date: " + Convert.ToDateTime(appointDate).ToString("dd-MMM-yyyy") + "," + Convert.ToDateTime(appointDate).DayOfWeek.ToString() + "," + timeSlot + ".";

                            if (_configuration["Project:isLive"] == "YES")
                            {
                                smsAPI.Single_Sms(phoneNumber, reply);
                                await SendHTMLEmail(chatData.Where(x => x.questionText == "Email" && x.branchInfoId == user.branchId).Select(x => x.answerText).LastOrDefault(), "Chatbot Appointment Request", htmlContent);
                            }
                            else
                            {
                                await this.SendSMSAsync(phoneNumber, reply);
                                await SendHTMLEmail(chatData.Where(x => x.questionText == "Email" && x.branchInfoId == user.branchId).Select(x => x.answerText).LastOrDefault(), "Chatbot Appointment Request", htmlContent);
                            }

                            int btnClickId = 495;
                            if (user.branchId == 2)
                            {
                                btnClickId = 1980;
                            }

                            messages.Add("{\"msg\" : \"" + reply?.Trim() + "\"}");
                            messages.Add("{\"msg\" : \"" + "Please let us know, what more i can help you with." + "\"}");
                            messages.Add("{\"msg\" : \"" + "<div style='display: inline-block;background: white;margin-left:-10px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment</button><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment with Doctor</button><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment with specializations</button></div>" + "\"}");
                            //messages.Add("{\"msg\" : \"" + "<div style='display: inline-block;background: white;margin-left:-10px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>More Appointment</button></div>" + "\"}");
                        }
                    }
                    else
                    {
                    }
                }
                //if (departmentCards != "")
                //{
                //    messages.Add("{\"msg\" : \"" + "Choose a department for appointment." + "\"}");
                //    messages.Add("{\"msg\" : \"" + "<div><div class='dept-box' style='align-content: center; background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -27px !important;  margin-top: -6px; padding: 10px;  height: 400px'>" + departmentCards + "</div><button class='dept-box-left' onclick='deptboxLeft()'><</button><button class='dept-box-right' onclick='deptboxRight()'>></button></div>" + "\"}");
                //}
                if (specializationsCards != "")
                {
                    messages.Add("{\"msg\" : \"" + "Choose a specializations for appointment." + "\"}");
                    messages.Add("{\"msg\" : \"" + "<div><div class='dept-box' style='align-content: center; background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -27px !important;  margin-top: -6px; padding: 10px;  height: 200px'>" + specializationsCards + "</div><button class='dept-box-left' onclick='specboxLeft()'><</button><button class='dept-box-right' onclick='specboxRight()'>></button></div>" + "\"}");
                }
                else if (doctorCards != "")
                {
                    messages.Add("{\"msg\" : \"" + "Please select the doctor for appointment." + "\"}");
                    messages.Add("{\"msg\" : \"" + "<div><div class='dr-box' style='align-content: center;background-color: white !important; width: 400px !important;margin-bottom: -20px; margin-left: -27px !important;  margin-top:-6px; padding: 10px;  height: 400px'>" + doctorCards + "</div><button class='dr-box-left' onclick='drboxLeft2(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'><</button><button class='dr-box-right' onclick='drboxRight2(" + doctors.FirstOrDefault()?.ApiDoctorId + ")'>></button></div>" + "\"}");
                }
                return messages;
            }
            else
            {
                return messages;
            }
        }

        private void InsertDoctorSlots(List<DoctorSlotVm> doctorSlot2, DoctorInfo doctorInfo, DateTime appointmentDate)
        {
            var slots = _context.TimePeriods.AsNoTracking().ToList();

            var newSlots = doctorSlot2.Select(x => x.slotFrom).Distinct().ToList();

            var slotData = new List<TimePeriod>();

            foreach (var item in newSlots.Except(slots.Select(x => x.timeSlot).ToList()))
            {
                slotData.Add(new TimePeriod
                {
                    Id = 0,
                    shiftPeriod = (Convert.ToInt32(item.Substring(0, 2)) >= 14 && Convert.ToInt32(item.Substring(0, 2)) <= 23) ? "Evening" : "Morning",
                    timeSlot = item,
                    sortOrder = 100,
                    botKey = doctorInfo.botKey,
                    branchInfoId = doctorInfo.branchInfoId
                });
            }

            if (slotData.Count > 0)
            {
                _context.AddRange(slotData);
                _context.SaveChanges();
            }

            var weeks = _context.Weeks.AsNoTracking().Where(x => x.branchInfoId == doctorInfo.branchInfoId).ToList();
            var weekId = weeks.Where(x => x.name == appointmentDate.DayOfWeek.ToString()).Select(x => x.Id).FirstOrDefault();

            var doctorSlots = new List<DoctorVisitTimePeriod>();

            foreach (var item in doctorSlot2)
            {
                doctorSlots.Add(new DoctorVisitTimePeriod
                {
                    entryDate = DateTime.Now,
                    doctorInfoId = doctorInfo.Id,
                    timePeriodId = slots.Where(x => x.timeSlot.Trim() == item.slotFrom.Trim()).Select(x => x.Id).FirstOrDefault(),
                    status = 1,
                    weeksId = weekId,
                    botKey = doctorInfo.botKey,
                    isDelete = 0,
                    branchInfoId = doctorInfo.branchInfoId
                });
            }

            var existingSlots = _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == doctorInfo.Id && x.weeksId == weekId).AsNoTracking().ToList();

            _context.DoctorVisitTimePeriods.RemoveRange(existingSlots);
            _context.SaveChanges();


            //var newDocSlots = doctorSlots.Except(existingSlots);

            _context.DoctorVisitTimePeriods.AddRange(doctorSlots);
            _context.SaveChanges();

            //var existingSlots = _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == doctorInfo.Id && x.weeksId == weekId).AsNoTracking().ToList();


        }

        public async Task<List<string>> GetResponseByTypingText(string msg, string connectionId, string botKey, string userId)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();
            var wrapperDetails = await _context.ConnectionInfos.Where(x => x.userId == userId && x.branchInfoId == user.branchId).Include(x => x.wrapperDetails).AsNoTracking().FirstOrDefaultAsync();

            var result = new List<string>();
            if (msg != "menu")
            {
                if ((msg.ToLower() == "hi" || msg.ToLower() == "hello") && wrapperDetails.wrapperDetailsId == 1)
                {
                    var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.question.ToLower() == "Doctor search by department" && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                    //Log Message Start
                    var msglog = new MessageLog
                    {
                        Id = 0,
                        botKey = botKey,
                        connectionId = connectionId,
                        message = questionsByKeyword.answer,
                        menuId = null,
                        Type = msg == "menu" ? "Menu" : "text",
                        rawMessage = questionsByKeyword.answer,
                        entryDate = DateTime.Now,
                        KeyWordQuesAnsId = null,
                        nextNodeId = questionsByKeyword.nextNodeId,
                        branchInfoId = user.branchId
                    };
                    _context.MessageLogs.Add(msglog);
                    await _context.SaveChangesAsync();
                    //Log Message End


                    result = await this.SendNextMessageByNodeId(botKey, connectionId, questionsByKeyword.Id);
                }
                if ((msg.ToLower() == "hi" || msg.ToLower() == "hello") && wrapperDetails.wrapperDetails.firstMessage == "Doctor search by Specializations")
                {
                    var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.question.ToLower() == "Doctor search by Specializations" && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                    //Log Message Start
                    var msglog = new MessageLog
                    {
                        Id = 0,
                        botKey = botKey,
                        connectionId = connectionId,
                        message = questionsByKeyword.answer,
                        menuId = null,
                        Type = msg == "menu" ? "Menu" : "text",
                        rawMessage = questionsByKeyword.answer,
                        entryDate = DateTime.Now,
                        KeyWordQuesAnsId = null,
                        nextNodeId = questionsByKeyword.nextNodeId,
                        branchInfoId = user.branchId
                    };
                    _context.MessageLogs.Add(msglog);
                    await _context.SaveChangesAsync();
                    //Log Message End


                    result = await this.SendNextMessageByNodeId(botKey, connectionId, questionsByKeyword.Id);
                }


                else if ((msg.ToLower() == "hi" || msg.ToLower() == "hello") && wrapperDetails.wrapperDetailsId == 2)
                {
                    var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.question.ToLower() == "Search by doctor" && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                    //Log Message Start
                    var msglog = new MessageLog
                    {
                        Id = 0,
                        botKey = botKey,
                        connectionId = connectionId,
                        message = questionsByKeyword.answer,
                        menuId = null,
                        Type = msg == "menu" ? "Menu" : "text",
                        rawMessage = questionsByKeyword.answer,
                        entryDate = DateTime.Now,
                        KeyWordQuesAnsId = null,
                        nextNodeId = questionsByKeyword.nextNodeId,
                        branchInfoId = user.branchId
                    };
                    _context.MessageLogs.Add(msglog);
                    await _context.SaveChangesAsync();
                    //Log Message End


                    result = await this.SendNextMessageByNodeId(botKey, connectionId, questionsByKeyword.Id);
                }
                else
                {
                    var textArr = new List<BotKnowledge>();


                    if (wrapperDetails.wrapperDetailsId == 1)
                    {
                        textArr = await _context.BotKnowledges.AsNoTracking().Where(x => x.botKey == botKey && x.keyWordQuesAns.departmentId != null && x.branchInfoId == user.branchId).ToListAsync();
                    }
                    else if (wrapperDetails.wrapperDetailsId == 1)
                    {
                        textArr = await _context.BotKnowledges.AsNoTracking().Where(x => x.botKey == botKey && x.keyWordQuesAns.departmentId != null && x.branchInfoId == user.branchId).ToListAsync();
                    }
                    else if (wrapperDetails.wrapperDetailsId == 2)
                    {
                        textArr = await _context.BotKnowledges.AsNoTracking().Where(x => x.botKey == botKey && x.keyWordQuesAns.doctorId != null && x.branchInfoId == user.branchId).ToListAsync();
                    }
                    else
                    {
                        textArr = await _context.BotKnowledges.AsNoTracking().Where(x => x.botKey == botKey && x.branchInfoId == user.branchId).ToListAsync();
                    }

                    var data = new List<TextSimilarityVm>();
                    foreach (var text in textArr)
                    {
                        double similarity = TextMatcher.CalculateLevenshteinSimilarity(msg.ToLower(), text.question.ToLower());
                        //data.Add(new TextSimilarityVm { text = text.question, percentage = similarity, KnowledgeId = text.Id });
                        //if (similarity == 1)
                        //{
                        //    break;
                        //}

                        if (text.question.ToLower().IndexOf(msg.ToLower()) >= 0)
                        {
                            similarity = 1;
                        }


                        if (similarity >= 0.50 && similarity <= 1) // Check if similarity is within range
                        {
                            data.Add(new TextSimilarityVm
                            {
                                text = text.question,
                                percentage = similarity,
                                KnowledgeId = text.Id
                            });

                            if (similarity == 1) // Stop if exact match
                            {
                                break;
                            }
                        }
                    }
                    var maxPercentageKnowledgeId = data.OrderByDescending(x => x.percentage).FirstOrDefault()?.KnowledgeId;
                    if (maxPercentageKnowledgeId != null)
                    {
                        var knowledge = await _context.BotKnowledges.AsNoTracking().Where(x => x.Id == maxPercentageKnowledgeId && x.branchInfoId == user.branchId).FirstOrDefaultAsync();
                        if (knowledge.keyWordQuesAnsId != null)
                        {
                            var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.Id == knowledge.keyWordQuesAnsId && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                            //Log Message Start
                            var msglog = new MessageLog
                            {
                                Id = 0,
                                botKey = botKey,
                                connectionId = connectionId,
                                message = questionsByKeyword.answer,
                                menuId = null,
                                Type = msg == "menu" ? "Menu" : "text",
                                rawMessage = questionsByKeyword.answer,
                                entryDate = DateTime.Now,
                                KeyWordQuesAnsId = null,
                                nextNodeId = questionsByKeyword.nextNodeId,
                                branchInfoId = user.branchId
                            };
                            _context.MessageLogs.Add(msglog);
                            await _context.SaveChangesAsync();
                            //Log Message End


                            result = await this.SendNextMessageByNodeId(botKey, connectionId, knowledge.keyWordQuesAnsId);
                        }

                        else
                        {
                            result.Add("{\"msg\" : \"" + knowledge.textReply?.Trim() + "\"}");
                        }
                    }
                    if (maxPercentageKnowledgeId == null && wrapperDetails.wrapperDetailsId == 1)
                    {
                        var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.question.ToLower() == "Department not match" && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                        //Log Message Start
                        var msglog = new MessageLog
                        {
                            Id = 0,
                            botKey = botKey,
                            connectionId = connectionId,
                            message = questionsByKeyword.answer,
                            menuId = null,
                            Type = msg == "menu" ? "Menu" : "text",
                            rawMessage = questionsByKeyword.answer,
                            entryDate = DateTime.Now,
                            KeyWordQuesAnsId = null,
                            nextNodeId = questionsByKeyword.nextNodeId,
                            branchInfoId = user.branchId
                        };
                        _context.MessageLogs.Add(msglog);
                        await _context.SaveChangesAsync();
                        //Log Message End


                        result = await this.SendNextMessageByNodeId(botKey, connectionId, questionsByKeyword.Id);
                    }

                    if (maxPercentageKnowledgeId == null && wrapperDetails.wrapperDetailsId == 2)
                    {
                        var questionsByKeyword = await _context.keyWordQuesAns.Where(x => x.question.ToLower() == "Doctor Name not match" && x.branchInfoId == user.branchId).AsNoTracking().FirstOrDefaultAsync();

                        //Log Message Start
                        var msglog = new MessageLog
                        {
                            Id = 0,
                            botKey = botKey,
                            connectionId = connectionId,
                            message = questionsByKeyword.answer,
                            menuId = null,
                            Type = msg == "menu" ? "Menu" : "text",
                            rawMessage = questionsByKeyword.answer,
                            entryDate = DateTime.Now,
                            KeyWordQuesAnsId = null,
                            nextNodeId = questionsByKeyword.nextNodeId,
                            branchInfoId = user.branchId
                        };
                        _context.MessageLogs.Add(msglog);
                        await _context.SaveChangesAsync();
                        //Log Message End


                        result = await this.SendNextMessageByNodeId(botKey, connectionId, questionsByKeyword.Id);
                    }
                }


            }

            //Console.WriteLine($"Text Similarity: {similarity * 100}%");

            //var maxPercentageText = data.OrderByDescending(x => x.percentage).FirstOrDefault()?.text;
            //var maxPercentage = data.OrderByDescending(x => x.percentage).FirstOrDefault()?.percentage;

            return result;
        }
        public async Task<string> SaveUserByOTP(string otp, string uhid)
        {
            var otpCode = await _context.OTPCodes.Where(x => x.otpCode == otp).AsNoTracking().LastOrDefaultAsync();
            if (otpCode != null)
            {
                var inputs = await _context.ServiceFlows.Where(x => x.connectionId == otpCode.connectionId && x.answerText != null && x.branchInfoId == otpCode.branchInfoId).AsNoTracking().OrderByDescending(x => x.DateTime).ToListAsync();
                var Mobile = inputs.Where(x => x.questionText == "Phone").Select(x => x.answerText).FirstOrDefault();

                var doctorService = await _context.ServiceFlows.Where(x => x.connectionId == otpCode.connectionId && x.branchInfoId == otpCode.branchInfoId && (x.questionText == "Enter Dr. Name" || x.questionText == "Doctor search by department")).OrderByDescending(x => x.DateTime).AsNoTracking().FirstOrDefaultAsync();
                //var doctorService = await _context.ServiceFlows.Where(x => x.connectionId == conId && (x.questionText == "Enter Dr. Name" || x.questionText == "Are you registered with us?" || x.questionText == "Doctor search by department" || x.questionText == "May I know your gender?")).OrderByDescending(x => x.DateTime).AsNoTracking().FirstOrDefaultAsync();

                //var doctorStatus = await _context.AppoinmentInfos.Where(x => x.userInfo.Mobile == Mobile).AsNoTracking().CountAsync();

                var doctorStatus = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfo.name.Trim() == doctorService.answerText.Trim() && x.status == 1 && x.branchInfoId == otpCode.branchInfoId).AsNoTracking().CountAsync();
                var normalizedMobile = Mobile.Replace("+88", "").Replace("88", "");
                var count = 0;

                if (uhid != null && uhid != "null")
                {
                    count = await _context.UserInfos
                    .Where(x => x.Mobile.Replace("+88", "").Replace("88", "") == normalizedMobile
                                && x.branchInfoId == otpCode.branchInfoId && x.UHID == uhid)
                    .AsNoTracking()
                    .CountAsync();

                }
                else
                {
                    count = await _context.UserInfos
                    .Where(x => x.Mobile.Replace("+88", "").Replace("88", "") == normalizedMobile
                                && x.branchInfoId == otpCode.branchInfoId)
                    .AsNoTracking()
                    .CountAsync();
                }

                if (doctorStatus == 0)
                {
                    return "schedule not found";
                }
                else if (count == 0)
                {
                    if (uhid != null && uhid != "null")
                    {
                        var user = new Models.BotModels.UserInfo
                        {
                            Id = Guid.NewGuid().ToString(),
                            FullName = inputs.Where(x => x.questionText == "FullName").Select(x => x.answerText).FirstOrDefault(),
                            Mobile = inputs.Where(x => x.questionText == "Phone").Select(x => x.answerText).FirstOrDefault(),
                            Email = inputs.Where(x => x.questionText == "Email").Select(x => x.answerText).FirstOrDefault(),
                            dateOfBirth = inputs.Where(x => x.questionText == "DateOfBirth").Select(x => x.answerText).FirstOrDefault(),
                            gender = inputs.Where(x => x.questionText == "Gender").Select(x => x.answerText).FirstOrDefault(),
                            otpMsg = otp,
                            UHID = inputs.Where(x => x.questionText == "Phone").Select(x => x.answerText).FirstOrDefault(),
                            branchInfoId = otpCode.branchInfoId
                        };


                        _context.UserInfos.Add(user);
                        await _context.SaveChangesAsync();
                        return "user saved";
                    }
                    else
                    {
                        var user = new Models.BotModels.UserInfo
                        {
                            Id = Guid.NewGuid().ToString(),
                            FullName = inputs.Where(x => x.questionText == "FullName").Select(x => x.answerText).FirstOrDefault(),
                            Mobile = inputs.Where(x => x.questionText == "Phone").Select(x => x.answerText).FirstOrDefault(),
                            Email = inputs.Where(x => x.questionText == "Email").Select(x => x.answerText).FirstOrDefault(),
                            dateOfBirth = inputs.Where(x => x.questionText == "DateOfBirth").Select(x => x.answerText).FirstOrDefault(),
                            gender = inputs.Where(x => x.questionText == "Gender").Select(x => x.answerText).FirstOrDefault(),
                            otpMsg = otp,
                            //UHID = inputs.Where(x => x.questionText == "Phone").Select(x => x.answerText).FirstOrDefault(),
                            branchInfoId = otpCode.branchInfoId
                        };


                        _context.UserInfos.Add(user);
                        await _context.SaveChangesAsync();
                        return "user saved";
                    }

                }
                else if (count == 1)
                {

                    if (uhid != null && uhid != "null")
                    {
                        var userData = await _context.UserInfos
                                           .Where(x => x.branchInfoId == otpCode.branchInfoId && x.UHID == uhid).AsNoTracking().FirstOrDefaultAsync();

                        userData.otpMsg = otp;
                        _context.UserInfos.Update(userData);
                        await _context.SaveChangesAsync();



                        var data = new ServiceFlow
                        {
                            Id = Guid.NewGuid().ToString(),
                            InfoType = "start",
                            ServiceCode = "Pre-Defined Question",
                            DateTime = DateTime.Now,
                            StepNo = 1,
                            Attempt = 0,
                            botKey = otpCode.botKey,
                            connectionId = otpCode.connectionId,
                            status = 1,
                            answerText = uhid,
                            questionText = "Uhid",
                            MenuId = 0,
                            keyWordQuesAnsId = null,
                            branchInfoId = otpCode.branchInfoId
                        };


                        _context.ServiceFlows.Add(data);
                        await _context.SaveChangesAsync();

                        return "user Update";
                    }
                    else
                    {
                        var userData = await _context.UserInfos
                                           .Where(x => x.branchInfoId == otpCode.branchInfoId && x.Mobile.Replace("+88", "").Replace("88", "") == normalizedMobile).AsNoTracking().FirstOrDefaultAsync();

                        userData.otpMsg = otp;
                        _context.UserInfos.Update(userData);
                        await _context.SaveChangesAsync();

                        return "user Update";
                    }


                }
                else
                {
                    return "user exist";
                }
            }
            else
            {
                return "otp not matched";
            }
        }
        public async Task<IEnumerable<Models.BotModels.UserInfo>> GetUserInfo(string mobile)
        {
            var data = await _context.UserInfos.Where(x => x.Mobile == mobile).AsNoTracking().ToListAsync();
            return data;
        }
        public async Task<IEnumerable<KeyWordQuesAns>> GetAllBotQuestionsByBotKey(string botKey)
        {
            var data = await _context.keyWordQuesAns.Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();
            return data;
        }
        public async Task<IEnumerable<KeyWordQuesAns>> GetAllKeywordQuesAns(string botKey)
        {
            var data = await _context.keyWordQuesAns.AsNoTracking().Where(x => x.status == 1 && x.botKey == botKey).Include(x => x.doctor).Include(x => x.department).Include(x => x.nextNode).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<DepartmentInfo>> GetAllDepartmentInfo(string botKey)
        {
            var data = await _context.DepartmentInfos.AsNoTracking().Where(x => x.botKey == botKey).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<DoctorInfo>> GetALlDoctorInfo(string botKey)
        {
            var data = await _context.DoctorInfos.AsNoTracking().Where(x => x.botKey == botKey).ToListAsync();
            return data;
        }
        public async Task<int> SaveOrUpdateKeywordQues(BotKnowledgeViewModel model)
        {
            var ques = await _context.keyWordQuesAns.Where(x => x.Id == model.id).AsNoTracking().FirstOrDefaultAsync();
            ques.question = model.question;
            ques.answer = model.answer;
            ques.priority = model.priority;
            ques.type = model.Type;
            ques.questionOrder = (int)model.Order;
            ques.status = model.Status;
            ques.isQuestion = model.IsQuestion;
            ques.nextNodeId = model.NextNode;
            ques.responseApi = model.ResponseApi;
            ques.smsOtp = model.SMSOTP;
            ques.refName = model.RefName;
            ques.departmentId = model.Department;
            ques.doctorId = model.Doctor;
            _context.keyWordQuesAns.Update(ques);
            var result = await _context.SaveChangesAsync();
            return result;
        }
        public async Task<int> AddMessageLogByQuestionId(int id)
        {
            var ques = await _context.keyWordQuesAns.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            var data = new MessageLog
            {
                Id = 0,
                botKey = "",
                connectionId = "",
                entryDate = DateTime.Now,
                KeyWordQuesAnsId = ques.Id,
                nextNodeId = ques.nextNodeId,
                rawMessage = ques.answer
            };
            _context.MessageLogs.Add(data);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetRawMessageByNodeId(int? nodeId, string connectionId, string botKey)
        {
            var result = new List<string>();
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            //var phone = await _context.ServiceFlows.Where(x => x.questionText == "Phone" && x.connectionId == connectionId).AsNoTracking().LastOrDefaultAsync();
            var keyWordQuesAns = await _context.keyWordQuesAns.Where(x => x.Id == nodeId).AsNoTracking().FirstOrDefaultAsync();
            var data = await _context.keyWordQuesAns.Where(x => x.questionKey == keyWordQuesAns.questionKey).AsNoTracking().ToListAsync();
            if (keyWordQuesAns.smsOtp == 1)
            {
                var otpCode = this.GenerateRandomOTP(4);
                var otpData = new OTPCode
                {
                    Id = 0,
                    botKey = botKey,
                    branchInfoId = user.branchId,
                    connectionId = connectionId,
                    entryDate = DateTime.Now,
                    expireTime = DateTime.Now.AddMinutes(5),
                    otpCode = otpCode.ToString(),
                    parameterName = "OTPCode",
                    refMenuId = null
                };
                _context.OTPCodes.Add(otpData);
                await _context.SaveChangesAsync();
                var phoneNumber = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && (x.questionText == "Phone" || x.questionText == "Please enter registered mobile number") && x.answerText != null).AsNoTracking().Select(x => x.answerText).LastOrDefaultAsync();
                var otpmsg = "<p>Please enter otp received on your number</p>";
                var otpHtml = "";

                if (_configuration["Project:isLive"] == "YES")
                {
                    smsAPI.Single_Sms(phoneNumber, "Your OTP Code is " + otpCode.ToString() + ". Keep it secure. It is valid for 5 minutes only.");
                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick=\"GetNextMessage(" + keyWordQuesAns.nodes + ",'" + connectionId + "','" + botKey + "')\" value='Submit' />";
                }
                else
                {
                    //await this.SendSMSAsync(phoneNumber, "Your OTP Code is " + otpCode.ToString() + ". Keep it secure. It is valid for 5 minutes only.");
                    otpHtml = "<input type='text' name='otpCode' id='otpCode' class='otpCode' value='" + otpCode.ToString() + "' class='form-control' placeholder='XXXX' /><input type='button' class='btn btn-success btn-sm' onclick=\"GetNextMessage(" + keyWordQuesAns.nodes + ",'" + connectionId + "','" + botKey + "')\" value='Submit' />";
                }

                result.Add(otpmsg);
                result.Add(otpHtml);
                var msgLog = new MessageLog
                {
                    Id = 0,
                    botKey = botKey,
                    connectionId = connectionId,
                    Type = "text",
                    nextNodeId = Convert.ToInt32(keyWordQuesAns.nodes)
                };
                _context.MessageLogs.Add(msgLog);
                await _context.SaveChangesAsync();
                return result;
            }
            foreach (var item in data)
            {
                if (item.type == 1)
                {
                    result.Add(item.answer);
                }
                else if (item.type == 3)
                {

                    var part1 = "<div id='message' class='message' style='background:white !important; border:none;box-shadow:none;'><div style='display: inline-block;background: white;margin-left:-15px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'>";

                    var part2 = "";
                    foreach (var btn in item.answer.Split(","))
                    {
                        //part2 += "<button class='btn-group-message'" +
                        //    " onclick='handleButtonClick(" + btn.Trim() + ")'>" + btn.Trim() + "</button>";

                        var btnText = btn;
                        if (btn?.Trim() == "Male")
                        {
                            btnText = "<img src=\'https://chatbot.hlthclub.in/static/male.svg\' width=\'32px\' alt=\'sunil\' style=\'float: left;\'>" + "<span style='padding: 15px 10px;'>Male </span>";
                        }
                        if (btn?.Trim() == "Female")
                        {
                            btnText = "<img src=\'https://chatbot.hlthclub.in/static/female.svg\' width=\'32px\' alt=\'sunil\' style=\'float: left;\'>" + "<span style='padding: 15px 10px;'>Female</span>";
                        }

                        part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                            " onclick='handleButtonClick(this, " + item.Id + ")'>" + btnText?.Trim() + "</button>";
                    }

                    var part3 = "</div></div>";

                    result.Add(part1 + part2 + part3);


                }
                else if (item.type == 4)
                {
                    if (item.refName == "RESCHEDULE")
                    {
                        var phone = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.questionText == "Phone").AsNoTracking().LastOrDefaultAsync();
                        if (phone != null)
                        {
                            //var schedules = await _context.AppoinmentInfos.Include(x => x.userInfo).Include(x => x.doctorInfo).Where(x => x.userInfo.Mobile == phone.answerText && x.status == 0).AsNoTracking().ToListAsync();
                            // var schedules = await _context.AppoinmentInfos.Include(x => x.userInfo).Where(x => x.userInfo.Mobile == phone.answerText && x.status == 0).AsNoTracking().ToListAsync();
                            var schedules = await _context.AppoinmentInfos.Include(x => x.userInfo).Where(x => x.userInfo.Mobile == phone.answerText && x.status == 0 && x.date.Value.Date >= phone.DateTime.Date).AsNoTracking().ToListAsync();

                            if (schedules.Count() > 0)
                            {

                                var part1 = "<div><div class=\"scheduleOptionscroll\" id=\"AppointmentCard\">";
                                var part2 = "";
                                foreach (var schedule in schedules)
                                {
                                    var uhid = await _context.UserInfos.Where(x => x.Id == schedule.userInfoId).Select(x => x.UHID).AsNoTracking().FirstOrDefaultAsync();
                                    //part2 += "<div class=\"bluedualcard drdtal drppt \" style=\"margin-bottom: -5px; width: 100%; flex: 0 0 100%; max-width: 320px; min-height: 150px;\"><div class=\"drdtal-top\" style=\"margin-top: 10px;\"><div class=\" drdtal-topleft\"><img src=\"https://chatbot.hlthclub.in/static/doctor-male.png\" alt=\"\" width=\"60px\"></div><div class=\"drdtal-topright\"><p>  Appointment with</p><p class=\"drname\">" + schedule.doctorInfo?.name + "</p><span style=\"font-size: 11px; width: 100%; float: left; margin-bottom: 2px;\">" + schedule.doctorInfo?.designationName + "</span><small style=\"color: rgb(255, 255, 255); margin-bottom: 2px;\"></small><p style=\"font-size: 13px; margin-top: 5px; float: left; margin-right: 3px;\"><i class=\"glyphicon glyphicon-time\" style=\"font-size: 13px; margin-top: 2px; float: left; margin-right: 3px;\"></i>" + schedule.time + ", " + schedule.date?.ToString("dd-MM-yyyy") + " </p></div></div><a class=\"btn btn-btm btnbooking\" style=\" width: 300px;background: rgb(84, 34, 103) !important;margin-bottom:-5px;\"><div class=\"yellowdate\" style=\"width: 50%; float: left; padding: 9px 0px; -webkit-box-flex: 1; flex: 1 1 auto; text-align: center; display: block; border-right: 1px solid rgb(84, 34, 103); font-size: 16px; background: rgb(233, 30, 99);\" onClick=\"CancelSchedule(" + schedule.Id + "," + schedule.doctorInfoId + "," + item.Id + ",'" + item.nodes + "','" + schedule.doctorInfo?.name + "','" + schedule.time + "','" + schedule.date?.ToString("yyyy-MM-dd") + "','" + phone.answerText + "','" + connectionId + "','" + botKey + "')\">  Cancel</div><div class=\"yellow_time\" style=\"padding: 9px 0px;background: rgb(84, 34, 103) !important; width: 50%; float: left; -webkit-box-flex: 1; flex: 1 1 auto; font-size: 16px;\" onClick=\"RescheduleSchedule(" + schedule.Id + "," + schedule.doctorInfoId + "," + item.Id + ",'" + item.nodes + "','" + schedule.doctorInfo?.name + "','" + schedule.time + "','" + schedule.date?.ToString("yyyy-MM-dd") + "','" + phone.answerText + "','" + connectionId + "','" + botKey + "')\">   Reschedule</div></a></div>";
                                    part2 += "<div class=\"bluedualcard drdtal drppt \" style=\"margin-bottom: -5px; width: 100%; flex: 0 0 100%; max-width: 300px; min-height: 175px;  margin-bottom: -10px;  \"><div class=\"drdtal-top\" style=\"margin-top: 10px;\"><div class=\" drdtal-topleft\"><img src=\"https://chatbot.hlthclub.in/static/doctor-male.png\" alt=\"\" width=\"60px\"></div><div class=\"drdtal-topright\"><p>  Appointment with</p><p class=\"drname\" style=\"font-size: 11px; \">" + schedule.doctorName + "</p><span style=\"font-size: 11px; width: 100%; float: left; margin-bottom: 2px;\">" + schedule.designationName + "</span><p class=\"uhidData\" style=\"font-size: 11px; \">" + "UHID: " + uhid + "</p><small style=\"color: rgb(255, 255, 255); margin-bottom: 2px;\"></small><p style=\"font-size: 13px; margin-top: 5px; float: left; margin-right: 3px;\"><i class=\"glyphicon glyphicon-time\" style=\"font-size: 13px; margin-top: 2px; float: left; margin-right: 3px;\"></i>" + schedule.time + ", " + schedule.date?.ToString("dd-MM-yyyy") + " </p></div></div><a class=\"btn btn-btm btnbooking\" style=\" width: 300px;background: rgb(84, 34, 103) !important;margin-bottom:-10px;\"><div class=\"yellowdate\" style=\"width: 50%; float: left; padding: 9px 0px; -webkit-box-flex: 1; flex: 1 1 auto; text-align: center; display: block; border-right: 1px solid rgb(84, 34, 103); font-size: 16px; background: rgb(233, 30, 99);\" onClick=\"CancelSchedule(" + schedule.Id + "," + schedule.doctorInfoId + "," + item.Id + ",'" + item.nodes + "','" + schedule.doctorName + "','" + schedule.time + "','" + schedule.date?.ToString("yyyy-MM-dd") + "','" + phone.answerText + "','" + connectionId + "','" + botKey + "')\">  Cancel</div><div class=\"yellow_time\" style=\"padding: 9px 0px;background: rgb(84, 34, 103) !important; width: 50%; float: left; -webkit-box-flex: 1; flex: 1 1 auto; font-size: 16px;\" onClick=\"RescheduleSchedule(" + schedule.Id + "," + schedule.doctorInfoId + "," + item.Id + ",'" + item.nodes + "','" + schedule.doctorName + "','" + schedule.time + "','" + schedule.date?.ToString("yyyy-MM-dd") + "','" + phone.answerText + "','" + connectionId + "','" + botKey + "','" + uhid + "')\">   Reschedule</div></a></div>";
                                }
                                var part3 = "</div><button class='reschedule-box-left' onclick='rescheduleboxLeft()'><</button><button class='reschedule-box-right' onclick='rescheduleboxRight()'>></button></div>";
                                result.Add(part1 + part2 + part3);
                            }
                            else
                            {
                                result.Add("<p>No Schedule Found.</p>");
                            }
                        }
                    }
                    else
                    {
                        result.Add(item.answer);
                    }
                }
                else if (item.type == 7)
                {
                    var part1 = "<form id='frmBasic' class='frmBasic'>";
                    var part2 = "";
                    var inpGroups = await _context.InputGroupDetails.Include(x => x.master).Where(x => x.master.menuId == item.Id).AsNoTracking().ToListAsync();
                    foreach (var inp in inpGroups)
                    {
                        part2 += "<div class='input-group' style='width: 100%;'>" +
                        "<div class='input-group-prepend'>" +
                                        "<span style='display:none;' class='input-group-text'>" + inp.inputName + "</span>" +
                                        "</div>" +
                                    "<input type='text' style='height: 40px;margin: 5px; border: 1px solid lightgrey; border-radius:4px !important;' class='form-control inp-control' name='valueText' placeholder='" + inp.placeHolder + "' aria-label='Name' aria-describedby='basic-addon1' required><input type='hidden' name='parameterName' value='" + inp.parameterName + "' />" + "<input type='hidden' name='botKey' value='" + botKey + "' />" + "<input type='hidden' name='connectionId' value='" + connectionId + "' />" + "<input type='hidden' name='nextNodeId' value='" + item.Id + "' />" +
                                "</div>";

                    }
                    var part3 = "<input type='button' style='height: 40px; margin: 5px; border-radius:4px' onclick='SubmitInputData(" + item.Id + "," + item.nextNodeId + ")' value='Submit' class='btn btn-success btn-sm mt-2' /></form>";

                    if (inpGroups.Count() > 0)
                    {
                        result.Add(part1 + part2 + part3);
                    }


                }
                else if (item.type == 110)
                {
                    if (item.refName == "NEXT7DAYS")
                    {

                        var doctorServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && (x.questionText == "Enter Dr. Name" || x.questionText == "Doctor search by department")).AsNoTracking().LastOrDefaultAsync();
                        var doctorWeeks = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfo.name == doctorServiceFlow.answerText).Select(x => x.weeks.name).AsNoTracking().Distinct().ToListAsync();
                        DateTime currentDate = DateTime.Now;
                        List<string> next7Days = new List<string>();
                        for (int i = 1; i < 30; i++)
                        {
                            DateTime nextDate = currentDate.AddDays(i);

                            if (doctorWeeks.Contains(nextDate.DayOfWeek.ToString()))
                            {
                                next7Days.Add(nextDate.ToString("yyyy-MM-dd"));
                            }

                            if (next7Days.Count() >= 7)
                            {
                                break;
                            }
                        }

                        var part1 = "<div class='NEXT7DAYS' style='display: inline-block; margin: 1px;'>";

                        var part2 = "";
                        foreach (var btn in next7Days)
                        {
                            part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                " onclick='handleAppointmentDateClick(this, " + item.Id + ")'>" + btn?.Trim() + "</button>";
                        }

                        var part3 = "</div>";

                        result.Add("Please select the date for appointment");
                        result.Add(part1 + part2 + part3);

                        //DateTime currentDate = DateTime.Now;
                        //List<string> next7Days = new List<string>();
                        //for (int i = 0; i < 7; i++)
                        //{
                        //    DateTime nextDate = currentDate.AddDays(i);
                        //    next7Days.Add(nextDate.ToString("yyyy-MM-dd"));
                        //}

                        //var part1 = "<div class='NEXT7DAYS' style='display: inline-block; margin: 1px;'>";

                        //var part2 = "";
                        //foreach (var btn in next7Days)
                        //{
                        //    part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                        //        " onclick='handleAppointmentDateClick(this, " + item.Id + ")'>" + btn?.Trim() + "</button>";
                        //}

                        //var part3 = "</div>";

                        //result.Add(part1 + part2 + part3);
                    }


                    if (item.refName == "TIMESLOTS")
                    {
                        var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId).AsNoTracking().Where(x => x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr")).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                        var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText && x.botKey == botKey).FirstOrDefaultAsync();
                        //var appointment = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id).Select(x => x.time).AsNoTracking().FirstOrDefaultAsync();

                        var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                        var bookedTime = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id && Convert.ToDateTime(x.date).Date == Convert.ToDateTime(appointDate).Date).Select(x => x.time).AsNoTracking().ToListAsync();

                        var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id && !bookedTime.Contains(x.timePeriod.timeSlot)).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.sortOrder).Select(x => x.timePeriod).AsNoTracking().ToListAsync();


                        var part1 = "<div class='TIMESLOTS' style='display: inline-block; margin: 1px;'>";

                        var part2 = "";

                        foreach (var btn in slots)
                        {
                            part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                " onclick='handleTimeSlotClick(this, " + item.Id + "," + btn.Id + ")'>" + btn.timeSlot?.Trim() + "</button>";
                        }

                        var part3 = "</div>";

                        result.Add("Please select the time slot for appointment.");
                        result.Add(part1 + part2 + part3);
                    }

                    if (item.refName == "RESCHEDULE")
                    {
                        var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId).AsNoTracking().Where(x => x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr")).OrderBy(x => x.DateTime).LastOrDefaultAsync();

                        var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText && x.botKey == botKey).FirstOrDefaultAsync();
                        //var appointment = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id).Select(x => x.time).AsNoTracking().FirstOrDefaultAsync();

                        var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                        var bookedTime = await _context.AppoinmentInfos.Where(x => x.doctorInfoId == docInfo.Id && Convert.ToDateTime(x.date).Date == Convert.ToDateTime(appointDate).Date).Select(x => x.time).AsNoTracking().ToListAsync();

                        var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id && !bookedTime.Contains(x.timePeriod.timeSlot)).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.sortOrder).Select(x => x.timePeriod).AsNoTracking().ToListAsync();


                        var part1 = "<div class='TIMESLOTS' style='display: inline-block; margin: 1px;'>";

                        var part2 = "";

                        foreach (var btn in slots)
                        {
                            part2 += "<button class='btn-group-message'" + " value='" + item.Id + "'" +
                                " onclick='handleTimeSlotClick(this, " + item.Id + "," + btn.Id + ")'>" + btn.timeSlot?.Trim() + "</button>";
                        }

                        var part3 = "</div>";

                        result.Add("Please select the time slot for appointment.");
                        result.Add(part1 + part2 + part3);
                    }

                    if (item.refName == "APPOINTMENTCONFIRMATION")
                    {
                        //var drServiceFlow = await _context.ServiceFlows.Where(x => x.connectionId == connectionId).AsNoTracking().Where(x => x.questionText == "Enter Dr. Name").OrderBy(x => x.DateTime).LastOrDefaultAsync();

                        //var docInfo = await _context.DoctorInfos.Where(x => x.name == drServiceFlow.answerText.Trim() && x.botKey == botKey).FirstOrDefaultAsync();

                        //var slots = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == docInfo.Id).Include(x => x.timePeriod).OrderBy(x => x.timePeriod.sortOrder).Select(x => x.timePeriod).AsNoTracking().ToListAsync();
                        var uhid = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.branchInfoId == user.branchId && x.answerText != null && x.questionText == "Uhid").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();

                        var phoneNumber = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.questionText == "Phone").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                        var doctorName = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && (x.questionText == "Enter Dr. Name" || x.answerText.ToLower().Contains("dr"))).AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                        var appointDate = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.questionText == "AppointDate").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();
                        var timeSlot = await _context.ServiceFlows.Where(x => x.connectionId == connectionId && x.answerText != null && x.questionText == "TimeSlot").AsNoTracking().OrderByDescending(x => x.DateTime).Select(x => x.answerText).FirstOrDefaultAsync();


                        var docInfo = await _context.DoctorInfos.Include(x => x.departmentName).Where(x => x.name == doctorName.Trim() && x.botKey == botKey).FirstOrDefaultAsync();
                        //var userInfo = await _context.UserInfos.Where(x => x.Mobile == phoneNumber.Trim()).FirstOrDefaultAsync();
                        var userInfo = await _context.UserInfos.Where(x => x.UHID == uhid.Trim()).FirstOrDefaultAsync();

                        var appointData = new AppoinmentInfo
                        {
                            Id = 0,
                            doctorInfoId = docInfo.ApiDoctorId,
                            userInfoId = userInfo.Id,
                            date = Convert.ToDateTime(appointDate),
                            botKey = botKey,
                            time = timeSlot,
                            status = 0,
                            isVerified = 0,
                            appointStatus = "Application",
                            entryDate = DateTime.Now,
                            entryby = "mehedi"
                        };

                        _context.AppoinmentInfos.Add(appointData);
                        await _context.SaveChangesAsync();


                        var chatData = await _context.ServiceFlows.Where(x => x.connectionId == connectionId).AsNoTracking().ToListAsync();

                        string htmlContent = "<table>" +
                                                "<tbody>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Full Name</td><td>: " + chatData.Where(x => x.questionText == "FullName").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Date of Birth</td><td>: " + chatData.Where(x => x.questionText == "DateOfBirth").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Sex</td><td>: " + chatData.Where(x => x.questionText == "Gender").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Mobile</td><td>: " + chatData.Where(x => x.questionText == "Phone").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Email</td><td style='font-size:10px;'>: " + chatData.Where(x => x.questionText == "Email").Select(x => x.answerText).LastOrDefault() + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Doctor</td><td>: " + doctorName + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: center' colspan='2'>Preferred Date and Time for Appointment</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Date</td><td>: " + Convert.ToDateTime(appointDate).ToString("dd-MMM-yyyy") + "</td></tr>" +
                                                        "<tr><td style='font-weight: bold; text-align: right;'>Time</td><td>: " + timeSlot + "</td></tr>" +
                                                "</tbody>" +
                                            "</table>";


                        var reply = "Thank you for your request . Our customer care officer will contact you for appointment confirmation";

                        //var reply = "Thanks for booking an appointment with us. Your booking details: Booking No. 2863523, " + doctorName + ","+ docInfo.Department?.departmentName + "; Date: " + Convert.ToDateTime(appointDate).ToString("dd-MMM-yyyy") + "," + Convert.ToDateTime(appointDate).DayOfWeek.ToString() + "," + timeSlot + ".";

                        if (_configuration["Project:isLive"] == "YES")
                        {
                            smsAPI.Single_Sms(phoneNumber, reply);
                            await SendHTMLEmail(chatData.Where(x => x.questionText == "Email").Select(x => x.answerText).LastOrDefault(), "Chatbot Appointment Request", htmlContent);
                        }
                        else
                        {
                            await this.SendSMSAsync(phoneNumber, reply);
                            await SendHTMLEmail(chatData.Where(x => x.questionText == "Email").Select(x => x.answerText).LastOrDefault(), "Chatbot Appointment Request", htmlContent);
                        }

                        int btnClickId = 495;
                        if (user.branchId == 2)
                        {
                            btnClickId = 1980;
                        }

                        result.Add(reply?.Trim());
                        result.Add("Please let us know, what more i can help you with.");
                        result.Add("<div style='display: inline-block;background: white;margin-left:-15px; width: 345px;display: flex;justify-content: center; flex-wrap: wrap;'><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment</button><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment with Doctor</button><button class='btn-group-message' value='" + btnClickId + "' onclick='handleButtonClick(this, " + btnClickId + ")'>Book Appointment with department</button></div>");
                    }
                }

                else
                {

                }
            }

            return result;
        }

        public async Task<List<string>> CancelAppointmentById(int scheduleId, string connectionId, string botKey)
        {
            var data = await _context.AppoinmentInfos.Where(x => x.Id == scheduleId).AsNoTracking().LastOrDefaultAsync();

            data.status = 5;
            data.isDelete = 1;

            _context.AppoinmentInfos.Update(data);
            await _context.SaveChangesAsync();

            var result = new List<string>();

            result.Add("Appointment Canceled");

            return result;
        }

        public async Task<int> SaveDoctorNameInServiceFlow(int nextNodeId, string connectionId, string botKey, string doctorName, int scheduleId)
        {
            var appointment = await _context.AppoinmentInfos.Where(x => x.Id == scheduleId).AsNoTracking().FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(doctorName))
            {
                var doctors = await _context.DoctorInfos.FindAsync(appointment.doctorInfoId);
                doctorName = doctors.name;
            }
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();

            var data = new ServiceFlow
            {
                Id = Guid.NewGuid().ToString(),
                InfoType = "start",
                ServiceCode = "Pre-Defined Question",
                DateTime = DateTime.Now,
                StepNo = 1,
                Attempt = 0,
                botKey = botKey,
                connectionId = connectionId,
                status = 1,
                answerText = doctorName,
                //questionText = "Are you registered with us?",
                questionText = "Enter Dr. Name",
                MenuId = 0,
                keyWordQuesAnsId = null,
                branchInfoId = user.branchId
            };


            _context.ServiceFlows.Add(data);
            return await _context.SaveChangesAsync();
        }

        public async Task<string> GetConnectionIdByOTP(string otp)
        {
            var isValid = await _context.OTPCodes.Where(x => x.otpCode == otp).AsNoTracking().LastOrDefaultAsync();

            var lastmsg = await _context.MessageLogs.Where(x => x.connectionId == isValid.connectionId).AsNoTracking().OrderBy(x => x.Id).LastOrDefaultAsync();

            return isValid.connectionId;
        }


        public async Task<string> GetBotKeyByOTP(string otp)
        {
            var isValid = await _context.OTPCodes.Where(x => x.otpCode == otp).AsNoTracking().LastOrDefaultAsync();

            var lastmsg = await _context.MessageLogs.Where(x => x.connectionId == isValid.connectionId).AsNoTracking().OrderBy(x => x.Id).LastOrDefaultAsync();

            return lastmsg.botKey;
        }

        public async Task<Models.BotModels.ConnectionInfo> GetConnectionInfoByUserId(string userid)
        {
            var data = await _context.ConnectionInfos.Where(x => x.userId == userid).AsNoTracking().LastOrDefaultAsync();
            return data;
        }



        public async Task<string> GetPhoneByConnectionId(string connectionId)
        {
            var data = await _context.ServiceFlows.Where(x => x.answerText != null && x.questionText == "Phone" && x.connectionId == connectionId).AsNoTracking().OrderByDescending(x => x.DateTime).FirstOrDefaultAsync();
            return data.answerText;
        }


        public async Task<Models.BotModels.UserInfo> GetUserInfoByuhId(string uhid)
        {
            var data = await _context.UserInfos.Where(x => x.UHID == uhid).AsNoTracking().LastOrDefaultAsync();
            return data;
        }

        #region Department Search

        #endregion

        #region DoctorSearch

        #endregion

        #region Booking

        #endregion

        #region Know More

        #endregion


        public async Task<ChatbotInfo> GetChatBotInfoByBotKey(string botkey)
        {
            var data = await _context.ChatbotInfos.Include(x => x.ApplicationUser).Where(x => x.botKey == botkey).AsNoTracking().LastOrDefaultAsync();

            return data;
        }





        #region SaveApiData

        public async Task<int> SaveUhidUserFromApi(List<Models.BotModels.UserInfo> models)
        {
            try
            {
                foreach (var model in models)
                {
                    var existingUser = await _context.UserInfos
                                                    .FirstOrDefaultAsync(u => u.UHID == model.UHID);

                    if (existingUser == null)
                    {
                        _context.UserInfos.Add(model);
                    }
                    else
                    {

                        existingUser.Id = existingUser.Id;
                        existingUser.Email = model.Email;
                        existingUser.FullName = model.FullName;
                        existingUser.Mobile = model.Mobile;
                        existingUser.UHID = model.UHID;
                        existingUser.branchInfoId = model.branchInfoId;
                        existingUser.dateOfBirth = model.dateOfBirth;
                        existingUser.gender = model.gender;

                        _context.UserInfos.Update(existingUser);


                    }
                }

                var changes = await _context.SaveChangesAsync();
                return changes;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<int> SaveApiDepartment(List<ApiDepartment> models)
        {
            var depts = await _context.ApiDepartments.Where(x => x.facility == "EHD").AsNoTracking().ToListAsync();
            _context.ApiDepartments.RemoveRange(depts);
            await _context.SaveChangesAsync();

            _context.ApiDepartments.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }
        public async Task<int> SaveApiDepartmentEHC(List<ApiDepartment> models)
        {
            var depts = await _context.ApiDepartments.Where(x => x.facility == "EHC").AsNoTracking().ToListAsync();
            _context.ApiDepartments.RemoveRange(depts);
            await _context.SaveChangesAsync();

            _context.ApiDepartments.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }
        public async Task<int> SaveApiDoctor(List<ApiDoctor> models)
        {
            var docts = await _context.ApiDoctors.AsNoTracking().ToListAsync();
            _context.ApiDoctors.RemoveRange(docts);
            await _context.SaveChangesAsync();

            _context.ApiDoctors.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }

        public async Task<int> SaveAllDoctorSlotsFromApi(List<DoctorSlotData> models)
        {
            var existingDoctors = await _context.ApiDoctors.Where(x => x.facility == "EHD").AsNoTracking().ToListAsync();
            _context.ApiDoctors.RemoveRange(existingDoctors);

            var existingSlots = await _context.ApiDoctorSlots.Where(x => x.facility == "EHD").AsNoTracking().ToListAsync();
            _context.ApiDoctorSlots.RemoveRange(existingSlots);

            await _context.SaveChangesAsync();
            foreach (var item in models)
            {
                var data = new ApiDoctor
                {
                    Id = 0,
                    doctorId = item.doctor.id,
                    name = item.doctor.name,
                    facility = item.doctor.facility,
                    department = item.doctor.department,
                    gender = item.doctor.gender,
                    specialization = item.doctor.specialization,
                    uniqueKey = item.uniqueKey,
                    date = DateTime.Now,
                };


                foreach (var item2 in item.slots)
                {
                    var data2 = new Models.ApiModels.ApiDoctorSlot
                    {
                        Id = 0,
                        slotId = item2.id,
                        appointmentDate = item2.appointmentDate,
                        slotFrom = item2.slotFrom,
                        slotTo = item2.slotTo,
                        doctorID = item2.doctorID,
                        facility = item2.facility,
                        remarks = item2.remarks,
                    };
                    _context.ApiDoctorSlots.AddRange(data2);
                }

                _context.ApiDoctors.AddRange(data);

                //return await _context.SaveChangesAsync();


            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveAllDoctorSlotsFromApiEhc(List<DoctorSlotData> models)
        {
            var existingDoctors = await _context.ApiDoctors.Where(x => x.facility == "EHC").AsNoTracking().ToListAsync();
            _context.ApiDoctors.RemoveRange(existingDoctors);

            var existingSlots = await _context.ApiDoctorSlots.Where(x => x.facility == "EHC").AsNoTracking().ToListAsync();
            _context.ApiDoctorSlots.RemoveRange(existingSlots);

            await _context.SaveChangesAsync();
            foreach (var item in models)
            {
                var data = new ApiDoctor
                {
                    Id = 0,
                    doctorId = item.doctor.id,
                    name = item.doctor.name,
                    facility = item.doctor.facility,
                    department = item.doctor.department,
                    gender = item.doctor.gender,
                    specialization = item.doctor.specialization,
                    uniqueKey = item.uniqueKey,
                    date = DateTime.Now,
                };


                foreach (var item2 in item.slots)
                {
                    var data2 = new Models.ApiModels.ApiDoctorSlot
                    {
                        Id = 0,
                        slotId = item2.id,
                        appointmentDate = item2.appointmentDate,
                        slotFrom = item2.slotFrom,
                        slotTo = item2.slotTo,
                        doctorID = item2.doctorID,
                        facility = item2.facility,
                        remarks = item2.remarks,
                    };
                    _context.ApiDoctorSlots.AddRange(data2);
                }

                _context.ApiDoctors.AddRange(data);

                //return await _context.SaveChangesAsync();


            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveApiSpecialisation(List<ApiSpecialisation> models)
        {
            var spec = await _context.ApiSpecialisation.AsNoTracking().ToListAsync();
            _context.ApiSpecialisation.RemoveRange(spec);
            await _context.SaveChangesAsync();


            _context.ApiSpecialisation.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }

        public async Task<int> SaveApiDoctorSlot(List<Models.ApiModels.ApiDoctorSlot> models)
        {
            var docts = await _context.ApiDoctorSlots.AsNoTracking().ToListAsync();
            _context.ApiDoctorSlots.RemoveRange(docts);
            await _context.SaveChangesAsync();

            _context.ApiDoctorSlots.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }

        //public async Task<ApiMasterDataVM> GetAllMasterData()
        //{
        //    try
        //    {
        //        //var result = await _context.apiMasterDataVMs.FromSql($"GetDoctorDepartmentDetails").ToListAsync();
        //        var result = await _context.apiMasterDataVMs.FromSql($"GetDoctorDepartmentDetails").ToListAsync();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public async Task<bool> GetAllMasterData()
        {
            try
            {
                //var result = await _context.apiMasterDataVMs               .FromSql("EXEC GetAlldataDetails")               .ToListAsync();
                var result = await _dapper.FromSqlAsync<UserFeedbackQueryViewModel>($"GetAlldataDetails");
                return result != null;


            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<int> UpdateTokenMessageStatus()
        {
            var data = await _context.EvercareTokens.AsNoTracking().FirstOrDefaultAsync();

            if (data.sentAlert == 0)
            {
                data.sentAlert = 1;
            }

            _context.EvercareTokens.Update(data);

            return await _context.SaveChangesAsync();
        }


        public async Task<EvercareToken> GetActiveToken()
        {
            var data = await _context.EvercareTokens.AsNoTracking().Where(x => x.isActive == 1).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            return data;
        }


        public async Task<ApiProcessLogInfo> GetLastProcess()
        {
            var data = await _context.ApiProcessLogs.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            return data;
        }


        public async Task<IEnumerable<ApiDoctor>> GetAllApiDoctor()
        {
            var data = await _context.ApiDoctors.ToListAsync();
            return data;
        }



        public async Task<ApiDoctor> GetPrevDoctor(int apiDocId)
        {
            var dept = await _context.ApiDoctors.Where(x => x.doctorId == apiDocId).Select(x => x.department).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.ApiDoctors.Where(x => x.department == dept && x.doctorId < apiDocId).AsNoTracking().OrderBy(x => x.doctorId).LastOrDefaultAsync();
            return data;
        }
        public async Task<ApiDoctor> GetNextDoctor(int apiDocId)
        {
            var dept = await _context.ApiDoctors.Where(x => x.doctorId == apiDocId).Select(x => x.department).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.ApiDoctors.Where(x => x.department == dept && x.doctorId > apiDocId).AsNoTracking().OrderBy(x => x.doctorId).FirstOrDefaultAsync();
            return data;
        }


        #endregion



        #region Get Api Doctor

        public async Task<bool> GetDoctorApiId(int doctorId, string date)
        {
            var docInfo = await _context.DoctorInfos
                .Where(x => x.ApiDoctorId == doctorId)
                .FirstOrDefaultAsync();

            if (docInfo == null)
            {

                return false;
            }

            int weekId = 1;

            #region Slots Api Call
            var branch = docInfo.branchInfoId == 1 ? "EHD" : "EHC";

            var weekDay = Convert.ToDateTime(date).DayOfWeek.ToString();
            weekId = _context.Weeks
                .Where(x => x.name == weekDay && x.branchInfoId == docInfo.branchInfoId)
                .AsNoTracking()
                .Select(x => x.Id)
                .FirstOrDefault();

            var token = await GetActiveToken();
            string bearerToken = token.token;
            string baseDoctorSlots2Url = "https://Applink.evercarebd.com:8018/api/DoctorSlot";

            var doctorSlotUrl2 = $"{baseDoctorSlots2Url}/{branch}/{doctorId}/{date}";
            var doctorSlot2 = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(doctorSlotUrl2, bearerToken);

            if (doctorSlot2 != null && doctorSlot2.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }



        public async Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorVm>> FetchDoctors(string botKey)
        {
            string facility = "";

            var botInfo = await _context.ChatbotInfos.Where(x => x.botKey == botKey).AsNoTracking().FirstOrDefaultAsync();

            if (botInfo.branchInfoId == 1)
            {
                facility = "Ehd";
            }
            if (botInfo.branchInfoId == 2)
            {
                facility = "Ehc";
            }

            //string doctorsurl = "https://Applink.evercarebd.com:8018/api/Employee/" + facility;

            //var token = await GetActiveToken();

            //var doctors = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctorsurl, token.token);



            var doctors = new List<DoctorVm>();
            var doc = await _context.DoctorInfos.Where(x => x.branchInfoId == botInfo.branchInfoId && x.isDelete != 1).Include(x => x.doctorSpecialization).ToListAsync();

            var combinedDoctors = doctors.Concat(doc.Select(d => new DoctorVm
            {
                id = (int)d.ApiDoctorId,
                name = d.name,
                code = d.doctorCode,
                department = d.departmentName,
                gender = d.gender,
                specialization = d.doctorSpecialization.name,
                facility = facility
            })).ToList();



            return combinedDoctors.OrderBy(x => x.id).ToList();
        }



        public async Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorSlotVm>> Fetch7DaysSlot(string botKey, int apiDocId)
        {

            string facility = "";
            var botInfo = await _context.ChatbotInfos.Where(x => x.botKey == botKey).AsNoTracking().FirstOrDefaultAsync();

            if (botInfo.branchInfoId == 1)
            {
                facility = "Ehd";
            }
            if (botInfo.branchInfoId == 2)
            {
                facility = "Ehc";
            }

            string doctorSlotUrl = "https://Applink.evercarebd.com:8018/api/DoctorSlot";

            var token = await GetActiveToken();

            var result = new List<DoctorSlotVm>();

            #region 7 Days
            DateTime today = DateTime.Today;
            List<DateTime> next7Days = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                next7Days.Add(today.AddDays(i));

                var postUrl = $"{doctorSlotUrl}/{facility.ToUpper()}/{apiDocId}/{date.ToString("yyyy-MM-dd")}";
                var doctorSlot2 = await ApiCall.GetApiResponseAsync<List<DoctorSlotVm>>(postUrl, token.token);

                result.AddRange(doctorSlot2);
            }
            #endregion

            return result.ToList();
        }


        public async Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorVm>> FetchDoctors2(string botKey, int apiDocId, string doctorKey)
        {
            string facility = "";

            var botInfo = await _context.ChatbotInfos.Where(x => x.botKey == botKey).AsNoTracking().FirstOrDefaultAsync();


            if (botInfo.branchInfoId == 1)
            {
                facility = "Ehd";
            }
            if (botInfo.branchInfoId == 2)
            {
                facility = "Ehc";
            }


            //string doctorsurl = "https://Applink.evercarebd.com:8018/api/Employee/" + facility;

            //var token = await GetActiveToken();
            //var doctors = await ApiCall.GetApiResponseAsync<List<DoctorVm>>(doctorsurl, token.token);


            //var filteredDoctors = doctors.OrderBy(x => x.id).Where(x => x.name.ToLower().IndexOf(doctorKey.ToLower()) >= 0).ToList();
            var filteredDoctors = new List<DoctorVm>();
            var doc = await _context.DoctorInfos.Where(x => x.branchInfoId == botInfo.branchInfoId && x.name.ToLower().IndexOf(doctorKey.ToLower()) >= 0 && x.isDelete != 1).Include(x => x.doctorSpecialization).ToListAsync();

            var combinedDoctors = filteredDoctors.Concat(doc.Select(d => new DoctorVm
            {
                id = (int)d.ApiDoctorId,
                name = d.name,
                code = d.doctorCode,
                department = d.departmentName,
                gender = d.gender,
                specialization = d.doctorSpecialization.name,
                facility = facility
            })).ToList();

            return combinedDoctors;
            //return filteredDoctors;
        }


        #endregion

        public async Task<List<string>> GetSearchByTypingText(string msg, string connectionId, string botKey, string userId)
        {
            var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey)
                                                   .Select(x => x.ApplicationUser)
                                                   .AsNoTracking()
                                                   .FirstOrDefaultAsync();
            var wrapperDetails = await _context.ConnectionInfos.Where(x => x.userId == userId && x.branchInfoId == user.branchId).Include(x => x.wrapperDetails)
                                                               .AsNoTracking()
                                                               .FirstOrDefaultAsync();

            var result = new List<string>();

            if (msg != "menu")
            {
                var textArr = new List<BotKnowledge>();

                if (wrapperDetails.wrapperDetailsId == 1)
                {
                    textArr = await _context.BotKnowledges.AsNoTracking()
                                                           .Where(x => x.botKey == botKey && x.keyWordQuesAns.departmentId != null && x.branchInfoId == user.branchId)
                                                           .ToListAsync();
                }
                else if (wrapperDetails.wrapperDetailsId == 2)
                {
                    textArr = await _context.BotKnowledges.AsNoTracking()
                                                           .Where(x => x.botKey == botKey && x.keyWordQuesAns.doctorId != null && x.branchInfoId == user.branchId)
                                                           .ToListAsync();
                }
                else if (wrapperDetails.wrapperDetails.firstMessage == "Doctor search by Specializations")
                {
                    textArr = await _context.BotKnowledges.AsNoTracking()
                                                           .Where(x => x.botKey == botKey && x.keyWordQuesAns.specializationId != null && x.branchInfoId == user.branchId)
                                                           .ToListAsync();
                }
                else
                {
                    textArr = await _context.BotKnowledges.AsNoTracking()
                                                           .Where(x => x.botKey == botKey && x.branchInfoId == user.branchId)
                                                           .ToListAsync();
                }

                var data = new List<TextSimilarityVm>();
                foreach (var text in textArr)
                {
                    double similarity = TextMatcher.CalculateLevenshteinSimilarity(msg.ToLower(), text.question.ToLower());

                    if (text.question.ToLower().IndexOf(msg.ToLower()) >= 0)
                    {
                        similarity = 1;
                    }



                    if (similarity >= 0.50 && similarity <= 1) // Check if similarity is within range
                    {
                        data.Add(new TextSimilarityVm
                        {
                            text = text.question,
                            percentage = similarity,
                            KnowledgeId = text.Id
                        });

                        if (similarity == 1) // Stop if exact match
                        {
                            break;
                        }
                    }
                }

                var maxPercentageKnowledgeId = data.OrderByDescending(x => x.percentage).FirstOrDefault()?.KnowledgeId;
                if (maxPercentageKnowledgeId != null && maxPercentageKnowledgeId > 0)
                {
                    //var matchedText = data.FirstOrDefault(x => x.KnowledgeId == maxPercentageKnowledgeId)?.text;
                    //if (matchedText != null)
                    //{
                    //    result.Add(matchedText);
                    //}
                    result.Add("match found");
                }
                else
                {
                    result.Add("No match found");
                }
            }

            return result;
        }



        #region Api Data List New

        public async Task<int> SaveApiSpecialisationData(List<ApiSpecialisationData> models)
        {
            var spec = await _context.ApiSpecialisationData.AsNoTracking().ToListAsync();
            _context.ApiSpecialisationData.RemoveRange(spec);
            await _context.SaveChangesAsync();


            _context.ApiSpecialisationData.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }

        public async Task<int> SaveApiDoctorData(List<ApiDoctorData> models)
        {
            var docts = await _context.ApiDoctorData.AsNoTracking().ToListAsync();
            _context.ApiDoctorData.RemoveRange(docts);
            await _context.SaveChangesAsync();

            _context.ApiDoctorData.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }
        public async Task<int> SaveApiDepartmentData(List<ApiDepartmentData> models)
        {
            var depts = await _context.ApiDepartmentData.AsNoTracking().ToListAsync();
            _context.ApiDepartmentData.RemoveRange(depts);
            await _context.SaveChangesAsync();

            _context.ApiDepartmentData.AddRange(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }
        #endregion

        #region Bot Message History
        public async Task<int> SaveBotTextHistory(BotMessageHistory data)
        {
            _context.botMessageHistories.Add(data);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BotMessageHistory>> GetHistoryData(string pageUserId, int wrapperDetailsId)
        {
            return await _context.botMessageHistories.Where(x => x.userId == pageUserId && x.wraperdId == wrapperDetailsId).ToListAsync();
        }

        public async Task<IEnumerable<object>> SearchBotMessageHistory(DateTime? startDate, DateTime? endDate)
        {
            var data = await _context.botMessageHistories
                .Where(x => (!startDate.HasValue || x.entryDate >= startDate) &&
                            (!endDate.HasValue || x.entryDate <= endDate))
                .ToListAsync();

            var groupedData = data
                .GroupBy(x => new { x.userId, x.connectionId, x.wraperdId })
                .Select(g => new
                {
                    g.Key.userId,
                    g.Key.connectionId,
                    g.Key.wraperdId,
                    MessageCount = g.Count()
                }).ToList();

            return groupedData;
        }

        public async Task<IEnumerable<BotMessageHistory>> GetHistoryData(string pageUserId, int wrapperDetailsId, string connectionId)
        {
            return await _context.botMessageHistories.Where(x => x.userId == pageUserId && x.wraperdId == wrapperDetailsId && x.connectionId == connectionId).ToListAsync();
        }

        #endregion
    }

}
