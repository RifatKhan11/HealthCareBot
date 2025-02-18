using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Response;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar
{
    public class ResponseBuilder
    {
        private readonly string pageId;
        private readonly string postback;
        private readonly string messgae;
        private readonly string senderId;
        private readonly string QRPayload;
        private readonly string Lang;
        private readonly string combinedId;
        private readonly string userId;
        private readonly IResponseBuilderService responseBuilderService;
        private readonly IOTPService oTPService;
        private readonly IKeyWordQuesService keyWordQuesService;
        private readonly IFacebookService facebookService;

        private readonly string baseUrl;
        private readonly UserLanguge MyLanguage;

        public ResponseBuilder(string pageId, string postback, string messgae, string senderId, string QRPayload, String Lang, IResponseBuilderService responseBuilderService, IOTPService oTPService, string rootpath, IKeyWordQuesService keyWordQuesService, IFacebookService facebookService, string combinedId, string userId)
        {
            this.pageId = pageId;
            this.postback = postback;
            this.messgae = messgae;
            this.senderId = senderId;
            this.QRPayload = QRPayload;
            this.combinedId = combinedId;
            this.userId = userId;
            this.Lang = Lang;
            this.keyWordQuesService = keyWordQuesService;
            this.facebookService = facebookService;
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
            this.responseBuilderService = responseBuilderService;
            this.oTPService = oTPService;
            //baseUrl = "https://tota.azurewebsites.net";opusbot.opus-bd.com
            // baseUrl = "https://f96a575b.ngrok.io";
            // baseUrl = "https://424f108a.ngrok.io";
            baseUrl = "http://103.95.38.180/";
            //baseUrl = "http://localhost:23997/";
            //baseUrl = "https://opusbot.opus-bd.com:93";
            //baseUrl = "https://dataqbd.com/";

        }

        public async Task<List<string>> GenerateResponse()
        {
            List<string> Messages = new List<string>();

            if (QRPayload == "start" || QRPayload == "BAN" || QRPayload == "ENG")
            {
                FacebookPage Data = await responseBuilderService.GetGritingsMessgaeWithMenus(pageId);

                //if (Lang == "ENG") Data.PageGreetingMessage = Data.PageGreetingMessageEN;
                //string response = "{ text:\"" + Data.PageGreetingMessage + "\"}";
                //Messages.Add(response);

                Messages.Add(MenuGenerate(0, Data.Id));//Using Helper Function of menu

            }
            else if (postback == "start")
            {
                FacebookPage Data = await responseBuilderService.GetGritingsMessgaeWithMenus(pageId);
                if (Lang == "ENG") Data.PageGreetingMessage = Data.PageGreetingMessageEN;
                string response = "{ text:\"" + Data.PageGreetingMessage + "\"}";
                Messages.Add(response);
                Messages.Add(LangChoose());//Using Helper Function of lang choose
            }
            else if (QRPayload == "myques" || postback == "myques")
            {
                Messages.Add(CustomQuesResponse()); //Using Helper Function of Myques

            }
            else if (QRPayload != "")
            {
                string[] QRSpiltData = QRPayload.Split(";");
                if (QRSpiltData[0] == "Menu")
                {
                    string response = MenuGenerate(Int32.Parse(QRSpiltData[1]), Int32.Parse(QRSpiltData[2]));//Using Helper Function of menu

                    Messages.Add(response);
                }
                else if (QRSpiltData[0] == "Ques")
                {

                    Question question = responseBuilderService.GetQuestion(Int32.Parse(QRSpiltData[2]), Int32.Parse(QRSpiltData[1]));

                    if (question.Id != 0)
                    {

                        List<quick_replies> quick_replies = new List<quick_replies>();

                        quick_replies.Add(new quick_replies("text", MyLanguage.view, payload: "Ans;" + question.Id + ";" + QRSpiltData[2]));
                        quick_replies.Add(new quick_replies("text", MyLanguage.nextQues, payload: "Ques;" + question.Id + ";" + QRSpiltData[2]));
                        quick_replies.Add(new quick_replies("text", MyLanguage.myQues, payload: "myques"));
                        quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));
                        string questiontext = "";
                        if (Lang == "ENG")
                        {
                            questiontext = question.QuestionTextEN.TrimStart();
                        }
                        else
                        {
                            questiontext = question.QuestionText;
                        }
                        //question.QuestionText = question.QuestionTextEN;

                        string response = "{ text:\"" + questiontext + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

                        Messages.Add(response);

                    }
                    else
                    {
                        List<quick_replies> quick_replies = new List<quick_replies>();

                        quick_replies.Add(new quick_replies("text", MyLanguage.myQues, payload: "myques"));

                        quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));

                        string response = "{ text:\"" + MyLanguage.selectFromBelow + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

                        Messages.Add(response);

                    }
                }
                else if (QRSpiltData[0] == "Ans")
                {
                    Answer answer = responseBuilderService.GetAnswer(Int32.Parse(QRSpiltData[1]));
                    List<quick_replies> quick_replies = new List<quick_replies>();

                    quick_replies.Add(new quick_replies("text", MyLanguage.nextQues, payload: "Ques;" + answer.QuestionId + ";" + QRSpiltData[2]));
                    quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));

                    if (Lang == "ENG") answer.AnswerText = answer.AnswerTextEN;

                    if (answer.AnswerTypeId != 1)
                    {
                        Messages.Add(answer.AnswerText);
                        string response = "{ text:\".\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }
                    else
                    {
                        string response = "{ text:\"" + answer.AnswerText + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

                        Messages.Add(response);
                    }
                }
                else if (QRSpiltData[0] == "KWA")
                {
                    List<quick_replies> quick_replies = new List<quick_replies>();
                    KeyWordQuesAns keyWordQuesAns = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(Int32.Parse(QRSpiltData[1]));
                    quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));

                    if (keyWordQuesAns.more != "" && keyWordQuesAns.more != null)
                    {
                        string response = "{ text:\"" + keyWordQuesAns.answer + "\"}";
                        Messages.Add(response);
                        response = "{ text:\"" + MyLanguage.more + ": " + keyWordQuesAns.more + " \",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }
                    else
                    {
                        string response = "{ text:\"" + keyWordQuesAns.answer + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }

                }
                else if (QRSpiltData[0] == "KWANEXT")
                {
                    KeyWordQuesAns temp = await keyWordQuesService.GetNextKeyWordQuesAnsByCOmbinedID(combinedId);
                    List<quick_replies> quick_replies = new List<quick_replies>();

                    if (temp.question != null && temp.question != "")
                    {
                        Messages.Add("{ text:\"" + MyLanguage.didYouMean + "\"}");
                        quick_replies.Add(new quick_replies("text", MyLanguage.view, payload: "KWA;" + temp.Id.ToString()));
                        quick_replies.Add(new quick_replies("text", MyLanguage.nextQues, payload: "KWANEXT"));
                        string response = "{ text:\"" + temp.question + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }
                    else
                    {
                        quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));
                        string response = "{ text:\"" + MyLanguage.sorry + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }
                }
            }
            else
            {
                List<quick_replies> quick_replies = new List<quick_replies>();
                int id = await facebookService.GetFacebookpageId(pageId);

                KeyWordQuesAns temp = await keyWordQuesService.GetKeyWordQuesAnsByMessageAndFbPageId(id, messgae, combinedId, userId);

                if (temp.question != null && temp.question != "")
                {

                    string tm1 = new String(messgae.Where(Char.IsLetter).ToArray());
                    string tm2 = new String(temp.question.Where(Char.IsLetter).ToArray());
                    if (tm1.ToLower() == tm2.ToLower()) // Exact Match
                    {
                        Messages.Add("{ text:\"" + temp.answer + ".\"}");
                        if (temp.more != "" && temp.more != null)
                            Messages.Add("{ text:\"" + MyLanguage.more + ": " + temp.more + "\"}");
                    }
                    else
                    {
                        Messages.Add("{ text:\"" + MyLanguage.didYouMean + "\"}");
                        quick_replies.Add(new quick_replies("text", MyLanguage.view, payload: "KWA;" + temp.Id.ToString()));
                        quick_replies.Add(new quick_replies("text", MyLanguage.nextQues, payload: "KWANEXT"));
                        string response = "{ text:\"" + temp.question + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                        Messages.Add(response);
                    }
                }
                else
                {
                    quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));
                    string response = "{ text:\"" + MyLanguage.sorry + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
                    Messages.Add(response);
                }

            }

            return Messages;
        }



        #region Helpper Functions
        //Here we generate MenuList
        private string MenuGenerate(int ParrentId, int PageId)
        {

            List<quick_replies> quick_replies = new List<quick_replies>();
            IEnumerable<Menu> Menus = responseBuilderService.GetMenus(ParrentId, PageId);
            if (ParrentId != 0)
            {
                quick_replies.Add(new quick_replies("text", MyLanguage.back.ToString(), "Menu;" + responseBuilderService.GetParrentId(ParrentId) + ";" + PageId));
            }
            foreach (Menu menu in Menus)
            {
                if (Lang == "ENG") menu.MenuName = menu.MenuNameEN;

                if (menu.IsLast)
                {
                    quick_replies.Add(new quick_replies("text", menu.MenuName, "Ques;0;" + menu.Id));
                }
                else
                {
                    quick_replies.Add(new quick_replies("text", menu.MenuName, "Menu;" + menu.Id + ";" + PageId));
                }
            }

            return "{ text:\"" + MyLanguage.selectFromBelow + "\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";
        }

        //This Method For Custom Ques Response;
        private string CustomQuesResponse()
        {
            List<quick_replies> quick_replies = new List<quick_replies>();

            quick_replies.Add(new quick_replies("text", MyLanguage.menu, payload: "start"));

            button button = new button
            {
                type = "web_url",
                title = MyLanguage.ask,
                url = HttpUtility.UrlEncode(baseUrl + "/Queries/UserQuery?pageid=" + pageId + "&userid=" + senderId),
                messenger_extensions = true,
                webview_height_ratio = "full",
                webview_share_button = "hide"
            };

            button_template button_Template = new button_template
            {
                text = MyLanguage.myQues,
                template_type = "button",
                buttons = new List<button>()
            };
            button_Template.buttons.Add(button);

            return "{attachment:{ \"type\":\"template\",\"payload\":" + JsonConvert.SerializeObject(button_Template) + "}}";
        }

        private string LangChoose()
        {
            List<quick_replies> quick_replies = new List<quick_replies>();

            quick_replies.Add(new quick_replies("text", "English", "ENG"));

            quick_replies.Add(new quick_replies("text", "বাংলা", "BAN"));

            return "{ text:\"Select Language\",\"quick_replies\":" + JsonConvert.SerializeObject(quick_replies) + "}";

        }
        #endregion
    }
}
