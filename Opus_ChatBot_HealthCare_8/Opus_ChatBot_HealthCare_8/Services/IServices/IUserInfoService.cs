using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IUserInfoService
    {
        bool InitUserInfo(string CombinedId);
        bool UpdateUserInfo(string CombinedId, string InfoType, string InfoValue);
        UserInfo GetuserInfo(string CombinedId);
        bool UpdatePatientInfo(UserInfo userInfo);
        bool UpdateProfileInfo(PatientInfoViewModel model);
        UserInfo GetuserInfoByMobile(string mobile);
        //bool SaveUserAppointment(AppoinmentInfo model);
        Task<IEnumerable<PatientInfoViewModel>> GetAppointmentList();
        Task<ChatbotInfo> GetBotInfoByUserName(string username);
        // Task<IEnumerable<AppointmentListVM>> GetAppointmentListInfo();
        Task<List<DoctorInfo>> GetDoctorList();
        Task<List<DepartmentInfo>> GetDepartmentList();
        Task<List<DoctorSpecialization>> GetDoctorSpecializationList();
        Task<List<DepartmentVM>> GetDepartmentDataList(int branchId);
        Task<List<WrapperHeader>> GetWrapperHeaderList();
        Task<List<WrapperHeaderImg>> GetWrapperHeaderImageList();
        Task<int> DeletewrapperHeader(int id);
        Task<int> DeletewrapperHeaderImg(int id);
        bool SaveWrapperHeader(WrapperHeader model);
        bool SaveWrapperHeaderImg(WrapperHeaderImg model);
        Task<WrapperHeader> UpdateWrapperHeader(WrapperHeaderVM model);
        Task<WrapperHeaderImg> UpdateWrapperHeaderImg(WrapperHeaderVM model);
        Task<List<BotRackInfoMaster>> GetBotRackInfo();

    }
}
