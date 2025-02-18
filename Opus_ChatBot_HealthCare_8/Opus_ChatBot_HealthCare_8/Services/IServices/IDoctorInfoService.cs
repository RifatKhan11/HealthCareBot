using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IDoctorInfoService
    {
        Task<bool> SaveDoctorInfo(DoctorInfo model);
        Task<int> SaveDoctorSpecialization(DoctorSpecialization model);
       
        Task<int> SaveDepartment(DepartmentInfo model);
        Task<DoctorInfo> GetDoctorInfobyid(int Id);
        Task<IEnumerable<DoctorInfo>> GetDoctorListbymenuid(int Id);
        Task<IEnumerable<DoctorInfo>> GetDoctorListWithSpecialist();
        Task<IEnumerable<DepartmentInfo>> GetAllDepartmentInfo();
        Task<IEnumerable<DoctorSpecialization>> GetAllDoctorSpecialization();
        Task<IEnumerable<DoctorInfo>> GetDoctorList();
        Task<int> SavedDoctorDetails(DoctorInfo model);
        Task<DoctorInfoForExcel> GetExcelLeadDataId(DoctorInfoForExcel model);


    }
}
