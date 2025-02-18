using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OPUSWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OPUSWebhookController(ApplicationDbContext context)
        {
            this._context = context;
        }


        [HttpPost("/api/webhooks/chatbot-whatsapp")]
        public async Task<IActionResult> HandleWebhookAsync()
        {
            try
            {
				string requestBody2 = await new StreamReader(Request.Body).ReadToEndAsync();

				LogWebhookPayload(requestBody2, "Report");

				//var key = "N3QRkQ4INv7ea4MUK6iNm9NHjKPJBprdnFWTlSCJ8EAdOB11g9uVKAQ2enPR64nbUA6BfZOfuoSASORM9YkFQFeLlc97XUC9L6Je";
    //            var apiKey = Request.Headers["x-api-key"].FirstOrDefault();
    //            var feedback = Request.Headers["EVERCAREBD"].FirstOrDefault();
    //            var evercareKey = Request.Headers["EVERCAREBD"].FirstOrDefault();
    //            if (string.IsNullOrEmpty(evercareKey) || evercareKey != key)
    //            {
    //                LogWebhookPayload("Invalid API key.");
    //                return BadRequest("Invalid API key.");
    //            }

    //            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

    //            LogWebhookPayload(evercareKey);

    //            if (requestBody.Length > 0)
    //            {
    //                LogWebhookPayload(requestBody);
    //            }

    //            if (feedback != null)
    //            {
    //                LogWebhookPayload(feedback);
    //            }
    //            LogWebhookPayload("Received webhook payload:");
    //            Console.WriteLine("Received webhook payload:");
    //            Console.WriteLine(requestBody);

                return Ok("Webhook received successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling webhook request: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }


		[HttpPost("/api/webhooks/chatbot-response")]
		public async Task<IActionResult> HandleWebhookResponseAsync()
		{
			try
			{
				string requestBody2 = await new StreamReader(Request.Body).ReadToEndAsync();

				LogWebhookPayload(requestBody2, "Response");

				//var key = "N3QRkQ4INv7ea4MUK6iNm9NHjKPJBprdnFWTlSCJ8EAdOB11g9uVKAQ2enPR64nbUA6BfZOfuoSASORM9YkFQFeLlc97XUC9L6Je";
				//            var apiKey = Request.Headers["x-api-key"].FirstOrDefault();
				//            var feedback = Request.Headers["EVERCAREBD"].FirstOrDefault();
				//            var evercareKey = Request.Headers["EVERCAREBD"].FirstOrDefault();
				//            if (string.IsNullOrEmpty(evercareKey) || evercareKey != key)
				//            {
				//                LogWebhookPayload("Invalid API key.");
				//                return BadRequest("Invalid API key.");
				//            }

				//            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

				//            LogWebhookPayload(evercareKey);

				//            if (requestBody.Length > 0)
				//            {
				//                LogWebhookPayload(requestBody);
				//            }

				//            if (feedback != null)
				//            {
				//                LogWebhookPayload(feedback);
				//            }
				//            LogWebhookPayload("Received webhook payload:");
				//            Console.WriteLine("Received webhook payload:");
				//            Console.WriteLine(requestBody);

				return Ok("Webhook received successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error handling webhook request: {ex.Message}");
				return StatusCode(500, "Internal server error.");
			}
		}


		private void LogWebhookPayload(string payload, string type)
        {
            try
            {
				// Deserialize JSON to dynamic object
				dynamic root = JsonConvert.DeserializeObject(payload);

				// Extracting values
				string from = root.results[0]?.from;
				string to = root.results[0]?.to;
				string integrationType = root.results[0]?.integrationType;
				string pairedMessageId = root.results[0]?.pairedMessageId;
				string groupName = root.results[0]?.status?.groupName;
				string messageId = root.results[0]?.messageId;
				string doneAt = root.results[0]?.doneAt;
				string sentAt = root.results[0]?.sentAt;
				string seenAt = root.results[0]?.seenAt;
				string channel = root.results[0]?.channel;
				string receivedAt = root.results[0]?.receivedAt;
				string message = root.results[0]?.message?.text;
				string contextFrom = root.results[0]?.message?.context?.from;
				string contextid = root.results[0]?.message?.context?.id;
				string messageType = root.results[0]?.message?.type;
				string senderName = root.results[0]?.contact?.name;
				//string messageText = root.results[0].message.text;
				//string messageType = root.results[0].message.type;
				//string contactName = root.results[0].contact.name;


				// Append the payload to the log file in the root directory
				//System.IO.File.AppendAllText(_webhookLogFile, payload + Environment.NewLine, Encoding.UTF8);

				if (string.IsNullOrEmpty(payload)) { return; }
                var data = new WebHookLog
                {
                    Id = 0,
                    type = type,
                    logTime = DateTime.Now,
                    requestBody = payload,
                    
                    integrationType = integrationType,
					messageType = messageType,
                    from = from,
                    to = to,
                    description = message?.Replace("EVERCAREBD", ""),
                    messageId = messageId,
                    pairedMessageId = pairedMessageId,
                    status = groupName,
                    sentAt = sentAt != null ? Convert.ToDateTime(sentAt) : DateTime.MinValue,
                    seenAt = seenAt != null ? Convert.ToDateTime(seenAt) : DateTime.MinValue,
                    name = senderName
				};

                _context.WebHookLogs.Add(data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error logging webhook payload: {ex.Message}");
            }
        }


        [HttpGet("/api/webhooks/chatbot-whatsapp/responses")]
        public async Task<IActionResult> GetWebHookResponses()
        {
            var data = await _context.WebHookLogs.OrderByDescending(x => x.logTime).AsNoTracking().ToListAsync();

            return Ok(data);
        }
    }
}
