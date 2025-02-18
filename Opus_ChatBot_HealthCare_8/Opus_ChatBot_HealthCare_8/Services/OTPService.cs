using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class OTPService : IOTPService
    {
        private readonly ApplicationDbContext _contex;
        private readonly IUserInfoService userInfoService;
        private readonly SmtpClient smtpClient;

        public OTPService(ApplicationDbContext contex, IUserInfoService userInfoService)
        {
            _contex = contex;
            this.userInfoService = userInfoService;
            smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("opustestmail@gmail.com", "Opus1234"),
                EnableSsl = true
            };
        }

        public async Task<bool> SendEMAIL(string mail, string subject, string message)
        {
            try
            {
                smtpClient.Send("jaggesher.mcc@gmail.com", mail, subject, message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> GenerateAndSendOTP(string UsersMobile, string UserId, string PageId)
        {
           // return await Task.FromResult(true);
            try
            {
                #region Production
                //string otp = new Random().Next(1000, 9999).ToString();
                //string url = String.Format("http://api.boom-cast.com/boomcast/WebFramework/boomCastWebService/externalApiSendTextMessage.php?masking=NOMASK&userName=OpusTech&password=c3eb7e87b84e252777057a07d984e98e&MsgType=TEXT&receiver={0}&message={1}", UsersMobile, $"Your Code is: {otp}");

                //HttpClient client = new HttpClient();
                //HttpResponseMessage response = await client.GetAsync(url);
                //response.EnsureSuccessStatusCode();

                //dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                //if (data[0].success == 1)
                //{
                //    userInfoService.UpdateUserInfo(PageId + UserId, "otp", otp);
                //    return await Task.FromResult(true);
                //}
                //return await Task.FromResult(false);
                #endregion

                #region SandBox
                userInfoService.UpdateUserInfo(PageId + UserId, "otp", "1234");
                return await Task.FromResult(true);
                #endregion

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> VerifyOTP(string UserId, string PageId, string otp)
        {
            var data = userInfoService.GetuserInfo(PageId + UserId);
            if (otp == data?.otpMsg) return await Task.FromResult(true);
            return await Task.FromResult(false);
        }
    }
}
