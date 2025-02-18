using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IAppointmentService
    {
        bool SaveUserAppointment(AppoinmentInfo model);
        Task<AppoinmentInfo> GetAppoinmentInfoByUserId(string userId);
        Task<List<DoctorVisitTimePeriod>> GetDoctorVisitTimeByDoctorId(int doctorId);
        Task<List<TimePeriod>> GetDoctorVisitTimeListByDoctorId(int doctorId);
        Task<AppointmentViewModel> GetBasicAppoinmentInfoByUserId(string userId);
        Task<int> SaveUserFeedback(UserFeedback model);
        Task<List<DashboardListVM>> GetDepartmentInforamtionWithStatus(string botKey);
        Task<List<AppointmentVM>> GetAllAppointmentData(string fDate, string tDate, int branchId);
        Task<IEnumerable<AppointmentVM>> GetTotalAppointmentData(string date, string botKey);
        Task<int> DeleteAppointment(int id);
        Task<AppoinmentInfo> UpdateAppointment(AppointmentListViewModel model);
        Task<List<AppointmentVM>> GetTodayAppointmentData(string date, int branchId);
        Task<List<DoctorInfo>> GetDoctorList();
        Task<DoctorInfo> SaveDoctor(DoctorVM model);
        Task<List<TimePeriod>> GetAllTimeSlots(string username);
        Task<List<DoctorInfo>> GetDoctorInfos(string username);
        Task<List<GetTimeSlotsByDoctorIdVm>> GetTimeSlotsByDoctorId(int doctorId);

        Task<int> RemoveAllTimeSlotByDoctorId(int doctorId);
        Task<int> SaveDoctorTimeSlots(DoctorVisitTimePeriod model);
        Task<List<UserFeedback>> GetAllPendingFeedbackData(string username);
        Task<List<UserFeedback>> GetAllRepliedFeedbackData(string username);
        Task<List<UserQuery>> GetAllPendingQueryData(string username);
        Task<int> SaveUserQuery(UserQuery model);
        bool SaveDepartment(DepartmentInfo model);
        Task<int> DeleteDepartment(int id);
        Task<int> DeleteDoctor(int id);
        Task<DepartmentInfo> UpdateDepartment(DepartmentVM model);
        Task<UserQuery> GetQueryById(int id);
        Task<UserFeedback> GetFeedbackById(int id);
        Task<List<UserQuery>> GetAllRepliedQueryData(string username);
        Task<List<UserFeedbackViewModel>> GetAllPendingQueryDataList(string username);
        Task<List<UserFeedbackQueryViewModel>> GetAllPendingQueryDataList2(string username, string fdate, string tdate);
        Task<List<UserFeedbackQueryViewModel>> GetAllRepliedPendingQueryDataList(string username, string fdate, string tdate);
        Task<List<UserFeedback>> GetAllLoadPendingQueryDataList(string username, string fdate, string tdate);
        Task<List<UserFeedback>> GetAllLoadFeedbackData(string username, string fdate, string tdate);
        Task<List<DoctorViewModel>> GetDoctorInfoList(int branchId);
        Task<List<ApiListVM>> GetAllApiInsertDataList();
        Task<IEnumerable<ApiDepartmentData>> GetAllApiDeparmentDataList();
        Task<IEnumerable<ApiSpecialisationData>> GetAllApiSpecialisationDataList();
        Task<IEnumerable<ApiDoctorData>> GetAllApiDoctorDataList();
        Task<int> DeleteApiDepartmentById(int id);
        Task<int> DeleteApiDoctorById(int id);
        Task<int> DeleteApiSpecialisationById(int id);
        Task<int> SaveEvercareToken(EvercareToken models);
        Task<IEnumerable<EvercareToken>> GetActiveTokenList();
        Task<IEnumerable<ApiActivityLog>> GetApiActivityLog();
        Task<int> SaveApiActivityLog(ApiActivityLog models);
        Task<IEnumerable<ApiDoctorData>> GetAllApiDoctorDataListWithSpecialisation(string specialisation);
        Task<IEnumerable<ApiDoctorData>> InActiveApiDoctorDataList(string specialisation);

    }
}
