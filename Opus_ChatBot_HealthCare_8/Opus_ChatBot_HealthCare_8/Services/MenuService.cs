using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;


        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetMenus(int faceBookPageId)
        {
            return await _context.Menus.Where(x => x.FacebookPageId == faceBookPageId && x.IsLast == true).ToListAsync();
        }
        public async Task<IEnumerable<Menu>> GetMenusBySpecialities(int id)
        {
            return await _context.Menus.Where(x => x.ParrentId == id).ToListAsync();
        }
        public async Task<IEnumerable<Menu>> GetMenusbyname(string name, int faceBookPageId)
        {

            return await _context.Menus.Where(x => x.FacebookPageId == faceBookPageId && x.MenuNameEN.ToLower() == name|| x.FacebookPageId == faceBookPageId && x.MenuName.ToLower() == name).ToListAsync();
        }
        public async Task<Menu> GetMenusbyId(int Id)
        {
            return await _context.Menus.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveMenu(MenuViewModel model, int faceBookPageId)
        {
            try
            {
                var entity = new Menu
                {
                    MenuName = model.MenuName,
                    MenuNameEN = model.MenuNameEN,
                    FacebookPageId = faceBookPageId,
                    IsLast = (model.IsLast == "on" ? true : false),
                    ParrentId = model.ParrentMenuId
                };
                _context.Menus.Add(entity);

                return 1 == await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<int> GetMenuCount(int fbPageId)
        {
            return await _context.Menus.CountAsync(x => x.FacebookPageId == fbPageId);
        }
        public async Task<int> GetMenusId(string menuname,int fbPageId)
        {
            return  _context.Menus.Where(x => x.FacebookPageId == fbPageId&& x.MenuNameEN.Contains(menuname.Trim())).Select(x=>x.Id).FirstOrDefault();
        }
        public async Task<IEnumerable<Menu>> GetMenusByParrentname(string  parentname, int faceBookPageId)
        {
            int id = _context.Menus.Where(x => x.MenuNameEN.Contains(parentname.Trim())).Select(x=>x.Id).FirstOrDefault();
            
            return await _context.Menus.Where(x => x.ParrentId == id && x.FacebookPageId == faceBookPageId && x.ParrentId != 0).ToListAsync();
        }
        public async Task<IEnumerable<Menu>> GetMenusByParrentId(int parrentId, int faceBookPageId)
        {
            if (parrentId == -1)
            {

                parrentId = 0;
            }
            return await _context.Menus.Where(x => x.ParrentId == parrentId && x.FacebookPageId == faceBookPageId).ToListAsync();
        }
       

        public bool RenameMenu(MenuViewModel model)
        {
            try
            {
                Menu menu = _context.Menus.Find(model.ParrentMenuId);
                menu.MenuName = model.MenuName;
                menu.MenuNameEN = model.MenuNameEN;
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            throw new NotImplementedException();
        }

        public string DeleteMenu(int MenuId)
        {

            try
            {
                Menu menu = _context.Menus.Find(MenuId);
                if (menu != null)
                {
                    bool status;
                    if (menu.IsLast) status = IsAnyQuesReferance(MenuId);
                    else status = IsAnyMenuRefarance(MenuId);

                    if (!status)
                    {
                        _context.Menus.Remove(menu);

                        if (_context.SaveChanges() == 1) return "success";

                        return "Something Went Wrong.";
                    }
                }
                return "Invalid Node for this action";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something Went Wrong.";
            }
        }

        public string MakeLastNode(int MenuId)
        {
            try
            {
                Menu menu = _context.Menus.Find(MenuId);
                if (menu != null && menu.IsLast == false)
                {
                    if (!IsAnyMenuRefarance(MenuId))
                    {
                        menu.IsLast = true;
                        if (_context.SaveChanges() == 1) return "success";
                        return "Something Went Wrong.";
                    }
                }
                return "Invalid Node for this action";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something Went Wrong.";
            }
        }

        public string MakeGeneralNode(int MenuId)
        {
            try
            {
                Menu menu = _context.Menus.Find(MenuId);
                if (menu != null && menu.IsLast == true)
                {
                    if (!IsAnyQuesReferance(MenuId))
                    {
                        menu.IsLast = false;
                        if (_context.SaveChanges() == 1) return "success";
                        return "Something Went Wrong.";
                    }
                }
                return "Invalid Node for this action";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something Went Wrong.";
            }
        }


        #region HelperMethods
        private bool IsAnyMenuRefarance(int MenuId)
        {
            int n = _context.Menus.Where(x => x.ParrentId == MenuId).AsNoTracking().ToList().Count;
            if (n > 0) return true;
            return false;
        }

        private bool IsAnyQuesReferance(int MenuId)
        {
            int n = _context.Questions.Where(x => x.MenuId == MenuId).AsNoTracking().ToList().Count;
            if (n > 0) return true;
            return false;
        }
        #endregion
        #region menuhitlog
        public async Task<bool> SaveMenuHitLog(MenuHitLog menuHitLog)
        {
            try
            {
                if (menuHitLog.Id != 0)
                {
                    _context.menuHitLogs.Update(menuHitLog);
                   
                }
                else
                {
                    _context.menuHitLogs.Add(menuHitLog);
                }

                await _context.SaveChangesAsync();
                //return passportInfo.Id;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<IEnumerable<TotalHitMenuLogViewModel>> GetMenuHitLogByDate(DateTime FDate, DateTime TDate, int fbid)
        {
            var data= await _context.totalHitMenuLogViewModels.FromSql($"getmenuhitlog {FDate},{TDate},{fbid}").AsNoTracking().ToListAsync();
            return data;
        }
        public async Task<IEnumerable<TotalHitMenuLogViewModel>> GetMenuHitLogByDateWOD(int fbid)
        {
            var data= await _context.totalHitMenuLogViewModels.FromSql($"getmenuhitlogWOD {fbid}").AsNoTracking().ToListAsync();
            return data;
        }
        #endregion
    }
}
