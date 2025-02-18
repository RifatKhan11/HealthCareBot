using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.HUB
{
    [HubName("chat")]
    public class TotaChat : Hub
    {
        private readonly IHubServiceManager hubServiceManager;
        private readonly IBotFlowService botFlowService;
        private readonly IServiceFlowService serviceFlowService;
        private readonly IMenuService menuService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly IFacebookService facebookService;
        private readonly IKnowledgeService knowledgeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserInfoService userInfoService;
        private readonly IKeyWordQuesService keyWordQuesService;
        private readonly IDoctorInfoService _doctorInfoService;
        private readonly IBotService _botService;

        //  private  string BEE;

        public TotaChat(IHubServiceManager hubServiceManager, IBotService _botService, IDoctorInfoService doctorInfoService, IKeyWordQuesService keyWordQuesService, IUserInfoService userInfoService, UserManager<ApplicationUser> userManager, IKnowledgeService knowledgeService, IFacebookService facebookService, IQuestionReplayService questionReplayService, IMenuService menuService, IBotFlowService botFlowService, IServiceFlowService serviceFlowService)
        {
            this.hubServiceManager = hubServiceManager;
            this.botFlowService = botFlowService;
            this.serviceFlowService = serviceFlowService;
            this.menuService = menuService;
            this._botService = _botService;
            this.questionReplayService = questionReplayService;
            this.facebookService = facebookService;
            this.knowledgeService = knowledgeService;
            this.userInfoService = userInfoService;
            this.keyWordQuesService = keyWordQuesService;
            _doctorInfoService = doctorInfoService;
            //this.BEE = BEE;
            _userManager = userManager;
        }
        //public bool saveMenuHitLog(int menuId, int FbPageId)
        //{
        //    MenuHitLog menuHitLog = new MenuHitLog
        //    {
        //        menuId = menuId,
        //        facebookPageId = FbPageId,
        //        dateTime=DateTime.Now
        //    };
        //    menuService.SaveMenuHitLog(menuHitLog);

        //    return true;
        //}

        public async Task<IEnumerable<Menu>> GetData(int parentId, string pageid)
        {

            int FbPageId = await facebookService.GetFacebookpageId(pageid);
            var data = await menuService.GetMenusByParrentId(parentId, FbPageId);
            return data;


        }

        public async Task<IEnumerable<Menu>> GetDataByBotKey(int parentId, string pageid, string botKey)
        {

            int FbPageId = await facebookService.GetFacebookpageId(pageid);
            var data = await _botService.GetDataByBotKey(parentId, FbPageId, botKey);
            return data;


        }
        string BE = "";
        string BEE = "";
        string passportdata = "";
        //int isdeclare = 0;
        public async Task Send(string nick, string messagejson, string userId)
        {

            string connectionId = Context.ConnectionId;

            var httpContext = Context.GetHttpContext(); // Get HttpContext in SignalR hub

            if (httpContext == null)
            {
                Console.WriteLine("HttpContext is not available.");
                return;
            }

            // Try to get IP from X-Forwarded-For
            string ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            // If X-Forwarded-For is not available, fall back to RemoteIpAddress
            if (string.IsNullOrEmpty(ipAddress))
            {
                var ip = httpContext.Connection.RemoteIpAddress;
                if (ip != null && ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4(); // Convert IPv6-mapped IPv4 to IPv4
                }
                ipAddress = ip?.ToString() ?? "IP Not Found";
            }

            var clickedWrapper = await _botService.GetConnectionInfoByUserId(userId);


            dynamic Data = (dynamic)null;
            try
            {
                #region Data Filtering..



                Data = JsonConvert.DeserializeObject(messagejson);

                string receivedMsg = Data.message == "" ? Data.postback.ToString().Split(",")[2] : Data.message;// postback.ToString().Split(",")[2];

                string message = Data.message;
                string pageId = Data.pageId;

                string postBack = Data.postback;

                string botKey = Data.botKey;

                string combinedId = pageId + nick;

                #region History For Chat
                string[] wraperText = { "Doctor search by Specializations", "Search by doctor", "appointment", "About Evercare Hospital Dhaka" };
                var pageUserId = Context.GetHttpContext()?.Request.Query["access_token"];

                if (string.IsNullOrEmpty(userId))
                {
                    pageUserId = Guid.NewGuid().ToString(); // Fallback if missing
                }
                if (clickedWrapper != null && !wraperText.Contains(message))
                {
                    var data = new BotMessageHistory
                    {
                        userId = pageUserId,
                        wraperdId = (int)clickedWrapper.wrapperDetailsId,
                        wraperdText = ipAddress,
                        masseges = "{ \"msg\":\"<div class='response'><div class='response-date' style='color: rgba(0, 0, 0, 0.7);'></div><div user='' class='response-wrapper'><div class='user response-content' style='width:270px;'><div id='message' class='message' style='color: rgb(255, 255, 255); background: rgb(0, 108, 255); border-color: rgb(0, 108, 255);'>" + message + "</div> <div class='info' style='color: rgb(100, 100, 100);'></div></div></div></div>textSend\"}",
                        messagesType = 1,
                        connectionId = connectionId,
                        entryDate = DateTime.Now
                    };
                    await _botService.SaveBotTextHistory(data);
                }
                #endregion

                #endregion

                Console.WriteLine("\n\n\n");
                Console.WriteLine(nick + " PageId : " + pageId);
                Console.WriteLine(nick + " Post Back : " + postBack);
                Console.WriteLine(nick + " Saying : " + message);
                Console.WriteLine(nick + " Saying : " + messagejson);
                Console.WriteLine(nick + " CombinedId : " + combinedId);
                Console.WriteLine("\n\n\n");

                #region flow Deside
                string[] values = new string[] { "a", "b", "c", "d" };
                int menuId = 0;
                string PassportNo = "";
                //if (message.ToLower() == "yes" && postBack.Contains("KWA"))
                //{
                //    values = postBack.Split(";");
                //    int Id = Convert.ToInt32(values[1]);

                //    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                //    var unknownquestion = await keyWordQuesService.getunknowquestionbyuserid(userId);
                //    KnowledgeHitLog knowledgeHitLog = new KnowledgeHitLog
                //    {
                //        keyWordQuesAnsId=Id,
                //        facebookPageId=FbPageId,
                //        unKnownKeyWordQuestionId=unknownquestion.Id,
                //        dateTime=DateTime.Now,
                //        botKey = botKey,
                //        connectionId = connectionId
                //    };
                //  await  knowledgeService.SaveKnowledgeHitLog(knowledgeHitLog);


                //}


                //if (postBack.StartsWith("menues") || postBack.StartsWith("menuel"))
                //{
                //    string s = postBack;
                //    values = s.Split(',');
                //    if (values[3] == "0")
                //    {
                //        BE = "Bangla";
                //    }
                //    else
                //    {
                //        BE = "English";
                //    }
                //    postBack = values[0];
                //    menuId= Convert.ToInt32(values[1]);
                //    if(menuId==-1||menuId==-1)
                //    {
                //        menuId = 0;
                //        postBack = "menu";
                //        message = "menu";
                //    }
                //    else if (menuId == 0 || menuId == 0)
                //    {
                //        menuId = 0;
                //        postBack = "menu";
                //        message = "মেনু";
                //    }
                //}
                //if (postBack.StartsWith("passport"))
                //{
                //    passportdata = postBack;
                //    string s = postBack;
                //    values = s.Split(',');
                //    if (values[1] == "Bangla")
                //    {
                //        BEE = "Bangla";
                //    }
                //    else
                //    {
                //        BEE = "English";
                //    }
                //    postBack = values[0];
                //}

                //if (postBack.StartsWith("complain-data"))
                //{
                //    passportdata = postBack;
                //    string s = postBack;
                //    values = s.Split(',');

                //    postBack = values[0];
                //    PassportNo= values[1];
                //}
                //if (postBack.StartsWith("Suggestion-data"))
                //{
                //    passportdata = postBack;
                //    string s = postBack;
                //    values = s.Split(',');

                //    postBack = values[0];
                //    PassportNo = values[1];
                //}
                //if (postBack.StartsWith("appointment"))
                //{
                //    passportdata = postBack;
                //    string s = postBack;
                //    values = s.Split(',');

                //    postBack = values[0];
                //    PassportNo = values[1];
                //}


                string flow = "default";
                var x = new List<Menu>();
                if (postBack == "wstart")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "wdefault");
                    serviceFlowService.CLearServiceData(combinedId);
                }
                else if (postBack == "nstart")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "default");
                    serviceFlowService.CLearServiceData(combinedId);
                }
                //else if (postBack == "passport")
                //{
                //    Console.WriteLine("\n\n\n\n Post Back: " + postBack + "\n\n\n\n");
                //    //flow = botFlowService.UpdateFlow(combinedId, postBack);

                //    flow = botFlowService.UpdateFlow(combinedId, passportdata);
                //    string s = flow;
                //    values = s.Split(',');
                //    flow = values[0];
                //    BEE = values[1];
                //    serviceFlowService.InitNewService(combinedId, postBack, "start");
                //}
                else if (postBack == "menues")
                {

                    flow = botFlowService.UpdateFlow(combinedId, "getmenu");
                    int FbPageId = await facebookService.GetFacebookpageId(pageId);
                    MenuHitLog menuHitLog = new MenuHitLog
                    {
                        menuId = menuId,
                        facebookPageId = FbPageId,
                        dateTime = DateTime.Now,
                        botKey = botKey,
                        connectionId = connectionId
                    };
                    await menuService.SaveMenuHitLog(menuHitLog);


                }
                //else if (postBack == "menuel")
                //{
                //    flow = botFlowService.UpdateFlow(combinedId, "menuel");


                //}
                //else if (postBack == "complain-data")
                //{
                //    flow = botFlowService.UpdateFlow(combinedId, "complain-data");


                //}
                //else if (postBack == "Suggestion-data")
                //{
                //    flow = botFlowService.UpdateFlow(combinedId, "Suggestion-data");


                //}
                //else if (postBack == "menuesq")
                //{
                //    flow = botFlowService.UpdateFlow(combinedId, "getmenuq");

                //}
                else if (postBack == "appointment")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "appointment");

                }
                else
                {
                    flow = botFlowService.GetCurrentFlowStatus(combinedId);

                    flow = "wdefault";
                }
                #endregion

                List<string> messages = new List<string>();
                #region Bot Design


                Console.WriteLine("\n\n\n\n\nflow: " + flow + "\n\n\n\n");


                if (flow == "default")
                {
                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
                    if (message != "")

                    {
                        if (userInfo.Id != null)
                        {
                            postBack = userInfo.Id;
                        }
                        else
                        {
                            postBack = "";
                        }
                        messages = await hubServiceManager.CustoomVisaService(nick, pageId, message, postBack, userId);
                    }
                    else
                    {

                        messages.Add("{ \"msg\":\"Please Select Language from Below <br> <button onclick='ClickedMenuMenu(0)' class='btn-menu' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Bangla</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' class='btn-menu' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>English</button>\"}");
                    }


                }


                var menuInfo = new Menu();
                if (flow == "getmenu")
                {
                    menuInfo = await _botService.GetMenuByIdAndBotKey(Convert.ToInt32(values[1]), botKey);
                }

                if (menuInfo != null && menuInfo.IsLast == true && menuInfo.responseApi != null)
                {
                    flow = "getquestions";
                }

                if (flow == "wdefault")
                {
                    if (Data.dateTxt != null)
                    {
                        var botInfo = await _botService.GetChatBotInfoByBotKey(botKey);
                        var sflow = new ServiceFlow
                        {
                            questionText = "AppointDate",
                            answerText = Data.dateTxt,
                            InfoType = "start",
                            ServiceCode = "Pre-Defined Question",
                            DateTime = DateTime.Now,
                            StepNo = 1,
                            Attempt = 0,
                            botKey = botKey,
                            connectionId = connectionId,
                            status = 1,
                            MenuId = 0,
                            branchInfoId = botInfo.ApplicationUser?.branchId
                        };

                        await _botService.SaveServiceFlow(sflow);
                    }
                    var hasQuestionReply = await _botService.GetPendingQuestion(botKey, connectionId, receivedMsg);

                    if ((receivedMsg.ToLower().Trim() == "hi" || receivedMsg.ToLower().Trim() == "hello") && clickedWrapper.wrapperDetailsId == 1)
                    {
                        hasQuestionReply.answerText = null;

                        await _botService.SaveServiceFlow(hasQuestionReply);
                    }
                    if ((receivedMsg.ToLower().Trim() == "hi" || receivedMsg.ToLower().Trim() == "hello") && clickedWrapper.wrapperDetailsId == 2)
                    {
                        hasQuestionReply.answerText = null;

                        await _botService.SaveServiceFlow(hasQuestionReply);
                    }



                    if (hasQuestionReply != null && hasQuestionReply.questionText != "Enter Dr. Name" && hasQuestionReply.questionText != "Gender" && receivedMsg.ToLower().Trim() != "hi" && receivedMsg.ToLower().Trim() != "hello")
                    {

                        var data = await _botService.GetSearchByTypingText(message, connectionId, botKey, userId);
                        if (data.Contains("No match found"))
                        {
                            hasQuestionReply.answerText = null;

                            await _botService.SaveServiceFlow(hasQuestionReply);
                        }
                        else
                        {
                            hasQuestionReply.answerText = receivedMsg;
                            hasQuestionReply.status = 1;

                            await _botService.SaveServiceFlow(hasQuestionReply);

                            if (hasQuestionReply.MenuId > 0)
                            {
                                messages = await _botService.CustomInputMessageGenerator(hasQuestionReply.MenuId, botKey, connectionId);
                            }

                        }


                    }

                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
                    if (messages.Count() == 0 && clickedWrapper != null)
                    {
                        messages = await _botService.CustomMessageGenerator(nick, pageId, message, postBack, userId, botKey, connectionId, messagejson, clickedWrapper);
                    }


                }
                else if (flow == "getmenu")
                {
                    string btndata = string.Empty;
                    var data = await GetDataByBotKey(Convert.ToInt32(values[1]), pageId, botKey);






                    int i = 0;
                    foreach (Menu d in data)
                    {
                        i++;
                    }
                    if (i > 0)
                    {
                        foreach (Menu m in data)
                        {

                            if (BE == "Bangla")
                            {

                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";


                            }
                            else
                            {
                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
                            }

                        }
                    }
                    else
                    {
                        var qdata = await _doctorInfoService.GetDoctorListbymenuid(Convert.ToInt32(values[1]));
                        if (BE == "Bangla")
                        {
                            //if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                            //{
                            //    messages.Add("{ \"msg\":\"" + qdata.AnswerText + "</div>\"}");
                            //}
                            if (qdata.Count() > 0)
                            {
                                foreach (DoctorInfo d in qdata)
                                {
                                    btndata += @"<div>" +
                                                    "<span>" + d.name + ", " + d.designationName + ", " + d.departmentName + "</span>" +
                                                    "<button onclick =ClickedMenuMenu(" + d.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Appointment</button>" +
                                                "</div>";
                                }
                                //messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");
                            }
                            else
                            {
                                btndata = string.Empty;
                                data = await GetData(0, pageId);
                                foreach (Menu m in data)
                                {
                                    btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                }
                                messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");


                            }


                        }
                        else
                        {
                            if (qdata.Count() > 0)
                            {
                                foreach (DoctorInfo d in qdata)
                                {
                                    btndata += @"<div>" +
                                                    "<span>" + d.name + ", " + d.designationName + ",</span><br /><span>" + d.departmentName + "</span><br />" +
                                                    "<div style='text -align:center'><span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 1 + "," + 1 + "," + "''" + ")>S</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 2 + "," + 1 + "," + "''" + ")>M</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 3 + "," + 1 + "," + "''" + ")>T</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 4 + "," + 1 + "," + "''" + ")>W</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 5 + "," + 1 + "," + "''" + ")>T</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 6 + "," + 1 + "," + "''" + ")>F</span> <span class='dot' onclick =ClickedAppointment(" + d.Id + "," + 7 + "," + 1 + "," + "''" + ")>S</span></div>" +
                                                    "<button onclick =ClickedAppointment(" + d.Id + "," + 1 + "," + 1 + "," + "''" + ") id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Appointment</button>" +
                                                "</div><p>____________________________</p>";
                                }
                                //messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");
                            }
                            //if (qdata.AnswerTextEN != string.Empty || qdata.AnswerTextEN != null)
                            //{
                            //    messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                            //}
                            else
                            {

                                btndata = string.Empty;

                                data = await GetDataByBotKey(0, pageId, botKey);
                                foreach (Menu m in data)
                                {
                                    btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                }
                                messages.Add("{ \"msg\":\"Please Choose One <br>" + btndata + "</div>\"}");


                            }
                        }

                    }

                    if (message != "")
                        messages = await hubServiceManager.QuesReplayService(nick, pageId, message, postBack, userId);
                    //else messages.Add("{ \"msg\":\"Hi There, <br> How Can I help you?\"}");

                    else
                    {
                        if (btndata != "")
                        {

                            if (BE == "Bangla")
                            {
                                messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                            }
                            else
                            {
                                messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");
                            }
                        }


                    }

                }
                else if (flow == "getmenuq")
                {
                    var qdata = await questionReplayService.GetanswerbyqID(Convert.ToInt32(values[1]));

                    if (BE == "Bangla")
                    {
                        if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                        {
                            messages.Add("{ \"msg\":\"" + qdata.AnswerText + "</div>\"}");
                        }
                        else
                        {
                            string btndata = string.Empty;
                            var data = await GetData(0, pageId);
                            foreach (Menu m in data)
                            {
                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                            }
                            messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");


                        }


                    }
                    else
                    {
                        if (qdata.AnswerTextEN != string.Empty || qdata.AnswerTextEN != null)
                        {
                            messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                        }
                        else
                        {
                            string btndata = string.Empty;
                            var data = await GetData(0, pageId);
                            foreach (Menu m in data)
                            {
                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                            }
                            messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");


                        }
                    }

                }
                else if (flow == "appointment")
                {
                    var qdata = await questionReplayService.GetanswerbyqID(Convert.ToInt32(values[1]));

                    if (BE == "Bangla")
                    {
                        if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                        {
                            messages.Add("{ \"msg\":\"" + qdata.AnswerText + "</div>\"}");
                        }
                        else
                        {
                            string btndata = string.Empty;
                            var data = await GetData(0, pageId);
                            foreach (Menu m in data)
                            {
                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                            }
                            messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");


                        }


                    }
                    else
                    {
                        if (qdata.AnswerTextEN != string.Empty || qdata.AnswerTextEN != null)
                        {
                            messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                        }
                        else
                        {
                            string btndata = string.Empty;
                            var data = await GetData(0, pageId);
                            foreach (Menu m in data)
                            {
                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") class='btn-menu' id = 'btnbanglas' class = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                            }
                            messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");


                        }
                    }

                }

                //else if(flow == "getquestions")
                //{
                //    messages = await _botService.CustomInputMessageGenerator(menuId, botKey, connectionId);


                //}

                #endregion




                #region Sending Message
                foreach (string msg in messages)
                {

                    await Clients.Caller.SendAsync("Send", nick, msg.ToString());
                    await Console.Out.WriteLineAsync(nick + " :::: " + msg);


                    if (clickedWrapper != null && !wraperText.Contains(message))
                    {
                        var data = new BotMessageHistory
                        {
                            userId = pageUserId,
                            wraperdId = (int)clickedWrapper.wrapperDetailsId,
                            wraperdText = ipAddress,
                            masseges = msg,
                            messagesType = 2,
                            connectionId = connectionId,
                            entryDate = DateTime.Now
                        };
                        await _botService.SaveBotTextHistory(data);
                    }

                    int? menId = menuId;

                    if (menId == 0)
                    {
                        menId = null;
                    }

                    var qId = await _botService.GetQuestionByText(msg);

                    #endregion
                }

                if (clickedWrapper != null && wraperText.Contains(message))
                {
                    var datas = await _botService.GetHistoryData(pageUserId, (int)clickedWrapper.wrapperDetailsId);

                    if (datas != null)
                    {
                        foreach (var msgs in datas)
                        {
                            await Clients.Caller.SendAsync("Send", nick, msgs.masseges.ToString());
                            await Console.Out.WriteLineAsync(nick + " :::: " + msgs.masseges);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n Err:" + e.Message);
            }
        }

    }
}
