using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface ISMSMailService
    {
        Task NewSendEmailWithAttatchments(string[] to, string subject, string pdfUrl, string message, string name, string displayEmail, string displayPassword);
        Task SendEmailViaNewPassword(string[] mailTo, string name, string subject, string message, string displayEmail, string displayPassword);
        Task SendEmailWithMeetingAttatchments(string[] to, string subject, string message, string pdfUrl);
        Task SendEmailWithAttatchmentsActivity(string[] to, string subject, string pdfUrl, string message);
        Task SendEmailViaActivity(string[] mailTo, string subject, string message);
        Task SendEmailViaAppPass(string mailTo, string name, string subject, string message);
        Task SendEmailViaAppPassBulk(string[] mailTo, string name, string subject, string message);
        Task SendEmailWithFromMail(string mailTo, string name, string subject, string message);
        Task SendEmailWithAttatchments(string[] to, string subject, /*IEnumerable<string> path,*/ string pdfUrl, string message, string name);
        Task SendEmailWithAttatchmentsBulk(string[] to, string name, string subject/*, string path*/, string pdfUrl, string message);
        Task<string> SendSMSAsync(string mobile, string message);
        string SendEMAIL(string mail, string subject, string message);
        string SendEmailWithFrom(string mailTo, string name, string subject, string message);
        string SendEMAILWithAttatchment(string toMail, string subject, string body, string path, bool isHtml);
    }
}
