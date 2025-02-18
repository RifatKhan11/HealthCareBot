using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.LogicAdaptar.Response;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class QuestionReplayService : IQuestionReplayService
    {
        private readonly ApplicationDbContext _contex;

        public QuestionReplayService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public IEnumerable<AnswerType> AnswerTypesAsync()
        {
            return _contex.AnswerTypes.AsNoTracking();

        }

        public bool DeleteQuesRelay(QuestionReplayViewModel model)
        {
            try
            {
                int count = _contex.Answers.Where(x => x.QuestionId == model.QuestionId).AsNoTracking().ToList().Count;
                _contex.Answers.Remove(_contex.Answers.Find(model.AnswerId));
                if (count == 1)
                {
                    _contex.Questions.Remove(_contex.Questions.Find(model.QuestionId));

                    return 2 == _contex.SaveChanges();
                }
                return 1==_contex.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<int> GetAllQUesCountByFbPageID(int fbPageId)
        {
            return await _contex.Questions.Where(x => x.Menu.FacebookPageId == fbPageId).CountAsync();
        }

        public async Task<Question> GetquestionbymenuID(int Id)
        {
            return await _contex.Questions.Where(x => x.MenuId == Id).FirstOrDefaultAsync();
        }
        public async Task<Answer> GetanswerbyqID(int Id)
        {
            return await _contex.Answers.Where(x => x.QuestionId == Id).FirstOrDefaultAsync();
        }
        public async Task<Answer> Getanswerbymenuid(int Id)
        {
            return await _contex.Answers.Include(x=>x.Question.Menu).Where(x => x.Question.MenuId == Id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MenuQuestionAnswer>> GetAllQuestionWithMenuAnser(int fbPageId)
        {
            return await _contex.menuQuestionAnswers.FromSql("EXEC SP_MenuQuestionAnswers {0}", fbPageId).ToListAsync();
        }

        public bool SaveNewCrousal(QuestionReplayViewModel model)
        {
            try
            {
                GenericTemplate genericTemplate = new GenericTemplate
                {
                    template_type = "generic",
                    elements = new List<GenericElement>()
                };

                GenericTemplate genericTemplateEN = new GenericTemplate
                {
                    template_type = "generic",
                    elements = new List<GenericElement>()
                };

                for (int i = 0; i < model.GenericTemplates.Count; i++)
                {
                    if (model.GenericTemplates[i].image_url != null && model.GenericTemplates[i].title != null && model.GenericTemplates[i].titleEN != null)
                    {
                        GenericElement genericElement = new GenericElement
                        {
                            image_url = HttpUtility.UrlEncode(model.GenericTemplates[i].image_url),
                            title = model.GenericTemplates[i].title,
                            subtitle = model.GenericTemplates[i].subtitle,
                            buttons = new List<button>()
                        };

                        GenericElement genericElementEN = new GenericElement
                        {
                            image_url = HttpUtility.UrlEncode(model.GenericTemplates[i].image_url),
                            title = model.GenericTemplates[i].titleEN,
                            subtitle = model.GenericTemplates[i].subtitleEN,
                            buttons = new List<button>()
                        };

                        for (int j = 0; j < 3; j++)
                        {
                            if (model.GenericTemplates[i].buttons[j].Title != null && model.GenericTemplates[i].buttons[j].TitleEN != null && model.GenericTemplates[i].buttons[j].url != null)
                            {
                                button button = new button
                                {
                                    type = "web_url",
                                    title = model.GenericTemplates[i].buttons[j].Title,
                                    url = HttpUtility.UrlEncode(model.GenericTemplates[i].buttons[j].url)
                                };

                                genericElement.buttons.Add(button);

                                button buttonEN = new button
                                {
                                    type = "web_url",
                                    title = model.GenericTemplates[i].buttons[j].TitleEN,
                                    url = HttpUtility.UrlEncode(model.GenericTemplates[i].buttons[j].url)
                                };

                                genericElementEN.buttons.Add(buttonEN);
                            }
                        }

                        genericTemplate.elements.Add(genericElement);
                        genericTemplateEN.elements.Add(genericElementEN);
                    }
                }

                string genericTemplateString = "{attachment:{ \"type\":\"template\",\"payload\":" + JsonConvert.SerializeObject(genericTemplate) + "}}";
                string genericTemplateStringEN = "{attachment:{ \"type\":\"template\",\"payload\":" + JsonConvert.SerializeObject(genericTemplateEN) + "}}";

                Question ques = new Question
                {
                    MenuId = model.MenuId,
                    QuestionText = model.QuestionText,
                    QuestionTextEN = model.QuestionTextEN
                };

                _contex.Questions.Add(ques);

                if (_contex.SaveChanges() == 1)
                {
                    Answer ans = new Answer
                    {
                        QuestionId = ques.Id,
                        AnswerText = genericTemplateString,
                        AnswerTypeId = model.AnswerTypeId,
                        AnswerTextEN = genericTemplateStringEN
                    };

                    _contex.Answers.Add(ans);

                    if (_contex.SaveChanges() == 1) return true;
                }

                return false;

                //Console.WriteLine("\n\n\n");
                //Console.WriteLine(genericTemplateString);
                //Console.WriteLine("\n\n\n");
                //Console.WriteLine(genericTemplateStringEN);
                //Console.WriteLine("\n\n\n");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool SaveNewQuesReplay(QuestionReplayViewModel model)
        {
            try
            {
                Question ques = new Question
                {
                    MenuId = model.MenuId,
                    QuestionText = model.QuestionText,
                    QuestionTextEN = model.QuestionTextEN
                };

                _contex.Questions.Add(ques);
                if (_contex.SaveChanges() == 1)
                {
                    Answer ans = new Answer
                    {
                        QuestionId = ques.Id,
                        AnswerText = model.AnswereText,
                        AnswerTypeId = model.AnswerTypeId,
                        AnswerTextEN = model.AnswereTextEN
                    };

                    _contex.Answers.Add(ans);

                    if (_contex.SaveChanges() == 1) return true;
                }
                return false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public bool SaveNewTextWithButton(QuestionReplayViewModel model)
        {

            try
            {
                Question ques = new Question
                {
                    MenuId = model.MenuId,
                    QuestionText = model.QuestionText,
                    QuestionTextEN = model.QuestionTextEN
                };

                _contex.Questions.Add(ques);

                button_template button_Template = new button_template
                {
                    text = model.AnswereText,
                    template_type = "button",
                    buttons = new List<button>()
                };

                button_template button_TemplateEN = new button_template
                {
                    text = model.AnswereTextEN,
                    template_type = "button",
                    buttons = new List<button>()
                };

                for (int i = 0; i < 3; i++)
                {
                    if (model.TextWithButton[i].Title != null && model.TextWithButton[i].TitleEN != null && model.TextWithButton[i].url != null)
                    {
                        button button = new button
                        {
                            type = "web_url",
                            title = model.TextWithButton[i].Title,
                            url = HttpUtility.UrlEncode(model.TextWithButton[i].url)
                        };

                        button_Template.buttons.Add(button);

                        button buttonEN = new button
                        {
                            type = "web_url",
                            title = model.TextWithButton[0].TitleEN,
                            url = HttpUtility.UrlEncode(model.TextWithButton[i].url)
                        };

                        button_TemplateEN.buttons.Add(buttonEN);
                    }
                }

                string AnswerText = "{attachment:{ \"type\":\"template\",\"payload\":" + JsonConvert.SerializeObject(button_Template) + "}}";

                string AnswerTextEN = "{attachment:{ \"type\":\"template\",\"payload\":" + JsonConvert.SerializeObject(button_TemplateEN) + "}}";

                if (_contex.SaveChanges() == 1)
                {
                    Answer ans = new Answer
                    {
                        QuestionId = ques.Id,
                        AnswerText = AnswerText,
                        AnswerTypeId = model.AnswerTypeId,
                        AnswerTextEN = AnswerTextEN
                    };

                    _contex.Answers.Add(ans);

                    if (_contex.SaveChanges() == 1) return true;
                }
                return false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool UpdateQuesReplay(QuestionReplayViewModel model)
        {
            try
            {
                Answer answer = _contex.Answers.Find(model.AnswerId);
                answer.AnswerText = model.AnswereText;
                answer.AnswerTextEN = model.AnswereTextEN;
                _contex.SaveChanges();

                Question question = _contex.Questions.Find(model.QuestionId);
                question.QuestionText = model.QuestionText;
                question.QuestionTextEN = model.QuestionTextEN;
                _contex.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
