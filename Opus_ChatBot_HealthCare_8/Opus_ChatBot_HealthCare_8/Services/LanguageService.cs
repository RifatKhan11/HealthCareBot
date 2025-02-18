using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Microsoft.EntityFrameworkCore;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext _contex;

        public LanguageService(ApplicationDbContext contex)
        {
            _contex = contex;
        }
        public string MyLanguage(string PageId, string UserId)
        {

            Language language = _contex.Languages.Where(x => x.UserId == UserId && x.PageId == PageId).FirstOrDefault();
            if (language != null) return language.Lang;
            return "";
        }

        public async Task<bool> SaveMyLanguage(string PageId, string UserId, string Lang)
        {
            Language language = await _contex.Languages.Where(x => x.UserId == UserId && x.PageId == PageId).FirstOrDefaultAsync();

            if(language != null)
            {
                language.PageId = PageId;
                language.UserId = UserId;
                language.Lang = Lang;
            }
            else
            {
                language = new Language
                {
                    Lang = Lang,
                    UserId = UserId,
                    PageId = PageId
                };
                _contex.Languages.Add(language);
            }
            return 1 == await _contex.SaveChangesAsync();

        }
    }
}
