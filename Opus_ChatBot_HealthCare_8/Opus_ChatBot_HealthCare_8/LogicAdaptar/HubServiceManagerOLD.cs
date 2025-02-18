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

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar
{
    public class HubServiceManagerOld : IHubServiceManagerOld
    {
        private readonly IServiceFlowService serviceFlowService;
        private readonly IBotFlowService botFlowService;
        private readonly IUserInfoService userInfoService;
        private readonly IOTPService oTPService;
        private readonly IPassportInfoService passportInfoService;
        private readonly IBankInfoService bankInfoService;
        private readonly IFacebookService facebookService;
        private readonly IKeyWordQuesService keyWordQuesService;

        public HubServiceManagerOld(IServiceFlowService serviceFlowService, IBotFlowService botFlowService, IUserInfoService userInfoService, IOTPService oTPService, IPassportInfoService passportInfoService, IBankInfoService bankInfoService, IFacebookService facebookService, IKeyWordQuesService keyWordQuesService)
        {
            this.serviceFlowService = serviceFlowService;
            this.botFlowService = botFlowService;
            this.userInfoService = userInfoService;
            this.oTPService = oTPService;
            this.passportInfoService = passportInfoService;
            this.bankInfoService = bankInfoService;
            this.facebookService = facebookService;
            this.keyWordQuesService = keyWordQuesService;
        }

        public async Task<List<string>> CustoomVisaService(string senderId, string pageId, string message)
        {
            string combinedId = pageId + senderId;

            List<string> messages = new List<string>();
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);

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
                messages.Add("{\"msg\" : \"What's your passport number or application reference number?\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);

            }
            else if (serviceFlow.StepNo == 2)
            {
                if (InfoValidation.CheckPassportOrRef(message) == "unknown")
                {
                    messages.Add("{ \"msg\" : \"Invalid!! :( passport or application reference number.<i style='color:yellow;' class='fas fa-sad-tear'></i>\"}");
                    messages.Add(" { \"msg\" : \"What's your passport number or application reference number?\"}");
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "Passport", message);
                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(message);
                    if (passportInfo == null)
                    {
                        messages.Add("{ \"msg\" : \"Invalid!! :( passport or application reference number.<i style='color:yellow;' class='fas fa-sad-tear'></i>\"}");
                        messages.Add("{ \"msg\" : \"What's your passport number or application reference number?\"}");
                    }
                    else
                    {
                        if (passportInfo.gender.ToLower() == "male")
                            messages.Add("{ \"msg\" : \"Hello, " + passportInfo.name + ".\"}");
                        else
                            messages.Add("{ \"msg\" : \"Hello, " + passportInfo.name + ".\"}");
                        messages.Add("{ \"msg\" : \"What's your date of birth (yyyy-mm-dd)?.\"}");
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
                    messages.Add("{ \"msg\" : \"Your date of birth dosen't Match.<i style='color:yellow;' class='fas fa-sad-tear'></i>\"}");
                    messages.Add("{ \"msg\" : \"What's your date of birth (yyyy-mm-dd)?.\"}");
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
                    messages.Add("{ \"msg\" : \"Congratulation! Sir, Your date of birth matched successfully.<i style='color:yellow;' class='fas fa-thumbs-up'></i>\"}");
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
                    messages.Add("{ \"msg\" : \"Invalid passport expire date.<i style='color:yellow;' class='fas fa-sad-tear'></i>\"}");
                    messages.Add("{ \"msg\" : \"When will your passport expire (yyyy-mm-dd)? \"}");
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
                    messages.Add("{ \"msg\" : \"Sir your application  reference no:<span style='font-weight: bold;'> " + passportInfo.refNo + "</span>.<br>Passport no:<span style='font-weight: bold;'> " + passportInfo.passportNo + "</span>.<br>Current Status:<span style='font-weight: bold;'> " + passportInfo.status + "</span>.<br>Expected Delivery Date:<span style='font-weight: bold;'> " + passportInfo?.expectedDeliveryDate + "</span>.<br>Thanks for taking our service.\"}");
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

        public async Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack)
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

        #region Global Helpper
        private void CloseService(string combinedId)
        {
            serviceFlowService.CLearServiceData(combinedId);
            botFlowService.UpdateFlow(combinedId, "default");
        }
        #endregion

    }
}
