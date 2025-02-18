using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.IServices.IServices;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OPUSERP.SCM.SMSService
{
    public class SMSService: ISMSService
    {
        private readonly ApplicationDbContext _context;
        public SMSService(ApplicationDbContext context)
        {
            _context = context;
        }
        //hxIi6jyZ

        public async Task<string> SendSMSAsync(string mobile, string message,string bpNo)
        {
            // return "Skip";
            string reason = "";
            try
            {
                //string source = "PHQ BD";
                //string url = String.Format("http://api.boom-cast.com/boomcast/WebFramework/boomCastWebService/externalApiSendTextMessage.php?masking=NOMASK&userName=OpusTech&password=c3eb7e87b84e252777057a07d984e98e&MsgType=TEXT&receiver={0}&message={1}", mobile, message);
                //string url = String.Format("http://api.rmlconnect.net/bulksms/bulksms?username=OpusSGNMask&password=9VTVB8T8&type=0&dlr=1&destination={0}&source=8809612445509&message={1}", mobile, message);
                //Last Used 2023-03-19
                //string url = String.Format("http://api.rmlconnect.net/bulksms/bulksms?username=OpusSGNMask&password=9VTVB8T8&type=0&dlr=1&destination={0}&source=PHQ%20BD&message={1}", mobile, message);
                string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&destination={0}&source=8809617611359&message={1}", mobile, message);
                //string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&destination={0}&source=PHQ%20BD&message={1}", mobile, message);//ok
                //string url = String.Format("http://api.rmlconnect.net/sendsms?username=OpusSGNMask&password=9VTVB8T8&type=0&dlr=0&destination={0}&source={2}&message={1}", mobile, message, source);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var smsData = await response.Content.ReadAsStringAsync();
                reason = smsData.ToString();
                //string[] splData = smsData.Split("|");
                //dynamic data = JsonConvert.DeserializeObject(splData);

                //if (splData[0].ToString() == "1701") return "success";
                //MailLog log = new MailLog
                //{
                //    sender="PHQ BD",
                //    mailType="SMS",
                //    recipient=mobile,
                //    subject=message,
                //    sendTime=DateTime.Now,
                //    refNo=smsData,
                //    notSendReason=bpNo,
                //    isSuccess=1
                //};
                //_context.MailLogs.Add(log);
                //await _context.SaveChangesAsync();
                return smsData;
            }
            catch (Exception e)
            {
                //MailLog log = new MailLog
                //{
                //    sender = "PHQ BD",
                //    mailType = "SMS",
                //    recipient = mobile,
                //    subject = message,
                //    sendTime = DateTime.Now,
                //    refNo = reason,
                //    notSendReason = reason,
                //    isSuccess = 1
                //};
                //_context.MailLogs.Add(log);
                //await _context.SaveChangesAsync();
                return e.Message;
            }
        }

        //public async Task<bool> SaveMailLog(MailLog mailLog)
        //{
        //    if (mailLog.Id != 0)
        //        _context.MailLogs.Update(mailLog);
        //    else
        //        _context.MailLogs.Add(mailLog);

        //    return 1 == await _context.SaveChangesAsync();
        //}
        //public async Task<IEnumerable<MailLog>> GetMailLogs()
        //{
        //    var logs=await _context.MailLogs.OrderByDescending(x=>x.Id).ToListAsync();
        //    return logs;
        //}
    }
}
