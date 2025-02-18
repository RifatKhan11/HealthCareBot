using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.ApiModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using static Opus_ChatBot_HealthCare_8.Controllers.HomeController;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IBotService
    {
        Task<List<string>> CustomMessageGenerator(string senderId, string pageId, string message, string postback, string userId, string botKey, string conectionId, string messagejson, Models.BotModels.ConnectionInfo connectionInfo);
        Task<List<string>> QuesReplayService(string senderId, string pageId, string message, string postBack, string userId, string botKe, string conectionId);
        Task<List<string>> QuesReplayServiceK(string senderId, string pageId, string message, string postBack, string userId, string botKey, string conectionId);
        Task<IEnumerable<Menu>> GetData(string parentId, string pageid);
        int CloseService(string combinedId);
        Task<IEnumerable<Menu>> GetMenusByBotKey(int parrentId, int faceBookPageId, string botKey);
        Task<bool> SaveMenu(MenuViewModel model, int faceBookPageId, string botKey);
        bool RenameMenu(MenuViewModel model, string botKey);
        Task<ChatbotInfo> GetBotInfoByUserName(string username);
        Task<IEnumerable<Menu>> GetDataByBotKey(int parrentId, int faceBookPageId, string botKey);
        Task<bool> SaveLastGreetings(LastGrettings model);
        Task<bool> SaveMessageLog(MessageLog model);
        Task<Menu> GetMenuByIdAndBotKey(int menuId, string botKey);
        Task<List<string>> CustomInputMessageGenerator(int menuId, string botKey, string connectionId);
        Task<ServiceFlow> GetPendingQuestion(string botKey, string connectionId, string message);
        Task<bool> SaveServiceFlow(ServiceFlow model);
        Task<IEnumerable<Menu>> GetAllMenusByBotKey(string botKey);
        Task<IEnumerable<BotKnowledge>> GetAllBotKnowledgeByBotKey(string botKey);
        Task<KeyWordQuesAns> GetKeywordQuesById(int? id);
        Task<IEnumerable<MenuReader>> GetMenuReaderByMenuId(int menuId);
        Task<bool> SaveMenuReader(MenuReader model);
        Task<int> DeleteMenuReader(int id);
        Task<List<string>> GetPreMessageByWrapperDetailsId(int id);
        Task<int?> GetQuestionByText(string text);
        Task<List<KeyWordQuesAns>> GetQuestions(string postback, string message, string botKey, string connectionId, int qId, Models.BotModels.ConnectionInfo connectionInfo);
        Task<List<string>> SendMessage(List<KeyWordQuesAns> questionsByKeyword, string botKey, string connectionId);
        Task<List<string>> SendNextMessageByNextNodeId(string botKey, string connectionId, int? nextNodeId);
        Task<string> SendSMSAsync(string mobile, string message);
        Task<List<string>> CheckAndValidateOTP(string otp);
        Task<int?> SendOTPByNodeId(string botKey, string connectionId, int? nodeId);
        Task<string> GetLastOTPByConnectionId(string connectionId);
        Task<List<string>> GetResponseByTypingText(string msg, string connectionId, string botKey, string userId);
        Task<string> SaveUserByOTP(string otp, string uhid);
        Task<IEnumerable<AppoinmentInfo>> GetAllAppointment(string botKey);
        Task<IEnumerable<AppoinmentInfo>> GetOngoingAppointment(string botKey);
        Task<IEnumerable<AppoinmentInfo>> GetConfirmedAppointment(string botKey);
        Task<List<string>> SendNextMessageByNodeId(string botKey, string connectionId, int? nodeId);
        List<KeyWordQuesAns> responseMessagesByNodeId(string botKey, int? nodeId);
        Task<IEnumerable<BotKnowledge>> GetKnowledgeByBotKey(string botKey);
        Task<List<KeyWordQuesAns>> SendNextKeyWordQuestionsByNodeId(string botKey, string connectionId, int? nodeId);
        #region OtherServices

        #endregion

        Task<int> SaveConnectionInfo(Models.BotModels.ConnectionInfo model);
        Task<int> AddRescheduleDoctor(int scheduleId);
        Task<IEnumerable<KeyWordQuesAns>> GetAllBotQuestionsByBotKey(string botKey);
        Task<KeyWordQuestionDetailVm> GetKeywordQuestionAnsById(int? id);
        Task<IEnumerable<KeyWordQuesAns>> GetAllKeywordQuesAns(string botKey);
        Task<IEnumerable<DepartmentInfo>> GetAllDepartmentInfo(string botKey);
        Task<IEnumerable<DoctorInfo>> GetALlDoctorInfo(string botKey);
        Task<int> SaveOrUpdateKeywordQues(BotKnowledgeViewModel model);
        Task<int> AddMessageLogByQuestionId(int id);
        Task<List<string>> GetRawMessageByNodeId(int? nodeId, string connectionId, string botKey);
        Task<List<string>> CancelAppointmentById(int scheduleId, string connectionId, string botKey);
        Task<int> SaveDoctorNameInServiceFlow(int nextNodeId, string connectionId, string botKey, string doctorName, int scheduleId);
        Task<string> GetConnectionIdByOTP(string otp);
        Task<string> GetBotKeyByOTP(string otp);
        Task<Models.BotModels.ConnectionInfo> GetConnectionInfoByUserId(string userid);
        Task<string> GetPhoneByConnectionId(string connectionId);

        Task<bool> SendHTMLEmail(string mailTo, string subject, string htmlMessage);
        Task<UserInfo> GetUserInfoByuhId(string uhid);
        Task<ChatbotInfo> GetChatBotInfoByBotKey(string botkey);
        Task<int> SaveAllDoctorSlotsFromApi(List<DoctorSlotData> models);
        Task<int> SaveAllDoctorSlotsFromApiEhc(List<DoctorSlotData> models);
        Task<int> SaveApiDepartment(List<ApiDepartment> models);
        Task<int> SaveApiDepartmentEHC(List<ApiDepartment> models);
        Task<int> SaveApiDoctor(List<ApiDoctor> models);
        Task<int> SaveApiSpecialisation(List<ApiSpecialisation> models);
        Task<int> SaveApiDoctorSlot(List<ApiDoctorSlot> models);
        Task<bool> GetAllMasterData();
        Task<EvercareToken> GetActiveToken();
        Task<ApiProcessLogInfo> GetLastProcess();
        Task<int> UpdateTokenMessageStatus();
        Task<int> SaveUhidUserFromApi(List<UserInfo> models);
        Task<int?> SendOTPByNodeId2(string botKey, string connectionId, int? nodeId);
        Task<IEnumerable<ApiDoctor>> GetAllApiDoctor();
        Task<bool> GetDoctorApiId(int doctorId, string date);
        Task<ApiDoctor> GetPrevDoctor(int apiDocId);
        Task<ApiDoctor> GetNextDoctor(int apiDocId);
        Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorVm>> FetchDoctors(string botKey);
        Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorSlotVm>> Fetch7DaysSlot(string botKey, int apiDocId);
        Task<List<Opus_ChatBot_HealthCare_8.Controllers.HomeController.DoctorVm>> FetchDoctors2(string botKey, int apiDocId, string doctorKey);
        Task<List<string>> GetSearchByTypingText(string msg, string connectionId, string botKey, string userId);
        Task<int> SaveApiSpecialisationData(List<ApiSpecialisationData> models);
        Task<int> SaveApiDoctorData(List<ApiDoctorData> models);
        Task<int> SaveApiDepartmentData(List<ApiDepartmentData> models);
        Task<int> SaveBotTextHistory(Opus_ChatBot_HealthCare_8.Models.BotModels.BotMessageHistory data);
        Task<IEnumerable<BotMessageHistory>> GetHistoryData(string pageUserId, int wrapperDetailsId);
    }
}
