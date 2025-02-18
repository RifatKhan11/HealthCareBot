using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class DoctorInfoService: IDoctorInfoService
    {
        private readonly ApplicationDbContext _context;

        public DoctorInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveDoctorInfo(DoctorInfo model)
        {
            try
            {
                if (model.Id>0)
                {

                    _context.Update(model);
                }
                else
                {

                    _context.Add(model);
                }

                if (await _context.SaveChangesAsync() == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<int> SavedDoctorDetails(DoctorInfo model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.DoctorInfos.Update(model);
                }
                else
                {

                    _context.DoctorInfos.Add(model);
                }

                await _context.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception e)
            {
                throw e;

            }

        }

        public async Task<int> SaveDepartment(DepartmentInfo model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.DepartmentInfos.Update(model);
                }
                else
                {

                    _context.DepartmentInfos.Add(model);
                }

                await _context.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception e)
            {
                throw e;

            }
            
        }

        public async Task<int> SaveDoctorSpecialization(DoctorSpecialization model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.DoctorSpecializations.Update(model);
                }
                else
                {

                    _context.DoctorSpecializations.Add(model);
                }

                await _context.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<DoctorInfo> GetDoctorInfobyid(int Id)
        {
            var data = await _context.DoctorInfos.Include(x=>x.menu).Where(x=>x.Id==Id).FirstOrDefaultAsync();
            return data;
        }

        public async Task<IEnumerable<DoctorInfo>> GetDoctorListbymenuid(int Id)
        {
            var data= await _context.DoctorInfos.Include(x => x.menu).Where(x => x.menuId == Id).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<DoctorInfo>> GetDoctorListWithSpecialist()
        {
            var data = await _context.DoctorInfos.Include(x => x.menu).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<DoctorInfo>> GetDoctorList()
        {
            var data = await _context.DoctorInfos.Include(x => x.Department).Include(x=>x.doctorSpecialization).ToListAsync();
            return data;
        }
        public async Task<IEnumerable<DepartmentInfo>> GetAllDepartmentInfo()
        {
            var data = await _context.DepartmentInfos.ToListAsync();
            return data;
        }
        public async Task<IEnumerable<DoctorSpecialization>> GetAllDoctorSpecialization()
        {
            var data = await _context.DoctorSpecializations.ToListAsync();
            return data;
        }


        public async Task<DoctorInfoForExcel> GetExcelLeadDataId(DoctorInfoForExcel model)
        {
            var data = model;

            var result = new DoctorInfoForExcel
            {
                DoctorSpecialization = data.DoctorSpecialization == null ? null : await _context.DoctorSpecializations.Where(x => x.name == data.DoctorSpecialization.Trim()).Select(x => x.Id.ToString()).FirstOrDefaultAsync(),
                Department = data.Department == null ? null : await _context.DepartmentInfos.Where(x => x.departmentName == data.Department.Trim()).Select(x => x.Id.ToString()).FirstOrDefaultAsync(),

            };

            return result;
        }
    }
}
