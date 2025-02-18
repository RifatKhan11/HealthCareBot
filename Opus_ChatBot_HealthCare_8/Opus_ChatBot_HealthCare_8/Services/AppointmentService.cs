using System.Collections.Immutable;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.Dapper.IInterfaces;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapper _dapper;

        public AppointmentService(ApplicationDbContext context, IDapper dapper)
        {
            _context = context;
            this._dapper = dapper;
        }

        public bool SaveUserAppointment(AppoinmentInfo model)
        {
            try
            {
                if (model.Id != null)
                {

                    _context.Update(model);
                }
                else
                {

                    _context.Add(model);
                }

                if (_context.SaveChanges() == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<int> SaveUserFeedback(UserFeedback model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.UserFeedbacks.Update(model);
                }
                else
                {

                    _context.UserFeedbacks.Add(model);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public async Task<int> SaveUserQuery(UserQuery model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.UserQueries.Update(model);
                }
                else
                {

                    _context.UserQueries.Add(model);
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public async Task<AppoinmentInfo> GetAppoinmentInfoByUserId(string userId)
        {
            //var data = await _context.AppoinmentInfos.Include(x => x.doctorInfo.menu).Include(x => x.userInfo).Where(x => x.userInfoId == userId).FirstOrDefaultAsync();
            var data = await _context.AppoinmentInfos.Include(x => x.userInfo).Where(x => x.userInfoId == userId).FirstOrDefaultAsync();
            return data;
        }

        public async Task<AppointmentViewModel> GetBasicAppoinmentInfoByUserId(string userId)
        {
            var result = await (from a in _context.AppoinmentInfos
                                join u in _context.UserInfos on a.userInfoId equals u.Id
                                join d in _context.DoctorInfos on a.doctorInfoId equals d.Id
                                join m in _context.Menus on d.menuId equals m.Id
                                where u.Id == userId
                                select new AppointmentViewModel
                                {
                                    date = Convert.ToDateTime(a.date).ToString("dd-MM-yyyy"),
                                    time = a.time,
                                    departmentName = m.MenuNameEN,
                                    designationName = d.designationName,
                                    doctorName = d.name,
                                    mobile = u.Mobile,
                                    patientName = u.FullName,
                                    UHID = u.UHID,
                                    bookingNo = a.bookingNo
                                }).FirstOrDefaultAsync();
            //var data = await _context.AppoinmentInfos.Include(x => x.doctorInfo.menu).Include(x => x.userInfo).Where(x => x.userInfoId == userId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<DoctorVisitTimePeriod>> GetDoctorVisitTimeByDoctorId(int doctorId)
        {
            var data = await _context.DoctorVisitTimePeriods.Include(x => x.timePeriod).Where(x => x.doctorInfoId == doctorId).ToListAsync();
            return data;
        }

        public async Task<List<TimePeriod>> GetDoctorVisitTimeListByDoctorId(int doctorId)
        {
            var result = await (from d in _context.DoctorVisitTimePeriods
                                join t in _context.TimePeriods on d.timePeriodId equals t.Id
                                where d.doctorInfoId == doctorId
                                select t).ToListAsync();
            //var data = await _context.DoctorVisitTimePeriods.Include(x => x.timePeriod).Where(x => x.doctorInfoId == doctorId).ToListAsync();
            return result;
        }

        #region Dashbaord

        public async Task<List<DashboardListVM>> GetDepartmentInforamtionWithStatus(string botKey)
        {
            try
            {
                //var result = await _context.DashboardListVMSPModels.FromSql($"SP_DashboardCount {botKey}").ToListAsync();
                var result = await _dapper.FromSqlAsync<DashboardListVM>($"SP_DashboardCount '{botKey}'");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
        #region Get Appountment data

        public async Task<List<AppointmentVM>> GetAllAppointmentData(string fDate, string tDate, int branchId)
        {
            try
            {
                var fromD = Convert.ToDateTime(fDate);
                var to = Convert.ToDateTime(tDate);
                var result = await (from a in _context.AppoinmentInfos
                                    join u in _context.UserInfos on a.userInfoId equals u.Id
                                    //join d in _context.DoctorInfos on a.doctorInfoId equals d.Id
                                    where Convert.ToDateTime($"{Convert.ToDateTime(a.date).ToString("yyyy-MM-dd")} {a.time}") >= fromD && a.date <= to && a.isDelete != 1 && a.branchInfoId == branchId
                                    orderby DateTime.ParseExact(Convert.ToDateTime(a.date).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(a.time).ToString("HH:mm"), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ascending
                                    select new AppointmentVM
                                    {
                                        id = a.Id,
                                        date = Convert.ToDateTime(a.date).ToString("yyyy-MM-dd"),
                                        time = Convert.ToDateTime(a.time).ToString("HH:mm"),
                                        doctorname = a.doctorName,
                                        departmentName = a.departmentName,
                                        mobile = u.Mobile,
                                        UHID = u.UHID,
                                        gender = u.gender,
                                        name = u.FullName,
                                        dob = u.dateOfBirth,
                                        status = a.status == 0 ? "Pending" : a.status == 1 ? "Passed" : "end",
                                        isVerified = a.isVerified == 1 ? "Yes" : "No",
                                        appointStatus = a.appointStatus,
                                    }).ToListAsync();

                int count = await (from a in _context.AppoinmentInfos
                                   where a.date >= DateTime.Now.Date && (a.status == 0 || a.status == 1)
                                   select a).CountAsync();


                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public async Task<List<AppointmentVM>> GetAllAppointmentData(string fDate, string tDate, int branchId)
        //{
        //    try
        //    {
        //        var fromD = Convert.ToDateTime(fDate);
        //        var to = Convert.ToDateTime(tDate); 
        //        var result = await (from a in _context.AppoinmentInfos
        //                            join u in _context.UserInfos on a.userInfoId equals u.Id
        //                            join d in _context.DoctorInfos on a.doctorInfoId equals d.Id

        //        where Convert.ToDateTime($"{Convert.ToDateTime(a.date).ToString("yyyy-MM-dd")} {a.time}") >= fromD && a.date <= to && a.isDelete != 1 && a.branchInfoId == branchId


        //                            select new AppointmentVM
        //                            {
        //                                id = a.Id,
        //                                date = Convert.ToDateTime(a.date).ToString("yyyy-MM-dd"),
        //                                time = a.time,
        //                                doctorname = d.name,
        //                                departmentName = d.departmentName,
        //                                mobile = u.Mobile,
        //                                UHID = u.UHID,
        //                                gender = u.gender,
        //                                name = u.FullName,
        //                                dob = u.dateOfBirth,
        //                                status = a.status == 0 ? "Pending" : a.status == 1 ? "Passed" : "end",
        //                                isVerified = a.isVerified == 1 ? "Yes" : "No",
        //                                appointStatus = a.appointStatus, 
        //                            }).OrderBy(x => x.date).ToListAsync();

        //        int count = await (from a in _context.AppoinmentInfos
        //                           where a.date >= DateTime.Now.Date && (a.status == 0 || a.status == 1)
        //                           select a).CountAsync();


        //        return result;


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}


        public async Task<List<AppointmentVM>> GetTodayAppointmentData(string date, int branchId)
        {
            try
            {
                var result = await (from a in _context.AppoinmentInfos
                                    join u in _context.UserInfos on a.userInfoId equals u.Id
                                    //join d in _context.DoctorInfos on a.doctorInfoId equals d.Id
                                    //where a.date >= DateTime.Now.Date
                                    where a.date == DateTime.Now.Date && a.branchInfoId == branchId
                                    orderby a.date, a.time
                                    select new AppointmentVM
                                    {
                                        id = a.Id,
                                        date = Convert.ToDateTime(a.date).ToString("yyyy-MM-dd"),
                                        time = a.time,
                                        doctorname = a.doctorName,
                                        departmentName = a.departmentName,
                                        mobile = u.Mobile,
                                        UHID = u.UHID,
                                        gender = u.gender,
                                        name = u.FullName,
                                        dob = u.dateOfBirth,
                                        status = a.status == 0 ? "Pending" : a.status == 1 ? "Passed" : "end",
                                        isVerified = a.isVerified == 1 ? "Yes" : "No",
                                        appointStatus = a.appointStatus
                                    }).ToListAsync();
                return result;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public async Task<int> DeleteAppointment(int id)
        {
            var data = await _context.AppoinmentInfos.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            _context.AppoinmentInfos.Remove(data);

            await _context.SaveChangesAsync();

            return 1;
        }

        public async Task<AppoinmentInfo> UpdateAppointment(AppointmentListViewModel model)
        {
            var data = await _context.AppoinmentInfos.Where(x => x.Id == model.id).AsNoTracking().FirstOrDefaultAsync();
            if (model.status == "Rejected")
            {
                data.date = Convert.ToDateTime(model.frmDate);
                data.time = model.time;
                data.appointStatus = model.status;
                data.status = 6;
                _context.AppoinmentInfos.Update(data);
            }
            else if (model.status == "Called")
            {
                data.date = Convert.ToDateTime(model.frmDate);
                data.time = model.time;
                data.appointStatus = model.status;
                data.status = 5;
                _context.AppoinmentInfos.Update(data);
            }
            else if (model.status == "Scheduled")
            {
                data.date = Convert.ToDateTime(model.frmDate);
                data.time = model.time;
                data.appointStatus = model.status;
                data.status = 4;
                _context.AppoinmentInfos.Update(data);
            }
            else
            {
                data.date = Convert.ToDateTime(model.frmDate);
                data.time = model.time;
                data.appointStatus = model.status;
                _context.AppoinmentInfos.Update(data);
            }



            await _context.SaveChangesAsync();

            return data;
        }


        #endregion

        #region Get Appountment data
        public async Task<IEnumerable<AppointmentVM>> GetTotalAppointmentData(string date, string botKey)
        {
            try
            {
                var user = await _context.ChatbotInfos.Where(x => x.botKey == botKey).Select(x => x.ApplicationUser).AsNoTracking().FirstOrDefaultAsync();



                var result = await (
                    from a in _context.AppoinmentInfos
                    join u in _context.UserInfos on a.userInfoId equals u.Id
                    join d in _context.DoctorInfos on a.doctorInfoId equals d.Id
                    where a.branchInfoId == user.branchId
                    select new AppointmentVM
                    {
                        id = a.Id,
                        date = Convert.ToDateTime(a.date).ToString("dd-MM-yyyy"),
                        time = a.time,
                        doctorname = d.name,
                        departmentName = d.departmentName,
                        mobile = u.Mobile,
                        UHID = u.UHID,
                        gender = u.gender
                    }
                ).ToListAsync();

                var groupedResult = result
                    .GroupBy(item => item.date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Count = group.Count()
                    }).Select(item => new AppointmentVM
                    {
                        date = item.Date,
                        totalAppointment = item.Count,
                    }).ToList();

                return groupedResult;




            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region Doctor Data
        public async Task<List<DoctorInfo>> GetDoctorList()
        {
            var data = await _context.DoctorInfos.Include(x => x.Department).Where(x => x.isDelete != 1).ToListAsync();

            return data;
        }

        public async Task<List<DoctorViewModel>> GetDoctorInfoList(int branchId)
        {
            try
            {
                //var result = await _context.DoctorVMs.FromSql($"GetDoctorData {branchId}").ToListAsync();
                var result = await _dapper.FromSqlAsync<DoctorViewModel>($"GetDoctorData {branchId}");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //try
            //{
            //    var result = await (from d in _context.DoctorInfos                                     
            //                        join de in _context.DepartmentInfos on d.DepartmentId equals de.Id
            //                        join ds in _context.DoctorSpecializations on d.doctorSpecializationId equals ds.Id
            //                        join u in _context.AppoinmentInfos on d.branchInfo equals u.branchInfo
            //                        where (d.isDelete == null || d.isDelete == 0)  
            //                        select new DoctorVM
            //                        {
            //                            Id = d.Id,
            //                            name=   d.name,
            //                            designationName = d.designationName,
            //                            details = d.details, 
            //                            gender= d.gender,
            //                            departmentName = d.departmentName, 
            //                            doctorSpecialization = ds.name,
            //                            doctorstatus = d.status == 1 ? "Active" : "InActive",
            //                            status = d.status,
            //                        }).ToListAsync();
            //    return result;


            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

        }

        public async Task<DoctorInfo> SaveDoctor(DoctorVM model)
        {
            var department = await _context.DepartmentInfos.Where(x => x.Id == model.DepartmentId).AsNoTracking().FirstOrDefaultAsync();

            var botKnowledge = await _context.BotKnowledges.Where(x => x.question == model.name).AsNoTracking().ToListAsync();
            //var user = await _context.
            var keywordQuesAns = await _context.keyWordQuesAns.Where(x => x.question == model.name).AsNoTracking().ToListAsync();
            var keyword = await _context.KeyWordInfos.Where(x => x.nameEn == model.name).AsNoTracking().ToListAsync();
            var servicesFlows = await _context.ServiceFlows.Where(x => x.keyWordQuesAnsId == keywordQuesAns.FirstOrDefault().Id).AsNoTracking().ToListAsync();

            if (botKnowledge.Count() > 0)
            {
                _context.BotKnowledges.RemoveRange(botKnowledge);
                await _context.SaveChangesAsync();
            }

            if (keyword.Count() > 0)
            {
                _context.KeyWordInfos.RemoveRange(keyword);
                await _context.SaveChangesAsync();
            }

            if (servicesFlows.Count() > 0)
            {
                _context.ServiceFlows.RemoveRange(servicesFlows);
                await _context.SaveChangesAsync();
            }

            if (keywordQuesAns.Count() > 0)
            {
                _context.keyWordQuesAns.RemoveRange(keywordQuesAns);
                await _context.SaveChangesAsync();
            }

            //var data = await _context.DoctorInfos.Where(x => x.name == model.name || x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            var data = await _context.DoctorInfos.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();

            if (data != null)
            {
                data.Id = model.Id;
                data.name = model.name;
                data.doctorCode = model.doctorCode;
                data.gender = model.gender;
                data.details = model.details;
                data.status = model.status;
                data.DepartmentId = model.DepartmentId;
                data.departmentName = department?.departmentName;
                data.designationName = model.designationName;
                data.doctorSpecializationId = model.doctorSpecializationId;
                data.botKey = model.botkey;
                data.entryby = "mehedi";
                data.entryDate = DateTime.Now;
                data.isDelete = 0;
                data.branchInfoId = model.branchId;

                _context.DoctorInfos.Update(data);
                await _context.SaveChangesAsync();


                var keyData = new KeyWordQuesAns
                {
                    Id = 0,
                    facebookPageId = 2,
                    question = model.name,
                    answer = model.name,
                    priority = 1,
                    IsLoop = 1,
                    botKey = model.botkey,
                    questionKey = Guid.NewGuid().ToString(),
                    type = 101,
                    questionOrder = 1,
                    status = 1,
                    isQuestion = 0,
                    doctorId = data.Id,
                    isDelete = 0,
                    keyText = model.name,
                    questionCategoryId = 1,
                    keyWord = model.name,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,

                };

                _context.keyWordQuesAns.Add(keyData);
                await _context.SaveChangesAsync();

                var keywordData = new KeyWordInfo
                {
                    Id = 0,
                    botKey = model.botkey,
                    nameEn = model.name,
                    status = 1,
                    KeyWordQuesAnsId = keyData.Id,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };
                _context.KeyWordInfos.Add(keywordData);
                await _context.SaveChangesAsync();

                var knowledgeData = new BotKnowledge
                {
                    Id = 0,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    question = model.name,
                    textReply = model.name,
                    status = 1,
                    keyWordQuesAnsId = keyData.Id,
                    branchInfoId = model.branchId,
                };
                _context.BotKnowledges.Add(knowledgeData);
                await _context.SaveChangesAsync();

                return data;
            }
            else
            {
                var doctorInfo = new DoctorInfo()
                {
                    name = model.name,
                    doctorCode = model.doctorCode,
                    gender = model.gender,
                    details = model.details,
                    status = model.status,
                    DepartmentId = model.DepartmentId,
                    departmentName = department?.departmentName,
                    designationName = model.designationName,
                    doctorSpecializationId = model.doctorSpecializationId,
                    botKey = model.botkey,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    isDelete = 0,
                    branchInfoId = model.branchId,

                };

                _context.DoctorInfos.Add(doctorInfo);
                await _context.SaveChangesAsync();

                var keyData = new KeyWordQuesAns
                {
                    Id = 0,
                    facebookPageId = 2,
                    question = model.name,
                    answer = model.name,
                    priority = 1,
                    IsLoop = 1,
                    botKey = model.botkey,
                    questionKey = Guid.NewGuid().ToString(),
                    type = 101,
                    questionOrder = 1,
                    status = 1,
                    isQuestion = 0,
                    doctorId = doctorInfo.Id,
                    isDelete = 0,
                    keyText = model.name,
                    questionCategoryId = 1,
                    keyWord = model.name,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };

                _context.keyWordQuesAns.Add(keyData);
                await _context.SaveChangesAsync();

                var keywordData = new KeyWordInfo
                {
                    Id = 0,
                    botKey = model.botkey,
                    nameEn = model.name,
                    status = 1,
                    KeyWordQuesAnsId = keyData.Id,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };
                _context.KeyWordInfos.Add(keywordData);
                await _context.SaveChangesAsync();

                var knowledgeData = new BotKnowledge
                {
                    Id = 0,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    question = model.name,
                    textReply = model.name,
                    status = 1,
                    keyWordQuesAnsId = keyData.Id,
                    botKey = model.botkey,
                    branchInfoId = model.branchId,
                };
                _context.BotKnowledges.Add(knowledgeData);
                await _context.SaveChangesAsync();

                return doctorInfo;


            }


        }
        public async Task<int> DeleteDoctor(int id)
        {
            var data = await _context.DoctorInfos.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            data.isDelete = 1;
            _context.DoctorInfos.Update(data);

            await _context.SaveChangesAsync();

            return 1;
        }

        #endregion
        #region query and feedback

        public async Task<List<UserFeedbackViewModel>> GetAllPendingQueryDataList(string username)
        {
            try
            {
                var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();
                var result = await (from a in _context.UserQueries
                                    where a.replied == 0 && a.branchInfoId == user.branchId
                                    select new UserFeedbackViewModel
                                    {
                                        uhid = a.uhid,
                                        connectionId = a.connectionId,
                                        email = a.email,
                                        query = a.query,
                                        querydate = a.querydate,
                                        querytime = a.querytime,
                                        replied = a.replied,
                                        queryId = a.Id,
                                        querystatus = a.status,
                                        phone = a.phone,
                                        querystatusType = a.status == 0 ? "Pending" : a.status == 1 ? "Called" : "Closed",

                                    }).OrderByDescending(x => x.queryId).ToListAsync();
                return result;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserFeedbackQueryViewModel>> GetAllPendingQueryDataList2(string username, string fdate, string tdate)
        {
            try
            {
                //var result = await _context.UserFeedbackQueryViewModels.FromSql($"GetUserFeedbackData {username},{fdate},{tdate}").OrderByDescending(x => x.queryId).ToListAsync();
                var result = await _dapper.FromSqlAsync<UserFeedbackQueryViewModel>($"GetUserFeedbackData '{username}','{fdate}','{tdate}'");

                return result.OrderByDescending(x=>x.queryId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserFeedbackQueryViewModel>> GetAllRepliedPendingQueryDataList(string username, string fdate, string tdate)
        {
            try
            {
                //var result = await _context.UserFeedbackQueryViewModels.FromSql($"GetRepliedQuery {username},{fdate},{tdate}").OrderByDescending(x => x.queryId).ToListAsync();
                var result = await _dapper.FromSqlAsync<UserFeedbackQueryViewModel>($"GetRepliedQuery '{username}','{fdate}','{tdate}'");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserFeedback>> GetAllLoadPendingQueryDataList(string username, string fdate, string tdate)
        {
            try
            {
                //var result = await _context.UserFeedbacks.FromSql($"GetFeedbackData {username},{fdate},{tdate}").ToListAsync();
                var result = await _dapper.FromSqlAsync<UserFeedback>($"GetFeedbackData '{username}','{fdate}','{tdate}'");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<UserFeedback>> GetAllLoadFeedbackData(string username, string fdate, string tdate)
        {
            try
            {
                //var result = await _context.UserFeedbacks.FromSql($"GetRepliedFeedbackData  {username},{fdate},{tdate}").ToListAsync();
                var result = await _dapper.FromSqlAsync<UserFeedback>($"GetRepliedFeedbackData '{username}','{fdate}','{tdate}'");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<UserQuery>> GetAllPendingQueryData(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.UserQueries.Where(x => x.replied == 0 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<UserQuery>> GetAllRepliedQueryData(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.UserQueries.Where(x => x.replied == 1 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<UserFeedback>> GetAllPendingFeedbackData(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.UserFeedbacks.Where(x => x.replied == 0 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<UserFeedback>> GetAllRepliedFeedbackData(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.UserFeedbacks.Where(x => x.replied == 1 && x.branchInfoId == user.branchId).AsNoTracking().ToListAsync();

            return data;
        }

        public async Task<UserQuery> GetQueryById(int id)
        {
            var data = await _context.UserQueries.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }


        public async Task<UserFeedback> GetFeedbackById(int id)
        {
            var data = await _context.UserFeedbacks.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }

        #endregion
        #region Department Data
        public bool SaveDepartment(DepartmentInfo model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _context.Update(model);
                    _context.SaveChangesAsync();
                }
                else
                {

                    _context.Add(model);
                    if (_context.SaveChanges() == 1) return true;
                }

                //if (_context.SaveChanges() == 1) return true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }




        public async Task<DepartmentInfo> UpdateDepartment(DepartmentVM model)
        {

            var department = await _context.DepartmentInfos.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();

            var botKnowledge = await _context.BotKnowledges.Where(x => x.question == model.departmentName).AsNoTracking().ToListAsync();
            var keywordQuesAns = await _context.keyWordQuesAns.Where(x => x.question == model.departmentName).AsNoTracking().ToListAsync();
            var keyword = await _context.KeyWordInfos.Where(x => x.nameEn == model.departmentName).AsNoTracking().ToListAsync();

            if (botKnowledge != null)
            {
                _context.BotKnowledges.RemoveRange(botKnowledge);
                await _context.SaveChangesAsync();
            }

            if (keyword != null)
            {
                _context.KeyWordInfos.RemoveRange(keyword);
                await _context.SaveChangesAsync();
            }

            if (keywordQuesAns != null)
            {
                _context.keyWordQuesAns.RemoveRange(keywordQuesAns);
                await _context.SaveChangesAsync();
            }

            //var data = await _context.DoctorInfos.Where(x => x.name == model.name || x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            var data = await _context.DepartmentInfos.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            var imagePath = "";

            if (model.fileUrl != null)
            {
                string message = FileSave.SaveFileImagePath(out imagePath, model.fileUrl, "chatbox");
            }
            else
            {
                //imagePath = DepartmentInfo.imgUrl;
            };
            if (data != null)
            {
                data.Id = model.Id;
                data.departmentName = model.departmentName;
                data.departmentCode = model.departmentCode;
                data.thumbUrl = imagePath;
                data.shortName = model.shortName;
                data.botKey = model.botkey;
                data.status = model.status;
                data.entryby = "mehedi";
                data.entryDate = DateTime.Now;
                data.isDelete = 0;
                data.branchInfoId = model.branchId;

                _context.DepartmentInfos.Update(data);
                await _context.SaveChangesAsync();


                var keyData = new KeyWordQuesAns
                {
                    Id = 0,
                    facebookPageId = 2,
                    question = model.departmentName,
                    answer = model.departmentName,
                    priority = 1,
                    IsLoop = 1,
                    botKey = model.botkey,
                    questionKey = Guid.NewGuid().ToString(),
                    type = 101,
                    questionOrder = 1,
                    status = 1,
                    isQuestion = 0,
                    departmentId = data.Id,
                    isDelete = 0,
                    keyText = model.departmentName,
                    questionCategoryId = 1,
                    keyWord = model.departmentName,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };

                _context.keyWordQuesAns.Add(keyData);
                await _context.SaveChangesAsync();

                var keywordData = new KeyWordInfo
                {
                    Id = 0,
                    botKey = model.botkey,
                    nameEn = model.departmentName,
                    status = 1,
                    KeyWordQuesAnsId = keyData.Id,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };
                _context.KeyWordInfos.Add(keywordData);
                await _context.SaveChangesAsync();

                var knowledgeData = new BotKnowledge
                {
                    Id = 0,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    question = model.departmentName,
                    textReply = model.departmentName,
                    status = 1,
                    keyWordQuesAnsId = keyData.Id,
                    branchInfoId = model.branchId
                };
                _context.BotKnowledges.Add(knowledgeData);
                await _context.SaveChangesAsync();

                return data;
            }
            else
            {
                var departmentInfo = new DepartmentInfo()
                {
                    departmentName = model.departmentName,
                    departmentCode = model.departmentCode,
                    shortName = model.shortName,
                    thumbUrl = imagePath,
                    status = model.status,
                    location = model.location,
                    botKey = model.botkey,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    isDelete = 0,
                    branchInfoId = model.branchId,

                };

                _context.DepartmentInfos.Add(departmentInfo);
                await _context.SaveChangesAsync();

                var keyData = new KeyWordQuesAns
                {
                    Id = 0,
                    facebookPageId = 2,
                    question = model.departmentName,
                    answer = model.departmentName,
                    priority = 1,
                    IsLoop = 1,
                    botKey = model.botkey,
                    questionKey = Guid.NewGuid().ToString(),
                    type = 101,
                    questionOrder = 1,
                    status = 1,
                    isQuestion = 0,
                    departmentId = departmentInfo.Id,
                    isDelete = 0,
                    keyText = model.departmentName,
                    questionCategoryId = 1,
                    keyWord = model.departmentName,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };

                _context.keyWordQuesAns.Add(keyData);
                await _context.SaveChangesAsync();

                var keywordData = new KeyWordInfo
                {
                    Id = 0,
                    botKey = model.botkey,
                    nameEn = model.departmentName,
                    status = 1,
                    KeyWordQuesAnsId = keyData.Id,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    branchInfoId = model.branchId,
                };
                _context.KeyWordInfos.Add(keywordData);
                await _context.SaveChangesAsync();

                var knowledgeData = new BotKnowledge
                {
                    Id = 0,
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    question = model.departmentName,
                    textReply = model.departmentName,
                    status = 1,
                    keyWordQuesAnsId = keyData.Id,
                    botKey = model.botkey,
                    branchInfoId = model.branchId,
                };
                _context.BotKnowledges.Add(knowledgeData);
                await _context.SaveChangesAsync();

                return departmentInfo;


            }


        }

        public async Task<int> DeleteDepartment(int id)
        {
            var data = await _context.DepartmentInfos.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            data.isDelete = 1;
            _context.DepartmentInfos.Update(data);



            await _context.SaveChangesAsync();

            return 1;
        }
        #endregion


        public async Task<List<TimePeriod>> GetAllTimeSlots(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.TimePeriods.Where(x => x.branchInfoId == user.branchId).OrderBy(x => x.sortOrder).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<DoctorInfo>> GetDoctorInfos(string username)
        {
            var user = await _context.Users.Where(x => x.UserName == username).AsNoTracking().FirstOrDefaultAsync();

            var data = await _context.DoctorInfos.Where(x => x.branchInfoId == user.branchId).Include(x => x.Department).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<GetTimeSlotsByDoctorIdVm>> GetTimeSlotsByDoctorId(int doctorId)
        {

            try
            {
                //var result = await _context.GetTimeSlotsByDoctorIdVms.FromSql($"SP_GetTimeSlotsByDoctorId {doctorId}").ToListAsync();
                var result = await _dapper.FromSqlAsync<GetTimeSlotsByDoctorIdVm>($"SP_GetTimeSlotsByDoctorId {doctorId}");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<int> RemoveAllTimeSlotByDoctorId(int doctorId)
        {
            var data = await _context.DoctorVisitTimePeriods.Where(x => x.doctorInfoId == doctorId).AsNoTracking().ToListAsync();
            _context.DoctorVisitTimePeriods.RemoveRange(data);
            return await _context.SaveChangesAsync();
        }


        public async Task<int> SaveDoctorTimeSlots(DoctorVisitTimePeriod model)
        {
            _context.DoctorVisitTimePeriods.Add(model);
            return await _context.SaveChangesAsync();
        }

        #region Api Data List
        public async Task<List<ApiListVM>> GetAllApiInsertDataList()
        {
            try
            {
                //var result = await _context.apiListVMs.FromSql($"SP_ApiDataCount").ToListAsync();

                var result = await _dapper.FromSqlAsync<ApiListVM>($"SP_ApiDataCount");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion


        public async Task<IEnumerable<ApiDepartmentData>> GetAllApiDeparmentDataList()
        {
            try
            {
                var user = await _context.ApiDepartmentData.AsNoTracking().ToListAsync();

                return user;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ApiDoctorData>> GetAllApiDoctorDataList()
        {
            try
            {
                var user = await _context.ApiDoctorData.AsNoTracking().ToListAsync();

                return user;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ApiDoctorData>> GetAllApiDoctorDataListWithSpecialisation(string specialisation)
        {
            try
            {
                var user = await _context.ApiDoctorData.Where(x => x.specialization == specialisation).AsNoTracking().ToListAsync();

                return user;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ApiSpecialisationData>> GetAllApiSpecialisationDataList()
        {
            try
            {
                var user = await _context.ApiSpecialisationData.AsNoTracking().ToListAsync();

                return user;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteApiDepartmentById(int id)
        {
            try
            {
                var data = await _context.ApiDepartmentData.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
                data.isDelete = 1;
                _context.ApiDepartmentData.Update(data);



                await _context.SaveChangesAsync();

                return 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<int> DeleteApiDoctorById(int id)
        {
            try
            {
                var data = await _context.ApiDoctorData.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
                data.isDelete = 1;
                _context.ApiDoctorData.Update(data);



                await _context.SaveChangesAsync();

                return 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<int> DeleteApiSpecialisationById(int id)
        {
            try
            {
                var data = await _context.ApiSpecialisationData.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

                _context.ApiSpecialisationData.Update(data);

                data.isDelete = 1;

                await _context.SaveChangesAsync();

                return 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public async Task<IEnumerable<EvercareToken>> GetActiveTokenList()
        {
            var data = await _context.EvercareTokens.AsNoTracking().Where(x => x.isActive == 1).OrderByDescending(x => x.Id).ToListAsync();

            return data;
        }

        public async Task<int> SaveEvercareToken(EvercareToken models)
        {

            models.sentAlert = 1;
            _context.EvercareTokens.Add(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }



        public async Task<IEnumerable<ApiActivityLog>> GetApiActivityLog()
        {
            var data = await _context.ApiActivityLogs.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();

            return data;
        }

        public async Task<int> SaveApiActivityLog(ApiActivityLog models)
        {


            _context.ApiActivityLogs.Add(models);

            var data = await _context.SaveChangesAsync();

            return data;
        }


        public async Task<IEnumerable<ApiDoctorData>> InActiveApiDoctorDataList(string specialisation)
        {
            try
            {
                var user = await _context.ApiDoctorData.Where(x => x.specialization == specialisation).AsNoTracking().ToListAsync();
                foreach (var item in user)
                {
                    item.isDelete = 1;
                }
                _context.UpdateRange(user);
                await _context.SaveChangesAsync();
                return user;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
