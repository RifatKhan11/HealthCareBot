using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.HUB
{
    public class TotaChatprenext : Hub
    {
        private readonly IHubServiceManagerOld hubServiceManager;
        private readonly IBotFlowService botFlowService;
        private readonly IServiceFlowService serviceFlowService;
        private readonly IMenuService menuService;
        private readonly IQuestionReplayService questionReplayService;
        private readonly IFacebookService facebookService;
        private readonly UserManager<ApplicationUser> _userManager;
      //  private  string BEE;

        public TotaChatprenext(IHubServiceManagerOld hubServiceManager, UserManager<ApplicationUser> userManager, IFacebookService facebookService, IQuestionReplayService questionReplayService, IMenuService menuService, IBotFlowService botFlowService, IServiceFlowService serviceFlowService)
        {
            this.hubServiceManager = hubServiceManager;
            this.botFlowService = botFlowService;
            this.serviceFlowService = serviceFlowService;
            this.menuService = menuService;
            this.questionReplayService = questionReplayService;
            this.facebookService = facebookService;
            //this.BEE = BEE;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Menu>> GetData(int parentId,string pageid)
        {
         
            int FbPageId = await facebookService.GetFacebookpageId(pageid);
            var data = await menuService.GetMenusByParrentId(parentId, FbPageId);
            return data;


        }
        string BE = "";
        string BEE = "";
        string passportdata = "";
        public async Task Send(string nick, string messagejson)
        {

            dynamic Data = (dynamic)null;
            try
            {
                #region Data Filtering..
                
                Data = JsonConvert.DeserializeObject(messagejson);
               
                string message = Data.message;
                string pageId = Data.pageId;
               
                string postBack = Data.postback;
                string combinedId = pageId + nick;
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
                if (postBack.StartsWith("menues")||postBack.StartsWith("menuel"))
                {
                    string s = postBack;
                    values = s.Split(',');
                    if (values[3] == "0")
                    {
                        BE = "Bangla";
                    }
                    else
                    {
                        BE = "English";
                    }
                    postBack = values[0];
                }
                if (postBack.StartsWith("passport"))
                {
                    passportdata = postBack;
                    string s = postBack;
                    values = s.Split(',');
                    if (values[1] == "Bangla")
                    {
                        BEE = "Bangla";
                    }
                    else
                    {
                        BEE = "English";
                    }
                    postBack = values[0];
                }



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
                else if (postBack == "passport")
                {
                    Console.WriteLine("\n\n\n\n Post Back: " + postBack + "\n\n\n\n");
                    //flow = botFlowService.UpdateFlow(combinedId, postBack);

                    flow = botFlowService.UpdateFlow(combinedId, passportdata);
                    string s = flow;
                    values = s.Split(',');
                    flow = values[0];
                    BEE = values[1];
                    serviceFlowService.InitNewService(combinedId, postBack, "start");
                }
                else if (postBack == "menues")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "getmenu");


                }
                else if (postBack == "menuel")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "menuel");


                }
                else if (postBack == "menuesq")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "getmenuq");


                }
                else
                {
                    flow = botFlowService.GetCurrentFlowStatus(combinedId);
                    if (flow.StartsWith("passport"))
                    {
                        string s = flow;
                        values = s.Split(',');
                        flow = values[0];
                        BEE = values[1];
                    }
                }
                #endregion

                List<string> messages = new List<string>();
                #region Bot Design


                Console.WriteLine("\n\n\n\n\nflow: " + flow + "\n\n\n\n");


                if (flow == "default")
                {
                    if (message != "")
                        messages = await hubServiceManager.QuesReplayService(nick, pageId, message, postBack);
                    // else messages.Add("{ \"msg\":\"Hi There, <br> How Can I help you?\"}");

                    else messages.Add("{ \"msg\":\"Please Select Language from Below <br> <button onclick='ClickedMenuMenu(0)' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Bangla</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>English</button>\"}");
                }

                if (flow == "getmenu")
                {
                    string btndata = string.Empty;
                    var data = await GetData(Convert.ToInt32(values[1]), pageId);
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

                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";


                            }
                            else
                            {
                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
                            }







                        }
                    }
                    else
                    {
                        Menu menu = await menuService.GetMenusbyId(Convert.ToInt32(values[1]));
                        if (menu.IsLast == true)
                        {
                            var qdata = await questionReplayService.GetquestionbymenuID(menu.Id);
                            if (qdata != null)
                            {
                                if (BE == "Bangla")
                                {
                                    messages.Add("{ \"msg\":\"&nbsp;" + qdata.QuestionText + "<br> <button onclick='ClickedMenuMenuQ(" + qdata.Id + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-3)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                else
                                {

                                    messages.Add("{ \"msg\":\"&nbsp;" + qdata.QuestionTextEN + "<br> <button onclick='ClickedMenuMenuQ(" + qdata.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-4)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                                }

                            }
                            else
                            {
                                if (BE == "Bangla")
                                {
                                    btndata = string.Empty;
                                    data = await GetData(0, pageId);
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                    }
                                    //messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                                }
                                else
                                {

                                    btndata = string.Empty;
                                    data = await GetData(0, pageId);
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                    }
                                    // messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");
                                }

                            }

                        }
                        else
                        {
                            if (BE == "Bangla")
                            {
                                messages.Add("{ \"msg\":\"Hi There, <br> How Can I help you?\"}");
                            }
                            else
                            {

                                messages.Add("{ \"msg\":\"Hi There, <br> How Can I help you?\"}");
                            }
                        }

                    }

                    if (message != "")
                        messages = await hubServiceManager.QuesReplayService(nick, pageId, message, postBack);
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
                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

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
                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                            }
                            messages.Add("{ \"msg\":\"Please Select Menu from Below <br>" + btndata + "</div>\"}");


                        }
                    }

                }
                else if (flow == "menuel")
                {
                    if (BE == "Bangla")
                    {
                        messages.Add("{ \"msg\":\"আপনি কি পুলিশ ক্লিয়ারেন্সের জন্য আবেদন করেছেন?<br><br><button onclick='ClickedMenuMenuQQ(" +1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                    }
                    else
                    {
                        messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button>\"}");
                    }
                   
                }
                else if (flow == "wdefault")
                {
                    if (message != "")
                        messages = await hubServiceManager.QuesReplayService(nick, pageId, message, postBack);
                    else
                    {
                        messages.Add("{ \"msg\":\"<b>Welcome to police clearance system.</b><br><br>Please select language from below: <br> <button onclick='ClickedMenuMenul(0)' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Bangla</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenul(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>English</button>\"}");
                        //  messages.Add("{ \"msg\":\"Please Select Language from Below <br> <button onclick='ClickedMenuMenu(0)' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Bangla</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>English</button>\"}");
                        //messages.Add("{ \"msg\":\"Please enter your passport number or Application Referance number.\"}");
                    }
                    //messages = await hubServiceManager.CustoomVisaService(nick, pageId, message);
                }
                else if (flow == "passport")
                {
                    if (BEE == "Bangla")
                    {
                        // messages = await hubServiceManager.CustoomVisaServiceBN(nick, pageId, message);
                        messages = await hubServiceManager.CustoomVisaService(nick, pageId, message);
                    }
                    else
                    {
                        messages = await hubServiceManager.CustoomVisaService(nick, pageId, message);
                    }
                }

                #endregion




                #region Sending Message
                //await Clients.Caller.SendAsync("Send", nick, "Please wait...");
                foreach (string msg in messages)
                {
                    //string MSGS= msg+flow.ToString();
                    await Clients.Caller.SendAsync("Send", nick, msg);
                }
                #endregion



            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n Err:" + e.Message);
            }
        }

    }
}
