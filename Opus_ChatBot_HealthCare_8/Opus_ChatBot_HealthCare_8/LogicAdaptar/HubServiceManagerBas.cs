using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Interface;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar
{
    public class HubServiceManagerBas : IHubServiceManager
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

        public HubServiceManagerBas(IServiceFlowService serviceFlowService, IquestionCategoryService iquestionCategoryService, IQueriesService queriesService, IQuestionReplayService questionReplayService, IMenuService menuService, IKnowledgeService knowledgeService, IBotFlowService botFlowService, IUserInfoService userInfoService, IOTPService oTPService, IPassportInfoService passportInfoService, IBankInfoService bankInfoService, IFacebookService facebookService, IKeyWordQuesService keyWordQuesService)
        {
            this.serviceFlowService = serviceFlowService;
            this.botFlowService = botFlowService;
            this.userInfoService = userInfoService;
            this.oTPService = oTPService;
            this.passportInfoService = passportInfoService;
            this.bankInfoService = bankInfoService;
            this.facebookService = facebookService;
            this.keyWordQuesService = keyWordQuesService;
            this.knowledgeService = knowledgeService;
            this.menuService = menuService;
            this.questionReplayService = questionReplayService;
            this.queriesService = queriesService;
            this.iquestionCategoryService = iquestionCategoryService;
        }


        //[HttpGet("{id}", Name = "GetRanking")]
        //public async Task<IActionResult> GetByIdAync(long id)
        //{
        //    var client = new RestClient($"http://api.football-data.org/v1/competitions/{id}/leagueTable");
        //    var request = new RestRequest(Method.GET);
        //    IRestResponse response = await client.ExecuteAsync(request);

        //    //TODO: transform the response here to suit your needs

        //    return OkResult(response);
        //}


        public async Task<IEnumerable<Menu>> GetData(string parentId, string pageid)
        {

            int FbPageId = await facebookService.GetFacebookpageId(pageid);
            var data = await menuService.GetMenusByParrentname(parentId, FbPageId);
            return data;


        }
        public async Task<List<string>> CustoomVisaService(string senderId, string pageId, string message, string postback, string userId)
        {
            //var datax = new Uri("http://pcc.police.gov.bd:8080/ords/pcc2/hr/getapplicantinfo/EE041699");
            message = message.Trim();

            if (message.ToLower() != "no")
            {
                var datax = await passportInfoService.GETApi(message);
            }

            int falsestep = 0;
            string combinedId = pageId + senderId;
            int FFbPageId = await facebookService.GetFacebookpageId(pageId);
            List<string> messages = new List<string>();
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);
            if (serviceFlow == null)
            {
                falsestep = 1;
                ServiceFlow data = new ServiceFlow
                {
                    Id = combinedId,
                    InfoType = "pasportOrref",
                    ServiceCode = "passport",
                    StepNo = 2,
                    DateTime = DateTime.Now,
                    Attempt = 0


                };
                serviceFlow = await serviceFlowService.SaveServiceFlow(data);
            }


            if (serviceFlow.StepNo == 0)
            {
                //if (!InfoValidation.CheckConfirmation(message))
                //{
                //    CloseService(combinedId);
                //}
                //else
                //{
                //    userInfoService.UpdateUserInfo(combinedId, "Passport", "");
                //    messages.Add("{\"msg\" : \"Please enter your passport number or Application Referance number.\"}");
                //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);
                //}

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
                        messages = await QuesReplayService(senderId, pageId, message, postback,userId);
                    }
                    else
                    {
                        //messages.Add("{ \"msg\" : \"Invalid!! passport or application reference number.<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                        messages.Add(" { \"msg\" : \"What's your passport number or application reference number?\"}");
                    }

                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "Passport", message);


                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(message);
                    if (passportInfo == null)
                    {
                        // var lstkeyWordQuesAns=  await knowledgeService.GetAllKnowledgebyPagebyword(message);
                        var lstkeyWordQuesAns = await knowledgeService.GetAllKnowledgebyPagebywordfbid(message, FFbPageId);

                        if (lstkeyWordQuesAns.Count() > 0)
                        {
                            messages = await QuesReplayService(senderId, pageId, message, postback,userId);
                        }
                        else
                        {
                            // messages.Add("{ \"msg\" : \"Invalid!! passport or application reference number.<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                            // messages.Add("{ \"msg\" : \"What's your passport number or application reference number?\"}");
                            // messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            messages = await QuesReplayService(senderId, pageId, message, postback,userId);
                        }


                    }
                    else
                    {
                        //if (passportInfo.gender.ToLower() == "male")
                        //    messages.Add("{ \"msg\" : \"Hello, " + passportInfo.name + ".\"}");
                        //else
                        messages.Add("{ \"msg\" : \"Hello, " + passportInfo.name + ".\"}");
                        messages.Add("{ \"msg\" : \"What's your date of birth (yyyy-mm-dd)?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                        PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                        {
                            passportNo = passportInfo.passportNo,
                            refNo = passportInfo.refNo,
                            date = DateTime.Now,
                            status = "Successfully Entered Passport or Reference Number"
                        };
                        await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
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
                    //messages.Add("{ \"msg\" : \"Your date of birth dosen't Match.<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
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
                    //if (await oTPService.SendEMAIL(passportInfo?.email, "OTP Main From Bangladesh Police", passportInfo?.message))
                    //{
                    //    messages.Add("{ \"msg\" : \"Sir, An OTP Send to your Email.\"}");
                    //    messages.Add("{ \"msg\" : \"Give me that.\"}");
                    //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);
                    //}
                    //else
                    //{
                    //    messages.Add("{ \"msg\" : \"Sir, Something Went Wrong.\"}");
                    //    messages.Add("{ \"msg\" : \"What is your Email According to bank?\"}");
                    //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                    //}
                    //messages.Add("{ \"msg\" : \"Congratulation! Your date of birth matched successfully.<i style='color:yellow;font-size:15px;' class='fas fa-thumbs-up'></i>\"}");
                    messages.Add("{ \"msg\" : \"When will your passport expire (yyyy-mm-dd)?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);

                    PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                    {
                        passportNo = passportInfo.passportNo,
                        refNo = passportInfo.refNo,
                        date = DateTime.Now,
                        status = "Successfully Entered Date of Birth"
                    };
                    await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                var data1 = userInfoService.GetuserInfo(combinedId);
                PassportInfo passportInfo1 = await passportInfoService.GetPassportInfoByPasspoertIds(data1.passport);
                DateTime dateTime = DateTime.Parse(message);
                if (passportInfo1?.expireDate != dateTime)
                {
                    //messages.Add("{ \"msg\" : \"Invalid!! passport expire date.<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
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

                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
                            }
                            messages.Add("{ \"msg\":\"<br>Please Select Menu from Below <br>" + btndata + "</div>\"}");
                        }
                        else
                        {
                            int id = await menuService.GetMenusId(ppvalues[0], FFbPageId);
                            var qdata = await questionReplayService.Getanswerbymenuid(Convert.ToInt32(id));
                            if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                            {
                                messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                                btndata += "<button onclick = 'ClickedComplain("+passportInfo.Id+")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Complain</button> <button onclick = 'ClickedSuggestion("+passportInfo.Id+")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Suggestion</button>";
                                messages.Add("{ \"msg\":\"<br>" + btndata + "</div>\"}");
                            }
                        }
                    }

                    //messages.Add("{ \"msg\" : \"And passport no: " + passportInfo.passportNo + ".\"}");
                    //messages.Add("{ \"msg\" : \"Current Status: " + passportInfo.status + ".\"}");
                    //messages.Add("{ \"msg\" : \"Message :" + passportInfo.message + "\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                    //messages.Add("{ \"msg\" : \"Thanks For taking our service.\"}");
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

            return messages;
        }


        public async Task<List<string>> CustoomVisaServiceBN(string senderId, string pageId, string message, string postBack, string userId)
        {
            int falsestep = 0;
            string combinedId = pageId + senderId;

            List<string> messages = new List<string>();
            int FFbPageId = await facebookService.GetFacebookpageId(pageId);
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);
            if (serviceFlow == null)
            {
                falsestep = 1;
                ServiceFlow data = new ServiceFlow
                {
                    Id = combinedId,
                    InfoType = "pasportOrref",
                    ServiceCode = "passport",
                    StepNo = 2,
                    DateTime = DateTime.Now,
                    Attempt = 0


                };
                serviceFlow = await serviceFlowService.SaveServiceFlow(data);
            }
            Console.WriteLine("\n\n\n From service Service Code : " + serviceFlow.ServiceCode);

            if (serviceFlow.StepNo == 100)
            {
                messages.Add("{ \"msg\" : \"If you help me I can tell you about visa applicatio status.\" ,\"postback\": \"Postback\"}");
                messages.Add("{ \"msg\" : \"Do you agree? Yes/No.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 1);
            }
            else if (serviceFlow.StepNo == 0)
            {
                //if (!InfoValidation.CheckConfirmation(message))
                //{
                //    CloseService(combinedId);
                //}
                //else
                //{
                //    userInfoService.UpdateUserInfo(combinedId, "Passport", "");
                //    messages.Add("{\"msg\" : \"Please enter your passport number or Application Referance number.\"}");
                //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);
                //}

                userInfoService.UpdateUserInfo(combinedId, "Passport", "");
                messages.Add("{\"msg\" : \"আপনার পাসপোর্ট নম্বর অথবা এপ্লিকেশন রেফারেন্স নম্বরটি জানতে পারি?\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);

            }
            else if (serviceFlow.StepNo == 2)
            {
                if (InfoValidation.CheckPassportOrRef(message) == "unknown")
                {
                    //messages.Add("{ \"msg\" : \"ভুল!! পাসপোর্ট নম্বর অথবা এপ্লিকেশন রেফারেন্স নম্বর।<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                    if (falsestep == 1)
                    {
                        messages = await QuesReplayService(senderId, pageId, message, "",userId);
                    }
                    else
                    {
                        messages.Add(" { \"msg\" : \"আপনার পাসপোর্ট নম্বর অথবা এপ্লিকেশন রেফারেন্স নম্বরটি জানতে পারি?\"}");
                    }
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "Passport", message);
                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(message);
                    if (passportInfo == null)
                    {
                        //messages.Add("{ \"msg\" : \"ভুল!! পাসপোর্ট নম্বর অথবা এপ্লিকেশন রেফারেন্স নম্বর।<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                        // var lstkeyWordQuesAns = await knowledgeService.GetAllKnowledgebyPagebyword(message);
                        var lstkeyWordQuesAns = await knowledgeService.GetAllKnowledgebyPagebywordfbid(message, FFbPageId);

                        if (lstkeyWordQuesAns.Count() > 0)
                        {
                            messages = await QuesReplayService(senderId, pageId, message, postBack,userId);
                        }
                        else
                        {
                            messages.Add("{ \"msg\" : \"আপনার পাসপোর্ট নম্বর অথবা এপ্লিকেশন রেফারেন্স নম্বরটি জানতে পারি?\"}");
                        }
                    }
                    else
                    {
                        //if (passportInfo.gender.ToLower() == "male")
                        //    messages.Add("{ \"msg\" : \"হ্যালো , " + passportInfo.name + ".\"}");
                        //else
                        messages.Add("{ \"msg\" : \"হ্যালো , " + passportInfo.name + ".\"}");
                        messages.Add("{ \"msg\" : \"আপনার জন্মতারিখ কি জানতে পারি (yyyy-mm-dd)?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                        PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                        {
                            passportNo = passportInfo.passportNo,
                            refNo = passportInfo.refNo,
                            date = DateTime.Now,
                            status = "Successfully Entered Passport or Reference Number"
                        };
                        await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
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
                    //messages.Add("{ \"msg\" : \"আপনার জন্মতারিখটি পুলিশ ক্লিয়ারেন্স আবেদনের সাথে মিলেনি।<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                    messages.Add("{ \"msg\" : \"দুঃখিত, সঠিক তারিখটি বলুন (yyyy-mm-dd).\"}");
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
                    //if (await oTPService.SendEMAIL(passportInfo?.email, "OTP Main From Bangladesh Police", passportInfo?.message))
                    //{
                    //    messages.Add("{ \"msg\" : \"Sir, An OTP Send to your Email.\"}");
                    //    messages.Add("{ \"msg\" : \"Give me that.\"}");
                    //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);
                    //}
                    //else
                    //{
                    //    messages.Add("{ \"msg\" : \"Sir, Something Went Wrong.\"}");
                    //    messages.Add("{ \"msg\" : \"What is your Email According to bank?\"}");
                    //    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                    //}
                    //messages.Add("{ \"msg\" : \"অভিনন্দন!! আপনার জন্ম তারিখটি সফলভাবে মিলেছে।<i style='color:yellow;font-size:15px;' class='fas fa-thumbs-up'></i>\"}");
                    messages.Add("{ \"msg\" : \"আপনার পাসপোর্টটি কবে মেয়াদোত্তীর্ণ হবে (yyyy-mm-dd)?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);

                    PoliceClearenceLog policeClearenceLog = new PoliceClearenceLog
                    {
                        passportNo = passportInfo.passportNo,
                        refNo = passportInfo.refNo,
                        date = DateTime.Now,
                        status = "Successfully Entered Date of Birth"
                    };
                    await passportInfoService.SavePoliceClearenceLog(policeClearenceLog);
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                var data1 = userInfoService.GetuserInfo(combinedId);
                PassportInfo passportInfo1 = await passportInfoService.GetPassportInfoByPasspoertIds(data1.passport);
                DateTime dateTime = DateTime.Parse(message);
                if (passportInfo1?.expireDate != dateTime)
                {
                    //messages.Add("{ \"msg\" : \"আপনার পাসপোর্টের শেষ তারিখ ভুল হয়েছে।<i style='color:yellow;font-size:15px;' class='fas fa-sad-tear'></i>\"}");
                    messages.Add("{ \"msg\" : \"দুঃখিত, সঠিক তারিখটি বলুন (yyyy-mm-dd). \"}");
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
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
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
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
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
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");

                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");

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
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }

                                }

                            }
                            else
                            {
                                if (passportInfo.status.Contains("null") || passportInfo.status == null)
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                }
                                else
                                {
                                    if (passportInfo.reason.Contains("null") || passportInfo.reason == null)
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
                                    }
                                    else
                                    {
                                        messages.Add("{ \"msg\" : \" আপনার আবেদনের রেফারেন্স নম্বর :<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>পাসপোর্ট নম্বর :<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>বর্তমান অবস্থা :<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>কারণ :<span style='font-weight: bold;'> " + passportInfo.reason + "</span>.<br>মন্তব্যঃ <span style='font-weight: bold;'> " + passportInfo.remarks + "</span>.<br>সম্ভাব্য ডেলিভারি তারিখ :<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate?.ToString("dd-MMM-yyyy") + "</span>.<br>বর্তমান যোগাযোগের নম্বর  :<span style='font-weight: bold;'> " + passportInfo.currentContact + "</span>.<br>আমাদের সার্ভিস গ্রহণের জন্য আপনাকে ধন্যবাদ।\"}");
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

                                btndata += "<button onclick = 'ClickedMenuMenu(" + m.Id + "," + 0 + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";
                            }
                            messages.Add("{ \"msg\":\"<br>দয়াকরে নিচে থেকে নির্বাচন করুন <br>" + btndata + "</div>\"}");
                        }
                        else
                        {
                            int id = await menuService.GetMenusId(ppvalues[0], FFbPageId);
                            var qdata = await questionReplayService.Getanswerbymenuid(Convert.ToInt32(id));
                            if (qdata.AnswerText != string.Empty || qdata.AnswerText != null)
                            {
                                messages.Add("{ \"msg\":\"" + qdata.AnswerText + "</div>\"}");
                                btndata += "<button onclick = 'ClickedComplain("+passportInfo.Id+")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>নালিশ</button> <button onclick = 'ClickedSuggestion(" + passportInfo.Id +")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>প্রস্তাবনা</button>";
                                messages.Add("{ \"msg\":\"<br>" + btndata + "</div>\"}");
                            }
                        }

                    }

                    //messages.Add("{ \"msg\" : \"And passport no: " + passportInfo.passportNo + ".\"}");
                    //messages.Add("{ \"msg\" : \"Current Status: " + passportInfo.status + ".\"}");
                    //messages.Add("{ \"msg\" : \"Message :" + passportInfo.message + "\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                    //messages.Add("{ \"msg\" : \"Thanks For taking our service.\"}");
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

            return messages;
        }

        public async Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack, string userId)
        {
            string combinedId = pageId + senderId;
            List<string> Messages = new List<string>();
            int id = await facebookService.GetFacebookpageId(pageId);
            if (InfoValidation.CheckConfirmationNew(message))
            {

                var messa = await QuesReplayServiceK(senderId, pageId, message, postBack);
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

                    var data = await menuService.GetMenusByParrentId(0, id);
                    string btndata = string.Empty;

                    //foreach (Menu m in data)
                    //{
                    //    btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                    //}
                    //Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                    if (message.Contains("menu") || message.Contains("Menu"))
                    {
                        foreach (Menu m in data)
                        {
                            btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + -1 + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                        }
                        Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                    }
                    else
                    {
                        foreach (Menu m in data)
                        {
                            btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + 0 + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

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
                                btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                            }
                            Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                        }
                        else
                        {
                            var qdata = await questionReplayService.Getanswerbymenuid(menudata.FirstOrDefault().Id);
                            if (qdata.AnswerTextEN != string.Empty || qdata.AnswerTextEN != null)
                            {
                                Messages.Add("{ \"msg\":\"" + qdata.AnswerTextEN + "</div>\"}");
                            }
                            else
                            {

                                 data = await menuService.GetMenusByParrentId(0, id);
                                 btndata = string.Empty;
                                if (data.Count() > 0)
                                {
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

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

                        };
                        await keyWordQuesService.Saveunknownquestion(unKnownKeyWordQuestion);

                        var messa = await QuesReplayServiceK(senderId, pageId, message, postBack);
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
        public async Task<List<string>> QuesReplayServiceK(string senderId, string pageId, string message, string postBack)
        {
            string combinedId = pageId + senderId;
            List<string> Messages = new List<string>();
            int id = await facebookService.GetFacebookpageId(pageId);
            if (postBack == "")
            {
                KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(id, message, combinedId, senderId);

                if (temp.question != null && temp.question != "")
                {

                    string tm1 = new String(message.Where(Char.IsLetter).ToArray());
                    string tm2 = new String(temp.question.Where(Char.IsLetter).ToArray());
                    if (tm1.ToLower() == tm2.ToLower()) // Exact Match
                    {
                        if (temp.answer.Contains("Have you applied for police clearance") || temp.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || temp.answer == "?")
                        {

                            Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" +-1 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + 0+ "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                    btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + -1 + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                }
                                Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                            }
                            else
                            {
                                foreach (Menu m in data)
                                {
                                    btndata += "<button onclick ='ClickedMenuMenu(" + m.Id + "," + 0 + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                }
                                Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                            }

                            // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        }
                        else
                        {
                            if (temp.answer.Contains("?"))
                            {
                                Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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

                        Messages.Add("{ \"msg\":\" Did you mean," + temp.question + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                    // Messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                    //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                    //Messages.Add(response);
                    var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                    string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                    Messages.Add(response);
                    Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                            if (tempy.answer.Contains("Have you applied for police clearance") || tempy.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || tempy.answer == "?")
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
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                    }
                                    Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                                }
                                else
                                {
                                    foreach (Menu m in data)
                                    {
                                        btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                    }
                                    Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                                }

                                // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            }
                            else
                            {
                                if (tempy.answer.Contains("?"))
                                {
                                    Messages.Add("{ \"msg\":\"" + tempy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                            // await queriesService.SaveUserQueries(userQueriesViewModel);
                            // Messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                            //Messages.Add(response);
                            var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                            string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                            Messages.Add(response);
                            Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            // Messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                        }
                    }
                    else
                    {

                        if (temp.answer.Contains("Have you applied for police clearance") || temp.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || temp.answer == "?")
                        {

                            Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1,-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0,0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                if (tempy.answer.Contains("Have you applied for police clearance") || tempy.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || tempy.answer == "?")
                                {

                                    Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + -1 + "," + -1 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(" + 0 + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                            btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";

                                        }
                                        Messages.Add("{ \"msg\":\"Please Select Menu from Below  <br>" + btndata + "</div>\"}");
                                    }
                                    else
                                    {
                                        foreach (Menu m in data)
                                        {
                                            btndata += "<button onclick =ClickedMenuMenu(" + m.Id + "," + 0 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuName + "</button>";

                                        }
                                        Messages.Add("{ \"msg\":\"দয়াকরে নিচে থেকে নির্বাচন করুন  <br>" + btndata + "</div>\"}");
                                    }

                                    // Messages.Add("{ \"msg\":\"" + temp.answer + ".<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                else
                                {
                                    if (tempy.answer.Contains("?"))
                                    {
                                        Messages.Add("{ \"msg\":\"" + tempy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempy.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                // Messages.Add("{ \"msg\":\"Have you applied for police clearance?<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><br><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                //string response = "{ \"msg\" : \"Sorry,I don't have answer for this question now.I have noted it for future response.\"}";
                                //Messages.Add(response);
                                var lastgret = await iquestionCategoryService.GetLastGrettingsleastone();
                                string response = "{ \"msg\" : \"" + lastgret.NameEn + "\"}";
                                Messages.Add(response);
                                Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                            }
                        }
                        else
                        {

                            KeyWordQuesAns tempyy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedIDcatid((int)temp.questionCategoryId, temp.Id, message);
                            if (tempyy != null)
                            {
                                if (message.ToLower() == "no")
                                {
                                    Messages.Add("{ \"msg\":\" Did you mean," + tempyy.question + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                else if (message.ToLower() == "না")
                                {
                                    Messages.Add("{ \"msg\":\" Did you mean," + tempyy.question + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                else
                                {
                                    if (tempyy.answer.Contains("Have you applied for police clearance") || tempyy.answer.Contains("আপনি কি পুলিশ ক্লিয়ারেন্স এর জন্য আবেদন করেছেন?") || tempyy.answer == "?")
                                    {

                                        Messages.Add("{ \"msg\":\"" + tempyy.answer + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0,-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0,0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                    }
                                    else
                                    {

                                        if (tempyy.answer.Contains("?"))
                                        {
                                            Messages.Add("{ \"msg\":\"" + tempyy.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + tempyy.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");

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
                                    string response = "{ \"msg\" : \""+ lastgret.NameEn+ "\"}";
                                    Messages.Add(response);
                                    Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                    Messages.Add("{ \"msg\":\"" + "আপনি কি পুলিশ ক্লিয়ারেন্সের জন্য আবেদন করেছেন?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
                                }
                                //else
                                //{
                                //    Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                                Messages.Add("{ \"msg\":\"" + temp.answer + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");

                            }
                            else
                            {
                                Messages.Add("{ \"msg\":\" Did you mean," + temp.question + "<br><br><button onclick ='ClickedMenuMenuK(" + temp.Id + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 1 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button  id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 0 + ")' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick ='ClickedMenuMenuK(" + temp.Id + "," + 2 + ")'  style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");
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
                            Messages.Add("{ \"msg\":\"" + "Have you applied for police clearance?" + "<br><br><button onclick='ClickedMenuMenuQQ(" + 2 + "," + -1 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>Yes</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(-1)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>No</button><button onclick='ClickedMenuMenuQQ(" + 1 + "," + 0 + ")' id='btnbangla' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>হ্যাঁ</button> &nbsp;&nbsp;&nbsp; <button id='btnbangla' onclick='ClickedMenuMenu(0)' style='border: 1px solid whitesmoke; border-radius: 40px; background: #0084FF; height: 30px; width: 70px; color: white; cursor: pointer;'>না</button>\"}");

                        }

                    }


                }
            }

            return Messages;
        }
        public async Task<List<string>> QuesReplayServiceKK(string senderId, string pageId, string message, string postBack)
        {
            string combinedId = pageId + senderId;
            List<string> Messages = new List<string>();
            int id = await facebookService.GetFacebookpageId(pageId);

            if (postBack == "")
            {
                KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(id, message, combinedId, senderId);

                if (temp.question != null && temp.question != "")
                {

                    string tm1 = new String(message.Where(Char.IsLetter).ToArray());
                    string tm2 = new String(temp.question.Where(Char.IsLetter).ToArray());
                    if (tm1.ToLower() == tm2.ToLower()) // Exact Match
                    {
                        Messages.Add("{ \"msg\":\"" + temp.answer + ".\"}");
                        if (temp.more != "" && temp.more != null)
                            Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");
                    }
                    else
                    {
                        Messages.Add("{ \"msg\":\" Did you mean? (yes or no)\"}");
                        string response = "{ \"msg\":\"" + temp.question + "\",\"postback\":\"KWA;" + temp.Id + "\"}";
                        Messages.Add(response);
                    }
                }
                else
                {
                    string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                    Messages.Add(response);
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
                        KeyWordQuesAns tempy = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);

                        if (tempy.question != null && tempy.question != "")
                        {
                            Messages.Add("{ \"msg\":\"" + temp.answer + ".\"}");
                            if (temp.more != "" && temp.more != null)
                                Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");

                            //Messages.Add("{ \"msg\":\" Did you mean? (yes or no)\"}");
                            string response = "{ \"msg\":\"" + tempy.question + "\",\"postback\":\"KWA;" + tempy.Id + "\"}";
                            Messages.Add(response);
                        }
                        else
                        {
                            string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                            Messages.Add(response);
                        }
                    }
                    else
                    {
                        Messages.Add("{ \"msg\":\"" + temp.answer + ".\"}");
                        if (temp.more != "" && temp.more != null)
                            Messages.Add("{ \"msg\": \"More : " + temp.more + "\"}");
                    }

                }
                else
                {


                    KeyWordQuesAns temp = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);

                    if (temp.question != null && temp.question != "")
                    {
                        Messages.Add("{ \"msg\":\" Did you mean? (yes or no)\"}");
                        string response = "{ \"msg\":\"" + temp.question + "\",\"postback\":\"KWA;" + temp.Id + "\"}";
                        Messages.Add(response);
                    }
                    else
                    {
                        string response = "{ \"msg\" : \"Sorry Cant Find any context\"}";
                        Messages.Add(response);
                    }
                }
            }

            return Messages;

        }










        public async Task<List<string>> CustomMessageGenerator(string senderId, string pageId, string message, string postback, string userId, string botKey)
        {
            message = message.Trim();


            int falsestep = 0;
            string combinedId = pageId + senderId;
            int FFbPageId = await facebookService.GetFacebookpageId(pageId);
            List<string> messages = new List<string>();
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);
            if (serviceFlow == null)
            {
                falsestep = 1;
                ServiceFlow data = new ServiceFlow
                {
                    Id = combinedId,
                    InfoType = "pasportOrref",
                    ServiceCode = "passport",
                    StepNo = 2,
                    DateTime = DateTime.Now,
                    Attempt = 0


                };
                serviceFlow = await serviceFlowService.SaveServiceFlow(data);
            }


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
                        messages = await QuesReplayService(senderId, pageId, message, postback, userId);
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
                            messages = await QuesReplayService(senderId, pageId, message, postback, userId);
                        }
                        else
                        {
                            messages = await QuesReplayService(senderId, pageId, message, postback, userId);
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

                                btndata += "<button onclick = ClickedMenuMenu(" + m.Id + "," + -1 + ") id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>" + m.MenuNameEN + "</button>";
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
                                    btndata += "<button onclick = 'ClickedComplain(" + passportInfo.Id + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Complain</button> <button onclick = 'ClickedSuggestion(" + passportInfo.Id + ")' id = 'btnbanglas' style='border:1px solid #006CFF; color: #006CFF; padding: 5px 10px 5px 10px; margin-top: 5px; margin-left: 3px; border-radius: 40px; cursor: pointer;'>Suggestion</button>";
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

            return messages;
        }


















        #region Global Helpper
        private void CloseService(string combinedId)
        {
            serviceFlowService.CLearServiceData(combinedId);
            botFlowService.UpdateFlow(combinedId, "default");
        }
        #endregion

    }
}
