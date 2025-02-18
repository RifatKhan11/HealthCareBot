using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.LogicAdaptar;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookController : ControllerBase
    {
        private readonly string VerifyToken = "opus_verify";
        private readonly IFacebookService _facebookService;
        private readonly IResponseBuilderService responseBuilderService;
        private readonly IOTPService oTPService;
        private readonly ILanguageService languageService;
        private readonly IWebHostEnvironment  _hostingEnvironment;
        private readonly IBotFlowService botFlowService;
        private readonly IServiceFlowService serviceFlowService;
        private readonly IUserInfoService userInfoService;
        private readonly IPassportInfoService passportInfoService;
        private readonly IBankInfoService bankInfoService;
        private readonly IKeyWordQuesService keyWordQuesService;
        private readonly IAnalyticsService analyticsService;


        public FacebookController(IFacebookService facebookService, IResponseBuilderService responseBuilderService, ILanguageService languageService, IWebHostEnvironment  hostingEnvironment, IOTPService oTPService, IServiceFlowService serviceFlowService, IBotFlowService botFlowService, IUserInfoService userInfoService, IPassportInfoService passportInfoService, IBankInfoService bankInfoService, IKeyWordQuesService keyWordQuesService, IAnalyticsService analyticsService)
        {
            _facebookService = facebookService;
            _hostingEnvironment = hostingEnvironment;
            this.responseBuilderService = responseBuilderService;
            this.languageService = languageService;
            this.oTPService = oTPService;
            this.serviceFlowService = serviceFlowService;
            this.botFlowService = botFlowService;
            this.userInfoService = userInfoService;
            this.passportInfoService = passportInfoService;
            this.bankInfoService = bankInfoService;
            this.keyWordQuesService = keyWordQuesService;
            this.analyticsService = analyticsService;
        }

        [HttpGet("Webhook")]
        public IActionResult WebhooksGet([FromQuery(Name = "hub.challenge")] string challenge, [FromQuery(Name = "hub.verify_token")] string verifyToken)
        {
            if (verifyToken == this.VerifyToken && !string.IsNullOrEmpty(challenge))
            {
                return Ok(challenge);
            }

            return BadRequest();
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> WebhooksPost()
        {
            //Console.WriteLine("Hook Fired");
            //return Ok();

            dynamic Data = (dynamic)null;
            string json = string.Empty;

            try
            {
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = await sr.ReadToEndAsync();
                }

                Data = JsonConvert.DeserializeObject(json);

                #region Interested Data

                string pageId = Data.entry[0].id;
                string senderId = Data.entry[0].messaging[0].sender.id;
                string combinedId = pageId + senderId;
                string postBack = "";
                string message = "";
                string QRPayload = "";
                string Lang = "BAN";

                #endregion

                string AccessToken = await _facebookService.GetAccessToken(pageId);
                if (AccessToken == null) return BadRequest();

                //_facebookService.SendSeenResponse(senderId, AccessToken);
                await _facebookService.SendTypingResponse(senderId, AccessToken);




                #region Interested Data Caparing
                if (Data.entry[0].messaging[0].ContainsKey("message"))
                {
                    message = Data.entry[0].messaging[0].message.text;
                    if (Data.entry[0].messaging[0].message.ContainsKey("quick_reply"))
                        QRPayload = Data.entry[0].messaging[0].message.quick_reply.payload;
                }

                if (Data.entry[0].messaging[0].ContainsKey("postback"))
                    postBack = Data.entry[0].messaging[0].postback.payload;

                if (QRPayload == "BAN" || QRPayload == "ENG") await languageService.SaveMyLanguage(pageId, senderId, QRPayload);
                Lang = languageService.MyLanguage(pageId, senderId);
                if (Lang == null || Lang == "") Lang = "BAN";
                #endregion



                #region Analytics Data Push
                try
                {
                    Analytics analytics = new Analytics
                    {
                        DateTime = DateTime.Now,
                        PageId = pageId,
                        SenderId = senderId,
                        QueryType = QRPayload + message
                    };

                    await analyticsService.SaveAnalytics(analytics);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n\n\n\n\n Message=  ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("\n\n\n\n\n");

                }
                #endregion



                List<string> messages = new List<string>();

                if (postBack == "start")
                {
                    string User_Name = await _facebookService.GetUserName(senderId, AccessToken);
                    if (Lang == "ENG")
                        messages.Add("{ text:\"Hello!! " + User_Name + "\"}");
                    else
                        messages.Add("{ text:\"হ্যালো!! " + User_Name + "\"}");
                }

                #region desiding flow
                string flow = "default";

                if (postBack == "start" && postBack != "blnsbank" && postBack != "")
                {
                    flow = botFlowService.UpdateFlow(combinedId, "default");
                    serviceFlowService.CLearServiceData(combinedId);
                }
                else if (postBack != "")
                {
                    flow = botFlowService.UpdateFlow(combinedId, postBack);
                    serviceFlowService.InitNewService(combinedId, postBack, "start");
                }
                else
                {
                    flow = botFlowService.GetCurrentFlowStatus(combinedId);
                }
                #endregion

                #region Redirecting Bot Flow
                if (flow == "default" || flow == "myques")
                {
                    ResponseBuilder responseBuilder = new ResponseBuilder(pageId, postBack, message, senderId, QRPayload, Lang, responseBuilderService, oTPService, _hostingEnvironment.ContentRootPath, keyWordQuesService, _facebookService, combinedId, senderId);
                    messages.AddRange(await responseBuilder.GenerateResponse());
                }
                else
                {
                    ServiceManager serviceManager = new ServiceManager(pageId, message, senderId, QRPayload, combinedId, Lang, _hostingEnvironment.ContentRootPath, serviceFlowService, botFlowService, userInfoService, oTPService, passportInfoService, bankInfoService);
                    messages.AddRange(await serviceManager.GenerateResponse());
                }
                #endregion

                await _facebookService.SendMessageToFacebook(senderId, messages, AccessToken);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return BadRequest();
            }

            return Ok();
        }
    }
}