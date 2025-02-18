using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetMenus(int faceBookPageId);
        Task<IEnumerable<Menu>> GetMenusByParrentId(int parrentId, int faceBookPageId);
        Task<IEnumerable<Menu>> GetMenusBySpecialities(int id);
        Task<bool> SaveMenu(MenuViewModel model, int faceBookPageId);
        Task<int> GetMenuCount(int fbPageId);
        bool RenameMenu(MenuViewModel model);
        string DeleteMenu(int MenuId);
        string MakeLastNode(int MenuId);
        string MakeGeneralNode(int MenuId);
        Task<Menu> GetMenusbyId(int Id);
        Task<IEnumerable<Menu>> GetMenusByParrentname(string parentname, int faceBookPageId);
        Task<int> GetMenusId(string menuname, int fbPageId);
        Task<bool> SaveMenuHitLog(MenuHitLog menuHitLog);
        Task<IEnumerable<TotalHitMenuLogViewModel>> GetMenuHitLogByDate(DateTime FDate, DateTime TDate, int fbid);
        Task<IEnumerable<Menu>> GetMenusbyname(string name, int faceBookPageId);
        Task<IEnumerable<TotalHitMenuLogViewModel>> GetMenuHitLogByDateWOD(int fbid);
    }
}
