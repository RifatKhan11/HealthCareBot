using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class SMSMailService : ISMSMailService
    {
        private readonly SmtpClient smtpClient;
        private readonly IConfiguration _configuration;
        public SMSMailService(IConfiguration _configuration)
        {
            this._configuration = _configuration;
            smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("opustech2k23@gmail.com", "OpusTech111"),
                EnableSsl = true
            };
        }

        public async Task SendEmailViaAppPassBulk(string[] mailTo, string name, string subject, string message)
        {
            try
            {
                string userName = _configuration["Email:Email"];
                string password = _configuration["Email:Password"];
                string host = _configuration["Email:Host"];
                int port = int.Parse(_configuration["Email:Port"]);
                string mailFrom = _configuration["Email:Email"];
                
                
                var i = 0;
                foreach (var mails in mailTo)
                {

                    var emailMessage = new MailMessage();
                    emailMessage.To.Add(new MailAddress(mailTo[i]));
                    emailMessage.From = new MailAddress(mailFrom, name);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;

                    string[] mailParts = mailTo[i].Split('@');
                    if (mailParts[1] == "opus-bd.com")
                    {
                        SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                        smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                        smtp.Port = 25;
                        smtp.EnableSsl = true;
                        smtp.Send(emailMessage);
                    }
                    else
                    {
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(emailMessage);
                    }

                    

                    i++;
                }
                
                //var credential = new NetworkCredential
                //{
                //    UserName = userName,
                //    Password = password
                //};

                
                //SmtpClient SmtpServer = new SmtpClient(host, port);
                ////SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                ////SmtpServer.Timeout = 80000000;
                //SmtpServer.EnableSsl = true;
                ////SmtpServer.UseDefaultCredentials = false;
                //SmtpServer.Credentials = new NetworkCredential(mailFrom, password);
                //SmtpServer.Send(emailMessage);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public async Task SendEmailViaActivity(string[] mailTo, string subject, string message)
        {
            try
            {
                string userName = _configuration["Email:Email"];
                string password = _configuration["Email:Password"];
                string host = _configuration["Email:Host"];
                int port = int.Parse(_configuration["Email:Port"]);
                string mailFrom = _configuration["Email:Email"];


                var i = 0;
                foreach (var mails in mailTo)
                {

                    var emailMessage = new MailMessage();
                    emailMessage.From = new MailAddress(mailFrom, "OPUS TECHNOLOGY LIMITED");
                    emailMessage.To.Add(new MailAddress(mailTo[i]));
              
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;

                    string[] mailParts = mailTo[i].Split('@');
                    if (mailParts[1] == "opus-bd.com")
                    {
                        SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                        smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                        smtp.Port = 25;
                        smtp.EnableSsl = true;
                        smtp.Send(emailMessage);
                    }
                    else
                    {
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(emailMessage);
                    }



                    i++;
                }

              
         
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task SendEmailWithMeetingAttatchments(string[] to, string subject, string message,string pdfUrl)
        {
            string userName = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];

            List<Attachment> attach = new List<Attachment>();

            attach.Add(new Attachment(pdfUrl));


            var i = 0;
            foreach (var mails in to)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailFrom, "OPUS TECHNOLOGY LIMITED");
                msg.To.Add(new MailAddress(to[i]));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                foreach (var item in attach)
                {
                    msg.Attachments.Add(item);
                }
                string[] mailParts = to[i].Split('@');
                if (mailParts[1] == "opus-bd.com")
                {
                    SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                    smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
                else
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }










                i++;
            }



            await Task.CompletedTask;
        }
        public async Task SendEmailWithAttatchmentsActivity(string[] to, string subject, string pdfUrl, string message)
        {
            string userName = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];

            List<Attachment> attach = new List<Attachment>();

            attach.Add(new Attachment(pdfUrl));


            var i = 0;
            foreach (var mails in to)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailFrom, "OPUS TECHNOLOGY LIMITED");
                msg.To.Add(new MailAddress(to[i]));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                foreach (var item in attach)
                {
                    msg.Attachments.Add(item);
                }
                string[] mailParts = to[i].Split('@');
                if (mailParts[1] == "opus-bd.com")
                {
                    SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                    smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
                else
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }










                i++;
            }

           

            await Task.CompletedTask;
        }
        public async Task SendEmailViaAppPass(string mailTo, string name, string subject, string message)
        {
            try
            {
                string userName = _configuration["Email:Email"];
                string password = _configuration["Email:Password"];
                string host = _configuration["Email:Host"];
                int port = int.Parse(_configuration["Email:Port"]);
                string mailFrom = _configuration["Email:Email"];



                var emailMessage = new MailMessage();
                emailMessage.To.Add(new MailAddress(mailTo));
                emailMessage.From = new MailAddress(mailFrom, name);
                emailMessage.Subject = subject;
                emailMessage.Body = message;
                emailMessage.IsBodyHtml = true;


                string[] mailParts = mailTo.Split('@');
                if (mailParts[1] == "opus-bd.com")
                {
                    SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                    smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Send(emailMessage);
                }
                else
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(emailMessage);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task SendEmailWithFromMail(string mailTo, string name, string subject, string message)
        {
            bool active = Convert.ToBoolean(_configuration["Email:Enabled"]);
            if (active)
            {

                string userName = _configuration["Email:Email"];
                string password = _configuration["Email:Password"];
                string host = _configuration["Email:Host"];
                int port = int.Parse(_configuration["Email:Port"]);
                string mailFrom = _configuration["Email:Email"];
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = userName,
                        Password = password
                    };

                    client.Credentials = credential;
                    client.Host = host;
                    client.Port = port;
                    client.EnableSsl = true;

                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.To.Add(new MailAddress(mailTo));
                        emailMessage.From = new MailAddress(mailFrom, name);
                        emailMessage.Subject = subject;
                        emailMessage.Body = message;
                        emailMessage.IsBodyHtml = true;
                        client.Send(emailMessage);
                    }
                }
            }



            await Task.CompletedTask;
        }


        public async Task SendEmailWithAttatchmentsBulk(string[] to, string name, string subject/*, string path*/, string pdfUrl, string message)
        {
            string userName = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];
            
            List<Attachment> attach = new List<Attachment>();

            attach.Add(new Attachment(pdfUrl));
            

            var i = 0;
            foreach (var mails in to)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailFrom, name);
                msg.To.Add(new MailAddress(to[i]));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                foreach (var item in attach)
                {
                    msg.Attachments.Add(item);
                }




                string[] mailParts = to[i].Split('@');
                if (mailParts[1] == "opus-bd.com")
                {
                    SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                    smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
                else
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }




                i++;
            }
            
            await Task.CompletedTask;
        }
        public async Task SendEmailWithAttatchments(string[] to, string subject/*, string path*/, string pdfUrl, string message, string name)
        {
            string userName = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];
            
            List<Attachment> attach = new List<Attachment>();

            attach.Add(new Attachment(pdfUrl));


            var i = 0;
            foreach (var mails in to)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailFrom, name);
                msg.To.Add(new MailAddress(to[i]));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                foreach (var item in attach)
                {
                    msg.Attachments.Add(item);
                }
                string[] mailParts = to[i].Split('@');
                if (mailParts[1] == "opus-bd.com")
                {
                    SmtpClient smtp = new SmtpClient("mail.opus-bd.com");
                    smtp.Credentials = new System.Net.NetworkCredential("no-reply@opus-bd.com", "Opus@#123$");

                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
                else
                {
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Credentials = new System.Net.NetworkCredential("opustech2k23@gmail.com", "vurk oich ugwg qxmk");

                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(msg);
                }
                i++;
            }

            //SmtpClient SmtpServer = new SmtpClient(host, port);
            //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            //SmtpServer.Timeout = 50000000;
            //SmtpServer.EnableSsl = true;
            //SmtpServer.UseDefaultCredentials = false;
            //SmtpServer.Credentials = new NetworkCredential(mailFrom, password);


            //try
            //{
            //    SmtpServer.Send(msg);

            //}
            //catch (System.Exception ex)
            //{

            //    throw;
            //}

            await Task.CompletedTask;
        }
        public string SendEMAIL(string mail, string subject, string message)
        {
            try
            {
                smtpClient.Send("opustech2k23@gmail.com", mail, subject, message);
                return "Sent";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string SendEMAIL(string password, string toMail, string ccMail, string bccMail, string subject, string body, string path, bool isHtml)
        {
            try
            {
                string fromMail = "opustech2k23@gmail.com";
                MailMessage msg = new MailMessage();

                msg.From = new MailAddress(fromMail);
                msg.To.Add(toMail);
                if (ccMail.Length > 0)
                {
                    msg.CC.Add(ccMail);
                }
                if (bccMail.Length > 0)
                {
                    msg.Bcc.Add(bccMail);
                }
                msg.BodyEncoding = Encoding.UTF8; ;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.Subject = subject;
                msg.Body = body;
                if (isHtml == true)
                {
                    msg.IsBodyHtml = true;
                }
                if (Directory.Exists(path))
                {
                    msg.Attachments.Add(new Attachment(path));
                }


                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                NetworkCredential credential = new NetworkCredential();
                credential.UserName = fromMail;
                credential.Password = password;
                smtp.Credentials = credential;
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(msg);
                return "Sent";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string SendEMAILWithAttatchment(string toMail, string subject, string body, string path, bool isHtml)
        {
            try
            {
                string fromMail = "opustestmail@gmail.com";
                string password = "Opus1234";





                Attachment attach = new Attachment(path);
                MailMessage msg = new MailMessage();

                msg.From = new MailAddress(fromMail);
                msg.To.Add(toMail);
                msg.BodyEncoding = Encoding.UTF8; ;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.Subject = subject;
                msg.Body = body;
                msg.Attachments.Add(attach);

                if (isHtml == true)
                {
                    msg.IsBodyHtml = true;
                }


                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                NetworkCredential credential = new NetworkCredential();
                credential.UserName = fromMail;
                credential.Password = password;
                smtp.Credentials = credential;
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(msg);
                return "Sent";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> SendSMSAsync(string mobile, string message)
        {
            // return "Skip";
            try
            {
                if (mobile.Length < 11)
                {
                    int numOfZeros = 11 - mobile.Length;
                    mobile = new string('0', numOfZeros) + mobile;
                }
                //string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&source=PHQ%20BD&destination={0}&message={1}", mobile, message);
                //string url = String.Format("http://api.boom-cast.com/boomcast/WebFramework/boomCastWebService/externalApiSendTextMessage.php?masking=NOMASK&userName=OpusTech&password=c3eb7e87b84e252777057a07d984e98e&MsgType=TEXT&receiver={0}&message={1}", mobile, message);

                string url = String.Format("http://apibd.rmlconnect.net/bulksms/personalizedbulksms?username=OpusBDENT&password=hxIi6jyZ&destination=88{0}&source=8809617611359&message={1}", mobile, message);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                //dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode) return "success";
                return "fail";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string SendEmailWithFrom(string mailTo, string name, string subject, string message)
        {
            string userName = _configuration["Email:Email"];
            string password = _configuration["Email:Password"];
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = userName,
                    Password = password
                };

                client.Credentials = credential;
                client.Host = host;
                client.Port = port;
                client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(mailTo));
                    emailMessage.CC.Add(new MailAddress("engineersclubuttara@gmail.com"));
                    emailMessage.From = new MailAddress(mailFrom, name);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;
                    client.Send(emailMessage);
                }
            }
            //await Task.CompletedTask;
            return "Sent";
        }
        #region new email

        public async Task SendEmailViaNewPassword(string[] mailTo, string name,string subject, string message, string displayEmail,string displayPassword)
        {
            try
            {
               
                string host = _configuration["Email:Host"];
                int port = int.Parse(_configuration["Email:Port"]);
                var i = 0;
                foreach (var mails in mailTo)
                {

                    var emailMessage = new MailMessage();
                    emailMessage.To.Add(new MailAddress(mailTo[i]));
                    emailMessage.From = new MailAddress(displayEmail, name);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;

                    string[] mailParts = mailTo[i].Split('@');
                   
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        smtp.Credentials = new System.Net.NetworkCredential(displayEmail, displayPassword);

                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(emailMessage);
                   



                    i++;
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        public async Task NewSendEmailWithAttatchments(string[] to, string subject, string pdfUrl, string message, string name, string displayEmail, string displayPassword)
        {
            string host = _configuration["Email:Host"];
            int port = int.Parse(_configuration["Email:Port"]);
            string mailFrom = _configuration["Email:Email"];

            List<Attachment> attach = new List<Attachment>();

            attach.Add(new Attachment(pdfUrl));


            var i = 0;
            foreach (var mails in to)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(displayEmail, name);
                msg.To.Add(new MailAddress(to[i]));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                foreach (var item in attach)
                {
                    msg.Attachments.Add(item);
                }
                string[] mailParts = to[i].Split('@');
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Credentials = new System.Net.NetworkCredential(displayEmail, displayPassword);

                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(msg);
                i++;
            }

           

            await Task.CompletedTask;
        }

        #endregion



    }
}
