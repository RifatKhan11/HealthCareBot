using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.IServices.IServices;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ISMSService _sMSService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBotService _botService;
        private readonly ApplicationDbContext _context;

        public AppointmentController(IUserInfoService userInfoService, ISMSService sMSService, IAppointmentService appointmentService, IBotService _botService, ApplicationDbContext context)
        {
            _userInfoService = userInfoService;
            _sMSService = sMSService;
            _appointmentService = appointmentService;
            this._botService = _botService;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;

            var userInfo = await _userInfoService.GetBotInfoByUserName(username);

            ViewBag.botkey = userInfo.botKey;
            ViewBag.branchInfoId = userInfo.branchInfoId;

            AppointmentListViewModel model = new AppointmentListViewModel
            {
                patientInfoViewModels = await _userInfoService.GetAppointmentList()
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SavePatientInfo(PatientInfoViewModel model)
        {
            try
            {
                string mobile = "";
                if (!model.mobile.StartsWith("880"))
                {
                    mobile = "88" + model.mobile;
                }
                else
                {
                    mobile = model.mobile;
                }
                Random random = new Random();
                int randomNumber = random.Next(100000, 1000000); // generates a random integer between 1000 and 9999 (inclusive)
                model.otp = randomNumber.ToString();
                var data = _userInfoService.UpdateProfileInfo(model);

                var trans = await _sMSService.SendSMSAsync(mobile, "Your OTP for Appointment is " + randomNumber.ToString(), "");

                return Json(model.mobile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("/api/Appointment/OTPVerified/{combinedId}/{otp}")]
        public IActionResult OTPVerified(string combinedId, string otp)
        {
            try
            {
                var response = new AppointmentViewModel();
                //var userInfo = _userInfoService.GetuserInfoByMobile(mobile);
                var userInfo = _userInfoService.GetuserInfo(combinedId);
                if (userInfo?.otpMsg.Trim() == otp.Trim())
                {
                    var appointment = new AppoinmentInfo
                    {
                        doctorInfoId = Convert.ToInt32(userInfo.keyWordQues),
                        weeksId = Convert.ToInt32(userInfo.bankaccountNumber),
                        userInfoId = userInfo.Id,
                        isVerified = 1,
                        //date=DateTime.Now,
                        status = 0,
                        appointStatus = "Application",
                        entryDate = DateTime.Now,
                        entryby = "mehedi"
                    };
                    _appointmentService.SaveUserAppointment(appointment);

                    response.msg = "success";
                    response.appoinment = appointment;
                    response.dates = AppointmentDates();
                }
                else { response.msg = "fail"; }
                return Json(response);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/Appointment/SaveAppointmentDate/{combinedId}/{date}")]
        public async Task<IActionResult> SaveAppointmentDate(string combinedId, string date)
        {
            try
            {
                string[] dates = date.Split("-");
                string date2 = dates[2] + "-" + dates[1] + "-" + dates[0];
                var response = new AppointmentViewModel();
                //var userInfo = _userInfoService.GetuserInfoByMobile(mobile);
                var userInfo = _userInfoService.GetuserInfo(combinedId);
                var appointmentInfo = await _appointmentService.GetAppoinmentInfoByUserId(userInfo.Id);
                if (appointmentInfo != null)
                {
                    appointmentInfo.date = Convert.ToDateTime(date2);
                    _appointmentService.SaveUserAppointment(appointmentInfo);

                    List<TimePeriod> timeInfo = await _appointmentService.GetDoctorVisitTimeListByDoctorId((int)appointmentInfo.doctorInfoId);
                    response.msg = "success";
                    response.appoinment = appointmentInfo;
                    response.lstMorning = timeInfo.Where(x => x.shiftPeriod == "Morning").Select(x => x.timeSlot).ToList();
                    response.lstEvening = timeInfo.Where(x => x.shiftPeriod == "Evening").Select(x => x.timeSlot).ToList();
                }
                else { response.msg = "fail"; }
                return Json(response);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/Appointment/SaveAppointmentTime/{combinedId}/{time}")]
        public async Task<IActionResult> SaveAppointmentTime(string combinedId, string time)
        {
            try
            {
                var response = new AppointmentViewModel();
                //var userInfo = _userInfoService.GetuserInfoByMobile(mobile);
                var userInfo = _userInfoService.GetuserInfo(combinedId);
                var appointmentInfo = await _appointmentService.GetAppoinmentInfoByUserId(userInfo.Id);
                if (appointmentInfo != null)
                {
                    appointmentInfo.time = time;
                    _appointmentService.SaveUserAppointment(appointmentInfo);

                    var basicInfo = await _appointmentService.GetBasicAppoinmentInfoByUserId(combinedId);
                    response = basicInfo;
                    response.msg = "success";
                    response.appoinment = appointmentInfo;
                }
                else { response.msg = "fail"; }
                return Json(response);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/Appointment/SaveAppointmentConfirm/{combinedId}")]
        public async Task<IActionResult> SaveAppointmentConfirm(string combinedId)
        {
            try
            {
                var response = new AppointmentViewModel();
                //var userInfo = _userInfoService.GetuserInfoByMobile(mobile);
                var userInfo = _userInfoService.GetuserInfo(combinedId);
                var appointmentInfo = await _appointmentService.GetAppoinmentInfoByUserId(userInfo.Id);
                if (appointmentInfo != null)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(100000, 1000000); // generates a random integer between 1000 and 9999 (inclusive)

                    appointmentInfo.status = 1;
                    appointmentInfo.bookingNo = randomNumber.ToString();
                    _appointmentService.SaveUserAppointment(appointmentInfo);
                    var basicInfo = await _appointmentService.GetBasicAppoinmentInfoByUserId(combinedId);
                    response = basicInfo;
                    response.msg = "success";
                    response.appoinment = appointmentInfo;
                }
                else { response.msg = "fail"; }
                return Json(response);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public List<string> AppointmentDates()
        {
            var result = new List<string>();
            DateTime today = DateTime.Today;

            for (int i = 0; i < 7; i++)
            {
                DateTime nextDay = today.AddDays(i);
                result.Add(nextDay.ToString("dd-MM-yyyy"));
            }

            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveQuery(UserFeedbackViewModel model)
        {
            var botInfo = await _botService.GetChatBotInfoByBotKey(model.botKeyNew);

            string message = "success";

            //var userInfo = await _botService.GetUserInfoByuhId(model.uhid);

            //if (userInfo == null)
            //{
            //    message = "user not found";
            //}
            if (model.query != null && model.phone != null)
            {
                if (model.phone.Length == 11 || model.phone.Length == 13)
                {
                    var data = new UserQuery
                    {
                        querydate = model.querydate,
                        uhid = model.uhid,
                        querytime = model.querytime,
                        query = model.query,
                        phone = model.phone,
                        entryDate = DateTime.Now,
                        entryby = "mehedi",
                        status = 0,
                        botKey = model.botKeyNew,
                        branchInfoId = botInfo.ApplicationUser?.branchId
                        // email = userInfo.Email
                    };

                    await _appointmentService.SaveUserQuery(data);
                }
                else
                {
                    message = "phoneInvalid";
                }

            }
            else
            {
                message = "error";
            }


            return Json(message);
        }

        [HttpGet]
        public async Task<IActionResult> SendFeedbackReply(int id, string query, string email, string reply)
        {
            var sub = "Feedback Reply: " + query;
            var result = await _botService.SendHTMLEmail(email, sub, reply);

            var data = await _appointmentService.GetFeedbackById(id);

            if (data != null)
            {
                data.repliedDate = DateTime.Now;
                data.replied = 1;
                data.replyText = reply;

                await _appointmentService.SaveUserFeedback(data);
            }

            return Json(result);
        }

        #region Query
        [HttpGet]
        public async Task<IActionResult> SendQueryReply(int id, string query, string email, string reply, int status)
        {
            var sub = "Query Reply: " + query;
            var result = await _botService.SendHTMLEmail(email, sub, reply);

            var data = await _appointmentService.GetQueryById(id);
            if (data != null)
            {
                data.repliedDate = DateTime.Now;
                data.replied = 1;
                data.replyText = reply;
                data.status = status;

                await _appointmentService.SaveUserQuery(data);
            }

            //return Json(result);
            return Json(data);

        }


        [HttpGet]
        public async Task<IActionResult> SendQueryChangeStatus(int id, int status)
        {
            //var sub = "Query Reply: " + query;
            //var result = await _botService.SendHTMLEmail(email, sub, reply);

            var data = await _appointmentService.GetQueryById(id);
            if (data != null)
            {
                data.repliedDate = DateTime.Now;
                //data.replied = 1;
                //data.replyText = reply;
                data.status = status;

                await _appointmentService.SaveUserQuery(data);
            }

            return Json(data);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveFeedback(UserFeedbackViewModel model)
        {
            var botInfo = await _botService.GetChatBotInfoByBotKey(model.botKeyNew);

            string message = "success";
            if (model.phone != null && model.name != null && model.email != null && model.message != null)
            {
                if (model.phone.Length == 11 || model.phone.Length == 13)
                {
                    var data = new UserFeedback
                    {
                        name = model.name,
                        email = model.email,
                        phone = model.phone,
                        message = model.message,
                        entryDate = DateTime.Now,
                        effectiveDate = DateTime.Now,
                        entryby = "mehedi",
                        botKey = model.botKeyNew,
                        replied = 0,
                        branchInfoId = botInfo.ApplicationUser?.branchId
                    };

                    await _appointmentService.SaveUserFeedback(data);
                }
                else
                {
                    message = "phoneInvalid";
                }

            }
            else
            {
                message = "error";
            }


            return Json(message);
        }




        #endregion

        #region Get Total Appointment
        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentData(string fDate, string tDate, int branchId)
        {
            var model = await _appointmentService.GetAllAppointmentData(fDate, tDate, branchId);

            return Json(model);
        }

        public async Task<IActionResult> GetAllAppointmentExcelData(string fDate, string tDate, int branchId)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = fDate;
                ViewBag.EndDate = tDate;
                string user = HttpContext.User.Identity.Name;

                var model = await _appointmentService.GetAllAppointmentData(fDate, tDate, branchId);

                var worksheet = workbook.Worksheets.Add("Doctor Appointment Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                //worksheet.Style.Font.SetBold().Font.FontSize = 11;
                worksheet.Cell(currentRow, 1).Value = "Patient Info";
                worksheet.Cell(currentRow, 2).Value = "Appointment Info";
                worksheet.Cell(currentRow, 3).Value = "Appointment Date/time";
                worksheet.Cell(currentRow, 4).Value = "Appointment Status";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = data.doctorname;
                    worksheet.Cell(currentRow2, 3).Value = data.date;
                    worksheet.Cell(currentRow2, 4).Value = data.appointStatus;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.mobile;
                    worksheet.Cell(currentRow2, 2).Value = data.departmentName;
                    worksheet.Cell(currentRow2, 3).Value = data.time;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.gender;
                    worksheet.Cell(currentRow2, 2).Value = data.date;
                    currentRow2++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DoctorAppointment.xlsx");
                }
            }
        }


        public async Task<IActionResult> GetAllAppointmentExcelxlsData(string fDate, string tDate, int branchId)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = fDate;
                ViewBag.EndDate = tDate;
                string user = HttpContext.User.Identity.Name;

                var model = await _appointmentService.GetAllAppointmentData(fDate, tDate, branchId);

                var worksheet = workbook.Worksheets.Add("Doctor Appointment Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                //worksheet.Style.Font.SetBold().Font.FontSize = 11;
                worksheet.Cell(currentRow, 1).Value = "Patient Info";
                worksheet.Cell(currentRow, 2).Value = "Appointment Info";
                worksheet.Cell(currentRow, 3).Value = "Appointment Date/time";
                worksheet.Cell(currentRow, 4).Value = "Appointment Status";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = data.doctorname;
                    worksheet.Cell(currentRow2, 3).Value = data.date;
                    worksheet.Cell(currentRow2, 4).Value = data.appointStatus;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.mobile;
                    worksheet.Cell(currentRow2, 2).Value = data.departmentName;
                    worksheet.Cell(currentRow2, 3).Value = data.time;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.gender;
                    worksheet.Cell(currentRow2, 2).Value = data.date;
                    currentRow2++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DoctorAppointment.xls");
                }
            }
        }

        public async Task<IActionResult> GetAllAppointmentExcelcvsData(string fDate, string tDate, int branchId)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = fDate;
                ViewBag.EndDate = tDate;
                string user = HttpContext.User.Identity.Name;

                var model = await _appointmentService.GetAllAppointmentData(fDate, tDate, branchId);

                var worksheet = workbook.Worksheets.Add("Doctor Appointment Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                //worksheet.Style.Font.SetBold().Font.FontSize = 11;
                worksheet.Cell(currentRow, 1).Value = "Patient Info";
                worksheet.Cell(currentRow, 2).Value = "Appointment Info";
                worksheet.Cell(currentRow, 3).Value = "Appointment Date/time";
                worksheet.Cell(currentRow, 4).Value = "Appointment Status";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = data.doctorname;
                    worksheet.Cell(currentRow2, 3).Value = data.date;
                    worksheet.Cell(currentRow2, 4).Value = data.appointStatus;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.mobile;
                    worksheet.Cell(currentRow2, 2).Value = data.departmentName;
                    worksheet.Cell(currentRow2, 3).Value = data.time;
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.gender;
                    worksheet.Cell(currentRow2, 2).Value = data.date;
                    currentRow2++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DoctorAppointment.csv");
                }
            }
        }



        [HttpGet]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var data = await _appointmentService.DeleteAppointment(id);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(AppointmentListViewModel model)
        {
            var data = await _appointmentService.UpdateAppointment(model);

            return Json(data);
        }

        #endregion

        #region Get Today Appointment
        public async Task<IActionResult> TodayAppointment()
        {
            var username = User.Identity.Name;

            var userInfo = await _userInfoService.GetBotInfoByUserName(username);

            ViewBag.botkey = userInfo.botKey;
            ViewBag.branchInfoId = userInfo.branchInfoId;

            AppointmentListViewModel model = new AppointmentListViewModel
            {
                patientInfoViewModels = await _userInfoService.GetAppointmentList()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetTodayAppointmentData(string date, int branchId)
        {
            var model = await _appointmentService.GetTodayAppointmentData(date, branchId);

            return Json(model);
        }

        #endregion


        #region Get Doctor Data
        public async Task<IActionResult> Doctor()
        {
            var username = User.Identity.Name;

            var userInfo = await _userInfoService.GetBotInfoByUserName(username);

            ViewBag.botkey = userInfo.botKey;
            ViewBag.branchInfoId = userInfo.branchInfoId;
            DoctorVM model = new DoctorVM
            {
                doctorInfoList = await _userInfoService.GetDoctorList(),
                DepartmentList = await _userInfoService.GetDepartmentList(),
                doctorSpecializationList = await _userInfoService.GetDoctorSpecializationList(),
                //botRackInfoMasters= await _userInfoService.GetBotRackInfo(),
            };
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctorData(int branchId)
        {
            //var model = await _appointmentService.GetDoctorList();
            var model = await _appointmentService.GetDoctorInfoList(branchId);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var data = await _appointmentService.DeleteDoctor(id);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDoctor(DoctorVM model)
        {

            var data = await _appointmentService.SaveDoctor(model);

            return Json(data);
        }

        #endregion
        #region Get Department Data
        public async Task<IActionResult> Department()
        {
            var username = User.Identity.Name;

            var userInfo = await _userInfoService.GetBotInfoByUserName(username);

            ViewBag.botkey = userInfo.botKey;
            ViewBag.branchInfoId = userInfo.branchInfoId;

            DepartmentVM model = new DepartmentVM
            {
                departmentInfos = await _userInfoService.GetDepartmentList()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentData(int branchId)
        {
            //var model = await _userInfoService.GetDepartmentList();
            var model = await _userInfoService.GetDepartmentDataList(branchId);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var data = await _appointmentService.DeleteDepartment(id);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartment(DepartmentVM model)
        {
            var data = await _appointmentService.UpdateDepartment(model);

            return RedirectToAction("Department");


        }

        [HttpPost]
        public async Task<IActionResult> SaveDepartment(DepartmentVM model)
        {
            string message = "success";
            if (model.departmentName != null)
            {
                var data = new DepartmentInfo
                {
                    departmentName = model.departmentName,
                    departmentCode = model.departmentCode,
                    shortName = model.shortName,
                    status = model.status,
                    location = model.location,
                    thumbUrl = "https://healthylabs.s3-ap-southeast-1.amazonaws.com/chatbot/department-icon/ANAESTHESIA.png",

                };

                _appointmentService.SaveDepartment(data);
            }
            else
            {
                message = "error";
            }


            return Json(message);
        }



        #endregion

        #region Get Doctor List
        public async Task<IActionResult> FeedBack()
        {
            var username = User.Identity.Name;
            UserFeedbackViewModel model = new UserFeedbackViewModel
            {
                Feedbacks = await _appointmentService.GetAllPendingFeedbackData(username)
            };
            return View(model);

        }

        public async Task<IActionResult> RepliedFeedback()
        {
            var username = User.Identity.Name;

            UserFeedbackViewModel model = new UserFeedbackViewModel
            {
                Feedbacks = await _appointmentService.GetAllRepliedFeedbackData(username)
            };
            return View(model);

        }

        public async Task<IActionResult> Queries()
        {
            var username = User.Identity.Name;

            UserFeedbackViewModel model = new UserFeedbackViewModel
            {
                Queries = await _appointmentService.GetAllPendingQueryData(username)


            };
            //var model = await _appointmentService.GetAllPendingQueryDataList();
            return View(model);

        }

        public async Task<IActionResult> RepliedQuery()
        {
            var username = User.Identity.Name;

            UserFeedbackViewModel model = new UserFeedbackViewModel
            {
                Queries = await _appointmentService.GetAllRepliedQueryData(username)

            };
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingQueryData()
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllPendingQueryDataList(username);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRepliedQueryData()
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllRepliedQueryData(username);

            return Json(model.OrderByDescending(x => x.entryDate));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingFeedbackData()
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllPendingFeedbackData(username);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRepliedFeedbackData()
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllRepliedFeedbackData(username);

            return Json(model);
        }

        // Query Data
        [HttpGet]
        public async Task<IActionResult> GetAllPendingQueryData2(string frmDate, string toDate)
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllPendingQueryDataList2(username, frmDate, toDate);

            return Json(model);
        }
        public async Task<IActionResult> QueryDataExcel(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;

                var model = await _appointmentService.GetAllPendingQueryDataList2(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Query Report");
                var currentRow = 1;

                #region Header 
                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Mobile NUmber";
                worksheet.Cell(currentRow, 3).Value = "Query Date";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";

                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.querydate + " " + data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = data.query;

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Panding Query.xlsx");
                }
            }
        }

        public async Task<IActionResult> QueryDataExcelxls(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllPendingQueryDataList2(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Query Report");
                var currentRow = 1;

                #region Header 
                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Mobile NUmber";
                worksheet.Cell(currentRow, 3).Value = "Query Date";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";

                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.querydate + " " + data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = data.query;

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Panding Query.xls");
                }
            }
        }
        public async Task<IActionResult> QueryDataExcelCsv(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllPendingQueryDataList2(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Query Report");
                var currentRow = 1;

                #region Header 
                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Mobile NUmber";
                worksheet.Cell(currentRow, 3).Value = "Query Date";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";

                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.querydate + " " + data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = data.query;

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Panding Query.csv");
                }
            }
        }



        //Replied Query Data

        [HttpGet]
        public async Task<IActionResult> GetAllRepliedQueryData2(string frmDate, string toDate)
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllRepliedPendingQueryDataList(username, frmDate, toDate);

            return Json(model);
        }
        public async Task<IActionResult> RepliedQueryDataExcel(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllRepliedPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Replied Query Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Query Date";
                worksheet.Cell(currentRow, 3).Value = "Query Time";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Replied Message";
                currentRow++;
                #endregion

                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = data.querydate;
                    worksheet.Cell(currentRow2, 3).Value = data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = "'" + data.query;
                    worksheet.Cell(currentRow2, 6).Value = "'" + data.replyText;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Query.xlsx");
                }
            }
        }

        public async Task<IActionResult> RepliedQueryDataExcelxls(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllRepliedPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Replied Query Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Query Date";
                worksheet.Cell(currentRow, 3).Value = "Query Time";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Replied Message";
                currentRow++;
                #endregion

                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = data.querydate;
                    worksheet.Cell(currentRow2, 3).Value = data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = "'" + data.query;
                    worksheet.Cell(currentRow2, 6).Value = "'" + data.replyText;
                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Panding Query.xls");
                }
            }
        }
        public async Task<IActionResult> RepliedQueryDataExcelCsv(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllRepliedPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Query Report");
                var currentRow = 1;

                #region Header

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "UHID";
                worksheet.Cell(currentRow, 2).Value = "Query Date";
                worksheet.Cell(currentRow, 3).Value = "Query Time";
                worksheet.Cell(currentRow, 4).Value = "Email";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Replied Message";
                currentRow++;
                #endregion

                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = "'" + data.uhid;
                    worksheet.Cell(currentRow2, 2).Value = data.querydate;
                    worksheet.Cell(currentRow2, 3).Value = data.querytime;
                    worksheet.Cell(currentRow2, 4).Value = data.email;
                    worksheet.Cell(currentRow2, 5).Value = "'" + data.query;
                    worksheet.Cell(currentRow2, 6).Value = "'" + data.replyText;
                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Query.csv");
                }
            }
        }


        // FeedbackData

        [HttpGet]
        public async Task<IActionResult> GetAllLoadPendingFeedbackData(string frmDate, string toDate)
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllLoadPendingQueryDataList(username, frmDate, toDate);

            return Json(model);
        }
        public async Task<IActionResult> FeedbackQueryDataExcel(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {

                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllLoadPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                    //worksheet.Cell(currentRow2, 6).Value = data.replyText;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Panding Feedback.xlsx");
                }
            }
        }

        public async Task<IActionResult> FeedbackQueryDataExcelxls(string frmDate, string toDate)
        {


            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllLoadPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Feedback Query.xls");
                }
            }
        }
        public async Task<IActionResult> FeedbackQueryDataExcelCsv(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;


                var model = await _appointmentService.GetAllLoadPendingQueryDataList(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Feedback Query.csv");
                }
            }
        }

        //Replied FeedbackData

        [HttpGet]
        public async Task<IActionResult> GetAllLoadFeedbackData(string frmDate, string toDate)
        {
            var username = User.Identity.Name;
            var model = await _appointmentService.GetAllLoadFeedbackData(username, frmDate, toDate);

            return Json(model);
        }
        public async Task<IActionResult> RepliedFeedbackQueryDataExcel(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllLoadFeedbackData(user, frmDate, toDate);



                var worksheet = workbook.Worksheets.Add("Replied Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Reply Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                    worksheet.Cell(currentRow2, 6).Value = data.replyText;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Feedback.xlsx");
                }
            }
        }

        public async Task<IActionResult> RepliedFeedbackQueryDataExcelxls(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllLoadFeedbackData(user, frmDate, toDate);

                var worksheet = workbook.Worksheets.Add("Replied Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Reply Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                    worksheet.Cell(currentRow2, 6).Value = data.replyText;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Feedback.xls");
                }
            }
        }
        public async Task<IActionResult> RepliedFeedbackQueryDataExcelcsv(string frmDate, string toDate)
        {

            using (var workbook = new XLWorkbook())
            {
                ViewBag.StartDate = frmDate;
                ViewBag.EndDate = toDate;
                string user = HttpContext.User.Identity.Name;
                var model = await _appointmentService.GetAllLoadFeedbackData(user, frmDate, toDate);


                var worksheet = workbook.Worksheets.Add("Replied Feedback Report");
                var currentRow = 1;

                #region Header 

                worksheet.ColumnWidth = 30;
                worksheet.Cell(currentRow, 1).Value = "Name";
                worksheet.Cell(currentRow, 2).Value = "Mobile";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "Query Message";
                worksheet.Cell(currentRow, 6).Value = "Reply Message";
                currentRow++;



                #endregion
                var currentRow2 = 1;
                foreach (var data in model)
                {
                    currentRow2++;
                    worksheet.Cell(currentRow2, 1).Value = data.name;
                    worksheet.Cell(currentRow2, 2).Value = "'" + data.phone;
                    worksheet.Cell(currentRow2, 3).Value = data.email;
                    worksheet.Cell(currentRow2, 4).Value = data.entryDate;
                    worksheet.Cell(currentRow2, 5).Value = data.message;
                    worksheet.Cell(currentRow2, 6).Value = data.replyText;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Replied Feedback.csv");
                }
            }
        }


        #endregion

        #region Doctor Time Slot
        public async Task<IActionResult> DoctorTimeSlot()
        {
            var username = User.Identity.Name;

            var data = new DoctorTimeSlotVm
            {
                TimePeriods = await _appointmentService.GetAllTimeSlots(username),
                DoctorInfos = await _appointmentService.GetDoctorInfos(username)
            };
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeSlotsByDoctorId(int doctorId)
        {
            var DoctorTimeSlots = await _appointmentService.GetTimeSlotsByDoctorId(doctorId);
            return Json(DoctorTimeSlots);
        }

        [HttpPost]
        public async Task<IActionResult> SaveDoctorTimeSlot(DoctorTimeSlotVm model)
        {
            if (model.doctorId > 0)
            {
                var doctor = await _context.DoctorInfos.Where(x => x.Id == model.doctorId).AsNoTracking().FirstOrDefaultAsync();

                await _appointmentService.RemoveAllTimeSlotByDoctorId(model.doctorId);

                if (model.Sunday != null)
                {
                    for (int i = 0; i < model.Sunday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Sunday[i],
                            status = 1,
                            weeksId = 1,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }


                if (model.Monday != null)
                {
                    for (int i = 0; i < model.Monday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Monday[i],
                            status = 1,
                            weeksId = 2,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }

                if (model.Tuesday != null)
                {
                    for (int i = 0; i < model.Tuesday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Tuesday[i],
                            status = 1,
                            weeksId = 3,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId,
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }

                if (model.Wednesday != null)
                {
                    for (int i = 0; i < model.Wednesday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Wednesday[i],
                            status = 1,
                            weeksId = 4,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId,
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }

                if (model.Thursday != null)
                {
                    for (int i = 0; i < model.Thursday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Thursday[i],
                            status = 1,
                            weeksId = 5,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId,
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }


                if (model.Friday != null)
                {
                    for (int i = 0; i < model.Friday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Friday[i],
                            status = 1,
                            weeksId = 6,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId,
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }

                if (model.Saturday != null)
                {
                    for (int i = 0; i < model.Saturday.Length; i++)
                    {
                        var data = new DoctorVisitTimePeriod
                        {
                            Id = 0,
                            //botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                            entryby = User.Identity.Name,
                            entryDate = DateTime.Now,
                            doctorInfoId = model.doctorId,
                            timePeriodId = model.Saturday[i],
                            status = 1,
                            weeksId = 7,
                            isDelete = 0,
                            botKey = doctor.botKey,
                            branchInfoId = doctor.branchInfoId,
                        };
                        await _appointmentService.SaveDoctorTimeSlots(data);
                    }
                }







                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        #endregion

        #region Get Wrapper Header
        public async Task<IActionResult> WrapperHeader()
        {
            ViewBag.botkey = "20dddd7c-734e-4c80-bbed-fae386e0291e";
            WrapperHeaderVM model = new WrapperHeaderVM
            {
                wrapperHeader = await _userInfoService.GetWrapperHeaderList(),
                wrapperHeaderImg = await _userInfoService.GetWrapperHeaderImageList()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetWrapperHeaderData()
        {
            //var model = await _userInfoService.GetDepartmentList();
            var model = await _userInfoService.GetWrapperHeaderList();

            return Json(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetWrapperHeaderImageData()
        {
            //var model = await _userInfoService.GetDepartmentList();
            var model = await _userInfoService.GetWrapperHeaderImageList();

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteWrapperHeader(int id)
        {
            var data = await _userInfoService.DeletewrapperHeader(id);

            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteWrapperHeaderImg(int id)
        {
            var data = await _userInfoService.DeletewrapperHeaderImg(id);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWrapperHeader(WrapperHeaderVM model)
        {
            var data = await _userInfoService.UpdateWrapperHeader(model);

            return RedirectToAction("WrapperHeader");


        }
        [HttpPost]
        public async Task<IActionResult> SaveWrapperHeaderImg(WrapperHeaderVM model)
        {
            var data = await _userInfoService.UpdateWrapperHeaderImg(model);

            return RedirectToAction("WrapperHeader");


        }



        #endregion




        [HttpGet]
        public async Task<IActionResult> GetAllApiDepartmentData()
        {
            var model = await _appointmentService.GetAllApiDeparmentDataList();

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApiDoctorData()
        {
            var model = await _appointmentService.GetAllApiDoctorDataList();

            return Json(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllApiDoctorDataBySpecialisation(string specialisation)
        {
            var model = await _appointmentService.GetAllApiDoctorDataListWithSpecialisation(specialisation);

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> InActiveApiDoctorData(string specialisation)
        {
            var model = await _appointmentService.InActiveApiDoctorDataList(specialisation);

            return Json(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllApiSpecialisationData()
        {
            var model = await _appointmentService.GetAllApiSpecialisationDataList();

            return Json(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteApiDepartment(int id)
        {
            var data = await _appointmentService.DeleteApiDepartmentById(id);

            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteApiDoctor(int id)
        {
            var data = await _appointmentService.DeleteApiDoctorById(id);

            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteApiSpecialisation(int id)
        {
            var data = await _appointmentService.DeleteApiSpecialisationById(id);

            return Json(data);
        }

    }
}