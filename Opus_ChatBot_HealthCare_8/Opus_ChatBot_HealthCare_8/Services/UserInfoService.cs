using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly ApplicationDbContext _contex;

        public UserInfoService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public UserInfo GetuserInfo(string CombinedId)
        {
            try
            {
                UserInfo userInfo = _contex.UserInfos.Find(CombinedId);
                if (userInfo != null) return userInfo;
                return new UserInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new UserInfo();
            }
        }

        public UserInfo GetuserInfoByMobile(string mobile)
        {
            try
            {
                UserInfo userInfo = _contex.UserInfos.Where(x => x.Mobile == mobile).FirstOrDefault();
                if (userInfo != null) return userInfo;
                return new UserInfo();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new UserInfo();
            }
        }

        public async Task<IEnumerable<PatientInfoViewModel>> GetAppointmentList()
        {
            var data = await (from u in _contex.UserInfos
                              join a in _contex.AppoinmentInfos on u.Id equals a.userInfoId
                              //join d in _contex.DoctorInfos on a.doctorInfoId equals d.Id
                              //join w in _contex.Weeks on a.weeksId equals w.Id
                              //join m in _contex.Menus on d.menuId equals m.Id
                              //where a.status == 0//Convert.ToDateTime(a.date).ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy")
                              select new PatientInfoViewModel
                              {
                                  dob = u.dateOfBirth,
                                  doctorName = a.doctorName,
                                  name = u.FullName,
                                  mobile = u.Mobile,
                                  gender = u.gender,
                                  weekName = Convert.ToDateTime(a.date).DayOfWeek.ToString(),
                                  email = u.Email,
                                  date = Convert.ToDateTime(a.date).ToString("dd-MM-yyyy") + " " + a.time,
                                  departmentName = a.departmentName,
                                  designationName = a.designationName
                              }).OrderByDescending(x => x.date).ToListAsync();

            return data;
        }


        public bool InitUserInfo(string CombinedId)
        {
            try
            {
                UserInfo userInfo = new UserInfo
                {
                    Id = CombinedId,
                };

                _contex.UserInfos.Add(userInfo);

                if (1 == _contex.SaveChanges()) return true;

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool UpdateUserInfo(string CombinedId, string InfoType, string InfoValue)
        {
            try
            {
                UserInfo userInfo = _contex.UserInfos.Find(CombinedId);
                if (userInfo != null)
                {

                    if (InfoType == "FullName")
                        userInfo.FullName = InfoValue;
                    else if (InfoType == "Email")
                        userInfo.Email = InfoValue;
                    else if (InfoType == "Mobile")
                        userInfo.Mobile = InfoValue;
                    else if (InfoType == "Address")
                        userInfo.Address = InfoValue;
                    else if (InfoType == "Passport")
                        userInfo.passport = InfoValue;
                    else if (InfoType == "otp")
                        userInfo.otpMsg = InfoValue;
                    else if (InfoType == "bankac")
                        userInfo.bankaccountNumber = InfoValue;
                    else if (InfoType == "keyWordQues")
                        userInfo.keyWordQues = InfoValue;
                    else if (InfoType == "UHID")
                        userInfo.UHID = InfoValue;
                    else if (InfoType == "dateOfBirth")
                        userInfo.dateOfBirth = InfoValue;
                    else if (InfoType == "gender")
                        userInfo.gender = InfoValue;
                }
                else
                {
                    userInfo = new UserInfo
                    {
                        Id = CombinedId,
                    };
                    if (InfoType == "FullName")
                        userInfo.FullName = InfoValue;
                    else if (InfoType == "Email")
                        userInfo.Email = InfoValue;
                    else if (InfoType == "Mobile")
                        userInfo.Mobile = InfoValue;
                    else if (InfoType == "Address")
                        userInfo.Address = InfoValue;
                    else if (InfoType == "Passport")
                        userInfo.passport = InfoValue;
                    else if (InfoType == "otp")
                        userInfo.otpMsg = InfoValue;
                    else if (InfoType == "bankac")
                        userInfo.bankaccountNumber = InfoValue;
                    else if (InfoType == "keyWordQues")
                        userInfo.keyWordQues = InfoValue;
                    else if (InfoType == "UHID")
                        userInfo.UHID = InfoValue;
                    else if (InfoType == "dateOfBirth")
                        userInfo.dateOfBirth = InfoValue;
                    else if (InfoType == "gender")
                        userInfo.gender = InfoValue;
                    _contex.Add(userInfo);
                }
                if (_contex.SaveChanges() == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool UpdatePatientInfo(UserInfo userInfo)
        {
            try
            {
                _contex.Add(userInfo);
                if (_contex.SaveChanges() == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool UpdateProfileInfo(PatientInfoViewModel model)
        {
            try
            {
                UserInfo userInfo = _contex.UserInfos.Find(model.pageId + model.nick);
                if (userInfo != null)
                {
                    userInfo.FullName = model.name;
                    userInfo.Mobile = model.mobile;
                    userInfo.Email = model.email;
                    userInfo.gender = model.gender;
                    userInfo.dateOfBirth = model.dob;
                    userInfo.otpMsg = model.otp;
                    userInfo.bankaccountNumber = model.weekId;
                    userInfo.keyWordQues = model.doctorId.ToString();
                    _contex.Update(userInfo);
                }
                else
                {
                    userInfo.FullName = model.name;
                    userInfo.Mobile = model.mobile;
                    userInfo.Email = model.email;
                    userInfo.gender = model.gender;
                    userInfo.dateOfBirth = model.dob;
                    userInfo.otpMsg = model.otp;
                    userInfo.keyWordQues = model.doctorId.ToString();
                    userInfo.bankaccountNumber = model.weekId;
                    _contex.Add(userInfo);
                }

                if (_contex.SaveChanges() == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }




        public async Task<ChatbotInfo> GetBotInfoByUserName(string username)
        {
            var data = await _contex.ChatbotInfos.Where(x => x.ApplicationUser.UserName == username).FirstOrDefaultAsync();
            return data;
        }


        //public async Task<IEnumerable<AppointmentListVM>> GetAppointmentListInfo()
        //{


        //    //var data = await (from u in _contex.UserInfos
        //    //                  join a in _contex.AppoinmentInfos on u.Id equals a.userInfoId
        //    //                  join d in _contex.DoctorInfos on a.doctorInfoId equals d.Id
        //    //                  join w in _contex.Weeks on a.weeksId equals w.Id
        //    //                  join m in _contex.Menus on d.menuId equals m.Id
        //    //                  where a.status == 1//Convert.ToDateTime(a.date).ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy")
        //    //                  select new PatientInfoViewModel
        //    //                  {
        //    //                      dob = u.dateOfBirth,
        //    //                      doctorName = d.name,
        //    //                      name = u.FullName,
        //    //                      mobile = u.Mobile,
        //    //                      gender = u.gender,
        //    //                      weekName = w.name,
        //    //                      email = u.Email,
        //    //                      date = Convert.ToDateTime(a.date).ToString("dd-MM-yyyy") + " " + a.time,
        //    //                      departmentName = d.departmentName,
        //    //                      designationName = d.designationName
        //    //                  }).ToListAsync();

        //    //return data;

        //                var data = await (
        //         from a in _contex.AppoinmentInfos
        //         join u in _contex.UserInfos on a.userInfoId equals u.Id
        //         join d in _contex.DoctorInfos on a.doctorInfoId equals d.Id
        //         orderby a.date descending
        //         select new AppointmentListVM
        //         {
        //             Doctorname = d.name,
        //             departmentName = d.departmentName,
        //             UHID = u.UHID,
        //             Mobile = u.Mobile,
        //             gender = u.gender,
        //             appointmentTime = $"{Convert.ToDateTime(a.date).ToString("dd-MM-yyyy")} {a.time}",
        //             designationName = d.designationName,
        //             //status = a.status == 0 ? "Pending" : (a.status == 1 ? "Passed" : ""),
        //             //Verified = a.isVerified == 1 ? "Yes" : "No"
        //         }).ToListAsync();


        //    return data;
        //            }

        #region Doctor Data

        public async Task<List<DoctorInfo>> GetDoctorList()
        {
            var data = await _contex.DoctorInfos.Include(x => x.Department).Include(x => x.doctorSpecialization).ToListAsync();

            return data;
        }
        public async Task<List<DoctorSpecialization>> GetDoctorSpecializationList()
        {
            var data = await _contex.DoctorSpecializations.ToListAsync();

            return data;
        }
        public async Task<List<BotRackInfoMaster>> GetBotRackInfo()
        {
            var data = await _contex.botRackInfoMasters.Include(x => x.branchInfo).ToListAsync();

            return data;
        }

        #endregion
        #region Department Data

        public async Task<List<DepartmentInfo>> GetDepartmentList()
        {
            var data = await _contex.DepartmentInfos.ToListAsync();

            return data;
        }

        public async Task<List<DepartmentVM>> GetDepartmentDataList(int branchId)
        {
            try
            {
                var result = await (from d in _contex.DepartmentInfos
                                    where d.isDelete != 1 && d.branchInfoId == branchId
                                    select new DepartmentVM
                                    {
                                        Id = d.Id,
                                        departmentCode = d.departmentCode,
                                        shortName = d.shortName,
                                        departmentName = d.departmentName,
                                        thumbUrl = d.thumbUrl,
                                        Departmentstatus = d.status == 1 ? "Active" : "InActive",
                                        status = d.status,
                                    }).ToListAsync();
                return result;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region WrapperHeader
        public async Task<List<WrapperHeader>> GetWrapperHeaderList()
        {
            var data = await _contex.WrapperHeaders.Where(x => x.isDelete != 1).ToListAsync();

            return data;
        }
        public async Task<List<WrapperHeaderImg>> GetWrapperHeaderImageList()
        {
            var data = await _contex.WrapperHeaderImgs.Where(x => x.isDelete != 1).Include(x => x.WrapperHeader).OrderBy(x => x.sortOrder).ToListAsync();

            return data;
        }


        public bool SaveWrapperHeader(WrapperHeader model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _contex.Update(model);
                    _contex.SaveChangesAsync();
                }
                else
                {

                    _contex.Add(model);
                    if (_contex.SaveChanges() == 1) return true;
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
        public bool SaveWrapperHeaderImg(WrapperHeaderImg model)
        {
            try
            {
                if (model.Id > 0)
                {

                    _contex.Update(model);
                    _contex.SaveChangesAsync();
                }
                else
                {

                    _contex.Add(model);
                    if (_contex.SaveChanges() == 1) return true;
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




        public async Task<WrapperHeader> UpdateWrapperHeader(WrapperHeaderVM model)
        {
            var data = await _contex.WrapperHeaders.Where(x => x.Id == model.WrapperHeaderId).AsNoTracking().FirstOrDefaultAsync();


            if (data != null)
            {
                data.Id = model.WrapperHeaderId;
                data.heading = model.heading;
                data.subHeading = model.subHeading;
                data.url = model.url;


                _contex.WrapperHeaders.Update(data);
                await _contex.SaveChangesAsync();


                return data;
            }
            else
            {

                var Wrapper = new WrapperHeader()
                {
                    heading = model.heading,
                    subHeading = model.subHeading,
                    url = model.url,
                    botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    isDelete = 0
                };

                _contex.WrapperHeaders.Add(Wrapper);
                await _contex.SaveChangesAsync();

                return Wrapper;


            }


        }

        public async Task<WrapperHeaderImg> UpdateWrapperHeaderImg(WrapperHeaderVM model)
        {
            var data = await _contex.WrapperHeaderImgs.Where(x => x.Id == model.WrapperHeaderImgId).Include(x => x.WrapperHeader).AsNoTracking().FirstOrDefaultAsync();
            var imagePath = "";

            if (model.fileUrl != null)
            {
                string message = FileSave.SaveFileImagePath(out imagePath, model.fileUrl, "chatbox");
            }
            else
            {
                //imagePath = visitorInfo.imgUrl;
            };
            if (data != null)
            {
                data.Id = model.WrapperHeaderImgId;
                data.WrapperHeaderId = model.WrapperHeaderImgId;
                data.imgUrl = imagePath;
                data.status = model.status;
                data.entryDate = DateTime.Now;

                _contex.WrapperHeaderImgs.Update(data);
                await _contex.SaveChangesAsync();

                return data;
            }
            else
            {
                var Wrapper = new WrapperHeaderImg()
                {
                    WrapperHeaderId = model.WrapperHeaderId,
                    sortOrder = model.sortOrder,
                    imgUrl = imagePath,
                    status = model.status,
                    botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                    entryby = "mehedi",
                    entryDate = DateTime.Now,
                    isDelete = 0

                };

                _contex.WrapperHeaderImgs.Add(Wrapper);
                await _contex.SaveChangesAsync();

                return Wrapper;


            }


        }

        public async Task<int> DeletewrapperHeader(int id)
        {
            var data = await _contex.WrapperHeaders.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            data.isDelete = 1;
            _contex.WrapperHeaders.Update(data);

            await _contex.SaveChangesAsync();

            return 1;
        }
        public async Task<int> DeletewrapperHeaderImg(int id)
        {
            var data = await _contex.WrapperHeaderImgs.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();

            data.isDelete = 1;
            _contex.WrapperHeaderImgs.Update(data);

            await _contex.SaveChangesAsync();

            return 1;
        }

        #endregion
    }
}
