using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Response;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar
{
    public class ServiceManager
    {
        #region Interested Data
        private readonly string pageId;
        private readonly string messgae;
        private readonly string senderId;
        private readonly string QRPayload;
        private readonly string combinedId;
        private readonly string Lang;
        private readonly IServiceFlowService serviceFlowService;
        private readonly IBotFlowService botFlowService;
        private readonly UserLanguge MyLanguage;
        private readonly IUserInfoService userInfoService;
        private readonly IOTPService oTPService;
        private readonly IPassportInfoService passportInfoService;
        private readonly IBankInfoService bankInfoService;
        #endregion

        public ServiceManager(string pageId, string messgae, string senderId, string QRPayload, string combinedId, string lang, string rootpath, IServiceFlowService serviceFlowService, IBotFlowService botFlowService, IUserInfoService userInfoService, IOTPService oTPService, IPassportInfoService passportInfoService, IBankInfoService bankInfoService)
        {
            this.pageId = pageId;
            this.messgae = messgae;
            this.senderId = senderId;
            this.combinedId = combinedId;
            this.Lang = lang;
            this.serviceFlowService = serviceFlowService;
            this.botFlowService = botFlowService;
            this.userInfoService = userInfoService;
            this.oTPService = oTPService;
            this.passportInfoService = passportInfoService;
            this.bankInfoService = bankInfoService;
            if (Lang == "ENG")
            {
                using (StreamReader r = new StreamReader(rootpath + "/wwwroot/Lang/English.json"))
                {
                    string json = r.ReadToEnd();
                    MyLanguage = JsonConvert.DeserializeObject<UserLanguge>(json);
                }
            }
            else
            {
                using (StreamReader r = new StreamReader(rootpath + "/wwwroot/Lang/Bangla.json"))
                {
                    string json = r.ReadToEnd();
                    MyLanguage = JsonConvert.DeserializeObject<UserLanguge>(json);
                }
            }
        }

        public async Task<List<string>> GenerateResponse()
        {
            List<string> Messages = new List<string>();
            ServiceFlow serviceFlow = serviceFlowService.CurrentServiceState(combinedId);

            if (!InfoValidation.VlidateInformation(messgae, serviceFlow.InfoType))
            {
                serviceFlowService.increaseAttempt(combinedId);
                string response = "{ text:\"Canot get this, please input currectly. \"}";
                Messages.Add(response);
            }
            else if (serviceFlow.ServiceCode == "blnsbank")
            {
                Messages.AddRange(await this.BankDService(serviceFlow));
            }
            else if (serviceFlow.ServiceCode == "booking")
            {
                Messages.AddRange(await this.BookingService(serviceFlow));
            }
            else if (serviceFlow.ServiceCode == "passport")
            {
                Messages.AddRange(await this.VisaService(serviceFlow));
            }
            else if (serviceFlow.ServiceCode == "remittence")
            {
                Messages.AddRange(await this.RemittenceService(serviceFlow));
            }
            return Messages;

        }

        #region custoom services flows  
        private async Task<List<string>> BankDService(ServiceFlow serviceFlow)
        {
            UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
            List<string> responses = new List<string>();

            if (serviceFlow.StepNo == 0)
            {
                responses.Add("{ text:\"Please enter your AC number.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 1);
            }
            else if (serviceFlow.StepNo == 1)
            {
                if (!await bankInfoService.CheckBankAccount(messgae))
                {
                    responses.Add("{ text:\"Invalid AC Number \"}");
                    responses.AddRange(this.CloseService(serviceFlow)); //Closing service
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "bankac", messgae);
                    responses.Add("{ text:\"For Security purpose please the below informations\"}");
                    responses.Add("{ text:\"Enter your Date Of Birth(DD-MM-YYYY)\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                }
            }
            else if (serviceFlow.StepNo == 2)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);

                if (messgae == bankAccountDetails.birthdate)
                {
                    responses.Add("{ text:\"Enter your mothers name.\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 3);
                }
                else
                {
                    responses.Add("{ text:\"Enter your valid Date Of Birth(DD-MM-YYYY)\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                }
            }
            else if (serviceFlow.StepNo == 3)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);

                if (messgae != bankAccountDetails.mothersName)
                {
                    responses.Add("{ text:\"Invalid Informations\"}");
                    responses.AddRange(this.CloseService(serviceFlow)); //Closing service
                }
                else
                {
                    if (await oTPService.GenerateAndSendOTP(bankAccountDetails.mobile, senderId, pageId))
                    {
                        responses.Add("{ text:\"OTP has been sent to your mobile.\"}");
                        responses.Add("{ text:\"Enter the OTP here.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);
                    }
                    else
                    {
                        responses.Add("{ text:\"Something Went Wrong. Please do again.\"}");
                        responses.Add("{ text:\"Enter your mothers name.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 3);
                    }
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);
                if (!await oTPService.VerifyOTP(senderId, pageId, messgae))
                {
                    responses.Add("{ text:\"Invalid OTP.\"}");
                    responses.Add("{ text:\"Please input valid OTP\"}");
                }
                else
                {
                    responses.Add("{ text:\"Your current balance is : " + bankAccountDetails.accountBlns.ToString("0.00") + "\"}");
                    //serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                    responses.AddRange(this.CloseService(serviceFlow));//Closing service
                }

            }
            else
            {
                responses.Add("{ text:\"Thanks For Testing \"}");
                responses.Add("{ text:\"More Actions are Comming.. According to Jafar Sir.\"}");

                responses.AddRange(this.CloseService(serviceFlow));//Closing service
            }
            return responses;
        }


        private async Task<List<string>> RemittenceService(ServiceFlow serviceFlow)
        {
            UserInfo userInfo = userInfoService.GetuserInfo(combinedId);
            List<string> responses = new List<string>();

            if (serviceFlow.StepNo == 0)
            {
                responses.Add("{ text:\"Please enter your AC number.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 1);
            }
            else if (serviceFlow.StepNo == 1)
            {
                if (!await bankInfoService.CheckBankAccount(messgae))
                {
                    responses.Add("{ text:\"Invalid AC Number \"}");
                    responses.AddRange(this.CloseService(serviceFlow)); //Closing service
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "bankac", messgae);
                    responses.Add("{ text:\"For Security purpose please the below informations\"}");
                    responses.Add("{ text:\"Enter your Date Of Birth(DD-MM-YYYY)\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                }
            }
            else if (serviceFlow.StepNo == 2)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);

                if (messgae == bankAccountDetails.birthdate)
                {
                    responses.Add("{ text:\"Enter your mothers name.\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 3);
                }
                else
                {
                    responses.Add("{ text:\"Enter your valid Date Of Birth(DD-MM-YYYY)\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                }
            }
            else if (serviceFlow.StepNo == 3)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);

                if (messgae != bankAccountDetails.mothersName)
                {
                    responses.Add("{ text:\"Invalid Informations\"}");
                    responses.AddRange(this.CloseService(serviceFlow)); //Closing service
                }
                else
                {
                    if (await oTPService.GenerateAndSendOTP(bankAccountDetails.mobile, senderId, pageId))
                    {
                        responses.Add("{ text:\"OTP has been sent to your mobile.\"}");
                        responses.Add("{ text:\"Enter the OTP here.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);
                    }
                    else
                    {
                        responses.Add("{ text:\"Something Went Wrong. Please do again.\"}");
                        responses.Add("{ text:\"Enter your mothers name.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 3);
                    }
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                BankAccountDetails bankAccountDetails = await bankInfoService.GetBankInformationByAccount(userInfo.bankaccountNumber);
                if (!await oTPService.VerifyOTP(senderId, pageId, messgae))
                {
                    responses.Add("{ text:\"Invalid OTP.\"}");
                    responses.Add("{ text:\"Please input valid OTP\"}");
                }
                else
                {
                    responses.Add("{ text:\"Enter your transiction number\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                }
            }
            else if (serviceFlow.StepNo == 5)
            {
                Remittance remittance = await bankInfoService.GetRemittanceInfoByAcNumber(userInfo.bankaccountNumber);

                if (remittance != null)
                {
                    responses.Add("{ text:\"Transiction "+remittance.refNumber+", Status is : " + remittance.status + "\"}");
                    responses.Add("{ text:\"Thanks for using TOTA\"}");
                    responses.AddRange(this.CloseService(serviceFlow));//Closing service
                }
                else
                {
                    responses.Add("{ text:\"In valid reference number enter it carefully.\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                }

            }
            else
            {
                responses.Add("{ text:\"Thanks for taking our service\"}");
                responses.AddRange(this.CloseService(serviceFlow));//Closing service
            }
            return responses;
        }

        private async Task<List<string>> BookingService(ServiceFlow serviceFlow)
        {
            List<string> responses = new List<string>();

            if (serviceFlow.StepNo == 0)
            {
                responses.Add("{ text:\"If you help me I can find you a hotel.\"}");
                responses.Add("{ text:\"Do you agree? Yes/No.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 1);
            }
            else if (serviceFlow.StepNo == 1)
            {
                if (!InfoValidation.CheckConfirmation(messgae))
                {
                    responses.AddRange(this.CloseService(serviceFlow));//Closing service
                }
                else
                {
                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                    if (userInfo.FullName != null && userInfo.FullName != "")
                    {
                        responses.Add("{ text:\" Sir, Are you " + userInfo.FullName + " ?\"}");
                        responses.Add("{ text:\" Yes/No ?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 3);
                    }
                    else
                    {
                        responses.Add("{ text:\"Sir, Your Full Name?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                    }
                }
            }
            else if (serviceFlow.StepNo == 2)
            {
                userInfoService.UpdateUserInfo(combinedId, "FullName", messgae);
                UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                if (userInfo.Email != null && userInfo.Email != "")
                {
                    responses.Add("{ text:\"" + userInfo.FullName + ",\"}");
                    responses.Add("{ text:\"Sir, is your Email: " + userInfo.Email + "?\"}");
                    responses.Add("{ text:\" Yes/No ?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 5);
                }
                else
                {
                    responses.Add("{ text:\" Sir, what is your email for future communication?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "email", 4);
                }
            }
            else if (serviceFlow.StepNo == 3)
            {
                if (!InfoValidation.CheckConfirmation(messgae))
                {
                    userInfoService.UpdateUserInfo(combinedId, "FullName", "");
                    responses.Add("{ text:\"Sir, Your Full Name?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 2);
                }
                else
                {
                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                    if (userInfo.Email != null && userInfo.Email != "")
                    {
                        responses.Add("{ text:\"" + userInfo.FullName + ",\"}");
                        responses.Add("{ text:\"Sir, is your Email: " + userInfo.Email + "?\"}");
                        responses.Add("{ text:\" Yes/No ?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 5);
                    }
                    else
                    {
                        responses.Add("{ text:\" Sir, what is your email for future communication?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "email", 4);
                    }
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                userInfoService.UpdateUserInfo(combinedId, "Email", messgae);

                UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                if (userInfo.Mobile != null && userInfo.Mobile != "")
                {
                    responses.Add("{ text:\"" + userInfo.FullName + ",\"}");
                    responses.Add("{ text:\"Sir, is your Mobile: " + userInfo.Mobile + "?\"}");
                    responses.Add("{ text:\" Yes/No ?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 7);
                }
                else
                {
                    responses.Add("{ text:\" Sir, what is your mobile for future communication?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "email", 6);
                }
            }
            else if (serviceFlow.StepNo == 5)
            {
                if (!InfoValidation.CheckConfirmation(messgae))
                {
                    userInfoService.UpdateUserInfo(combinedId, "Email", "");
                    responses.Add("{ text:\" Sir, what is your email for future communication?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "email", 4);
                }
                else
                {
                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                    if (userInfo.Mobile != null && userInfo.Mobile != "")
                    {
                        responses.Add("{ text:\"" + userInfo.FullName + ",\"}");
                        responses.Add("{ text:\"Sir, is your Mobile: " + userInfo.Mobile + "?\"}");
                        responses.Add("{ text:\" Yes/No ?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 7);
                    }
                    else
                    {
                        responses.Add("{ text:\" Sir, what is your mobile for future communication?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 6);
                    }
                }
            }
            else if (serviceFlow.StepNo == 6)
            {
                userInfoService.UpdateUserInfo(combinedId, "Mobile", messgae);

                UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                responses.Add("{ text:\"Checkin Date?\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "date", 8);
            }
            else if (serviceFlow.StepNo == 7)
            {
                if (!InfoValidation.CheckConfirmation(messgae))
                {
                    userInfoService.UpdateUserInfo(combinedId, "Mobile", "");
                    responses.Add("{ text:\" Sir, what is your mobile for future communication?\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 6);
                }
                else
                {
                    UserInfo userInfo = userInfoService.GetuserInfo(combinedId);

                    responses.Add("{ text:\"Checkin Date? (Ex 23/02)\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "date", 8);
                }
            }
            else if (serviceFlow.StepNo == 8)
            {
                responses.Add("{ text:\"CheckOut Date? (Ex 25/02)\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "date", 9);
            }
            else if (serviceFlow.StepNo == 9)
            {
                responses.Add("{ text:\"How many Rooms: ?\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "integer", 10);
            }
            else if (serviceFlow.StepNo == 10)
            {
                responses.Add("{ text:\"Our aget communicate you very soon, for confirmation.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 11);
            }
            else
            {
                responses.Add("{ text:\"Thanks For Testing Booking DEMO \"}");
                responses.Add("{ text:\"More Actions are Comming.. According to Jafar Sir(MD Opus Technology Ltd).\"}");

                responses.AddRange(this.CloseService(serviceFlow));//Closing service
            }
            return responses;
        }

        //Design For Police Head Quater
        private async Task<List<string>> VisaService(ServiceFlow serviceFlow)
        {
            List<string> responses = new List<string>();

            if (serviceFlow.StepNo == 0)
            {
                responses.Add("{ text:\"If you help me I can tell you about visa applicatio status.\"}");
                responses.Add("{ text:\"Do you agree? Yes/No.\"}");
                serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "confirmation", 1);
            }
            else if (serviceFlow.StepNo == 1)
            {
                if (!InfoValidation.CheckConfirmation(messgae))
                {
                    responses.AddRange(this.CloseService(serviceFlow));//Closing service
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "Passport", "");
                    responses.Add("{ text:\"Please enter your passport number or Application Referance number.\"}");
                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "pasportOrref", 2);
                }
            }
            else if (serviceFlow.StepNo == 2)
            {
                if (InfoValidation.CheckPassportOrRef(messgae) == "unknown")
                {
                    responses.Add("{ text:\"Invalid!! :( passport or Application Referance number.\"}");
                    responses.Add("{ text:\"Send me valid passport or Application Referance number.\"}");
                }
                else
                {
                    userInfoService.UpdateUserInfo(combinedId, "Passport", messgae);
                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(messgae);
                    if (passportInfo == null)
                    {
                        responses.Add("{ text:\"Invalid!! :( passport or Application Referance number.\"}");
                        responses.Add("{ text:\"Send me valid passport or Application Referance number.\"}");
                    }
                    else
                    {
                        if (passportInfo.gender.ToLower() == "male")
                            responses.Add("{ text:\"Hello,  Mr." + passportInfo.name + ".\"}");
                        else
                            responses.Add("{ text:\"Hello,  Ms." + passportInfo.name + ".\"}");
                        responses.Add("{ text:\"Send me valid mobile number according to your visa application.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                    }

                }
            }
            else if (serviceFlow.StepNo == 3)
            {
                var data = userInfoService.GetuserInfo(combinedId);
                PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(data.passport);
                if (passportInfo?.mobile != messgae)
                {
                    responses.Add("{ text:\" Your Mobile Number Dosen't Match.\"}");
                    responses.Add("{ text:\"Send me valid mobile number according to your visa application.\"}");
                }
                else
                {
                    if (await oTPService.GenerateAndSendOTP(messgae, senderId, pageId))
                    {
                        responses.Add("{ text:\"Sir, An OTP Send to your mobile.\"}");
                        responses.Add("{ text:\"Give me that.\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 4);
                    }
                    else
                    {
                        responses.Add("{ text:\"Sir, Something Went Wrong.\"}");
                        responses.Add("{ text:\"What is your mobile number According to bank?\"}");
                        serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "mobile", 3);
                    }
                }
            }
            else if (serviceFlow.StepNo == 4)
            {
                if (!await oTPService.VerifyOTP(senderId, pageId, messgae))
                {
                    responses.Add("{ text:\"Invalid OTP.\"}");
                    responses.Add("{ text:\"Please input valid OTP\"}");
                }
                else
                {
                    var data = userInfoService.GetuserInfo(combinedId);
                    PassportInfo passportInfo = await passportInfoService.GetPassportInfoByPasspoertIds(data.passport);
                    string status = passportInfo.status;
                    responses.Add("{ text:\"Sir your application with ref no: " + passportInfo.refNo + ".\"}");
                    responses.Add("{ text:\"Sir your application with passport no: " + passportInfo.passportNo + ".\"}");

                    responses.Add("{ text:\"Current Status: " + passportInfo.status + ".\"}");
                    responses.Add("{ text:\"" + passportInfo.message + ".\"}");

                    serviceFlowService.UpdateNextStep(combinedId, serviceFlow.ServiceCode, "text", 5);
                    responses.Add("{ text:\"Thanks For taking our service. :) \"}");
                    responses.AddRange(this.CloseService(serviceFlow));//Closing service
                }

            }
            else
            {
                responses.Add("{ text:\"Thanks a lot sir.\"}");
                responses.AddRange(this.CloseService(serviceFlow));//Closing service
            }
            return responses;
        }
        #endregion

        #region Global Helpper
        private List<string> CloseService(ServiceFlow serviceFlow)
        {
            serviceFlowService.CLearServiceData(combinedId);
            botFlowService.UpdateFlow(combinedId, "default");

            List<string> responses = new List<string>();
            List<quick_replies> quick_replies = new List<quick_replies>();

            quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));

            string response = "{ text:\"" + MyLanguage.selectFromBelow + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
            responses.Add(response);

            return responses;
        }
        #endregion
    }
}
