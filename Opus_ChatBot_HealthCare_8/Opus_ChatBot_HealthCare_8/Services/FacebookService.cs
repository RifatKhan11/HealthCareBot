using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class FacebookService : IFacebookService
    {

        private readonly ApplicationDbContext _context;
        public FacebookService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<string> GetAccessToken(string PageId)
        {
            try
            {
                FacebookPage data = await _context.FacebookPages.FirstOrDefaultAsync(x => x.PageId == PageId);
                return data.PageAccessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "";

        }

        public async Task<string> GetAccessTokenById(int Id)
        {
            try
            {
                FacebookPage data = await _context.FacebookPages.FindAsync(Id);
                return data.PageAccessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }

        public async Task<int> GetFacebookpageId(string PageId)
        {
            try
            {
                FacebookPage data = await _context.FacebookPages.FirstOrDefaultAsync(x => x.PageId == PageId);
                return data.Id;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public async Task<string> GetUserName(string userId, string PageAccessToken)
        {
            try
            {
                string url = "https://graph.facebook.com/v2.6/" + userId + "?fields=name&access_token=" + PageAccessToken;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                dynamic data = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                return data.name;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }

        }

        public async Task SendMessageToFacebook(string userId, List<string> messages, string PageAccessToken)
        {

            try
            {
                string url = "https://graph.facebook.com/v2.6/me/messages?access_token=" + PageAccessToken;

                HttpClient client = new HttpClient();
                foreach (string message in messages)
                {

                    string sendData = string.Format("recipient={0}&message={1}", "{id:" + userId + "}", message);

                    var content = new StringContent(sendData, Encoding.UTF8, "application/json");
                    await client.PostAsync(url, content);

                    //HttpResponseMessage msg =  await client.PostAsync(url, content);
                    //var contents = await msg.Content.ReadAsStringAsync();
                    //Console.WriteLine("\n\n\nStatucs Code:"+contents);
                    //Console.WriteLine();

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public async Task SendSeenResponse(string userId, string PageAccessToken)
        {
            try
            {
                string url = "https://graph.facebook.com/v2.6/me/messages?access_token=" + PageAccessToken;
                string sendData = string.Format("recipient={0} & sender_action={1}", "{id:" + userId + "}", "mark_seen");

                HttpClient client = new HttpClient();
                var content = new StringContent(sendData, Encoding.UTF8, "application/json");
                //HttpResponseMessage result = await client.PostAsync(url, content);
                await client.PostAsync(url, content);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task SendTypingResponse(string userId, string PageAccessToken)
        {
            try
            {
                string url = "https://graph.facebook.com/v2.6/me/messages?access_token=" + PageAccessToken;
                string sendData = string.Format("recipient={0} & sender_action={1}", "{id:" + userId + "}", "typing_on");

                HttpClient client = new HttpClient();
                var content = new StringContent(sendData, Encoding.UTF8, "application/json");
                //HttpResponseMessage result = await client.PostAsync(url, content);
                await client.PostAsync(url, content);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<bool> SaveNewPage(FacebookPage model)
        {
            try
            {
                _context.FacebookPages.Add(model);
                return 1 == await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }
        
        public async Task<bool> SaveChatBotInfo(ChatbotInfo model)
        {
            try
            {
                _context.ChatbotInfos.Add(model);
                return 1 == await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        public async Task<int> GetPageIdByLoggedInUserId(string ApplicationUserId)
        {
            FacebookPage data= await _context.FacebookPages.FirstAsync(x => x.ApplicationUserId == ApplicationUserId);
            return data.Id;
        }

        public async Task<FacebookPage> GetFacebookPageById(int PageId)
        {
            return await _context.FacebookPages.FindAsync(PageId);
        }
        public async Task<ChatbotInfo> botKeyInfo(string botkey)
        {
            return await _context.ChatbotInfos.FindAsync(botkey);
        }

        public async Task<bool> UpdatePageGreetingsMessage(int PageId, string GreetingsMessage, string GreetingsMessageEN)
        {
            FacebookPage  facebookPage =  await _context.FacebookPages.FindAsync(PageId);
            facebookPage.PageGreetingMessage = GreetingsMessage;
            facebookPage.PageGreetingMessageEN = GreetingsMessageEN;
            return 1 == await _context.SaveChangesAsync();            
        }

        public async Task<IEnumerable<ChatbotInfo>> GetAllBots()
        {
            var data = await _context.ChatbotInfos.Include(x => x.ApplicationUser).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<string> GetScriptById(int id)
        {
            var data = await _context.ChatbotInfos.Where(x => x.Id == id).AsNoTracking().Select(x => x.scriptText).FirstOrDefaultAsync();

            return data;
        }
    }
}
