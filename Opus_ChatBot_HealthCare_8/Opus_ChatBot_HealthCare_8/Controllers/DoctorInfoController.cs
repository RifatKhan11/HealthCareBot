using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Services.IServices;
//using static Microsoft.EntityFrameworkCore.Internal.DbContextPool<TContext>;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Authorize]
    public class DoctorInfoController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IDoctorInfoService _doctorInfoService;
        private readonly IMenuService _menuService;
        private readonly IBotService botService;
        private readonly IKeyWordQuesService keyWordQuesService;

        public DoctorInfoController(IUserInfoService userInfoService, IDoctorInfoService doctorInfoService, IMenuService menuService, IBotService botService, IKeyWordQuesService keyWordQuesService)
        {
            _userInfoService = userInfoService;
            _doctorInfoService = doctorInfoService;
            _menuService = menuService;
            this.botService = botService;
            this.keyWordQuesService = keyWordQuesService;
        }

        public async Task<IActionResult> Index()
        {
            var doctorList = new DoctorListViewModel
            {
                menus = await _menuService.GetMenusBySpecialities(146),
                doctorInfos = await _doctorInfoService.GetDoctorListWithSpecialist()
            };
            return View(doctorList);
        }

        // POST: DoctorInfo/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DoctorInfoViewModel model)
        {
            //return Json(model);

            if (!ModelState.IsValid) // If Validation fail. 
            {
                model.menus = await _menuService.GetMenusBySpecialities(146);
                model.doctorInfos = await _doctorInfoService.GetDoctorListWithSpecialist();
                return View(model);
            }
            if (model.Id > 0)
            {
                DoctorInfo doctorInfo = new DoctorInfo // New Item To Save.
                {
                    Id = (int)model.Id,
                    name = model.doctorName,
                    nameBn = model.doctorName,
                    designationName = model.designationName,
                    designationNameBn = model.designationNameBn,
                    menuId = model.menuId,
                    status = 1
                };

                await _doctorInfoService.SaveDoctorInfo(doctorInfo); // Saving data In Here.
            }
            else
            {
                DoctorInfo doctorInfo = new DoctorInfo // New Item To Save.
                {
                    name = model.doctorName,
                    nameBn = model.doctorNameBn,
                    designationName = model.designationName,
                    designationNameBn = model.designationNameBn,
                    menuId = model.menuId,
                    status = 1
                };

                await _doctorInfoService.SaveDoctorInfo(doctorInfo); // Saving data In Here.
            }


            return RedirectToAction(nameof(Index));
        }

        #region
        public async Task<IActionResult> DoctorList()
        {
            var doctorList = new DoctorListViewModel
            {

                doctorInfos = await _doctorInfoService.GetDoctorList()
            };
            return View(doctorList);
        }
        #region-Department
        public async Task<ActionResult> DepartmentList()
        {
            var deptList = new DoctorListViewModel
            {

                departmentInfos = await _doctorInfoService.GetAllDepartmentInfo()
            };
            return View(deptList);
        }
        public async Task<ActionResult> DepartmentUpload()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DepartmentUpload([FromForm] IFormFile formFile)
        {

            string DepartmentCode = "";
            string DepartmentName = "";

            try
            {
                var userName = HttpContext.User.Identity.Name;
                var botInfo = await botService.GetBotInfoByUserName(userName);


                using (var stream = formFile.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[1];

                        // Read the data from the worksheet
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                            var data1 = new DepartmentInfo()
                            {
                                departmentCode = Convert.ToString(worksheet.Cells[row, 1].Value),
                                departmentName = Convert.ToString(worksheet.Cells[row, 2].Value),

                                botKey = botInfo.botKey,
                                status = 1


                            };

                            var saveDept = await _doctorInfoService.SaveDepartment(data1);
                            var depId = saveDept;

                            var keyword = new KeyWordQuesAns()
                            {
                                departmentId = depId,
                                facebookPageId = 2,
                                question = Convert.ToString(worksheet.Cells[row, 2].Value),
                                answer = Convert.ToString(worksheet.Cells[row, 2].Value),
                                questionCategoryId = 1,
                                priority = 1,
                                IsLoop = 1,
                                keyWord = Convert.ToString(worksheet.Cells[row, 2].Value),
                                botKey = botInfo.botKey,
                                type = 101,
                                questionOrder = 1,
                                status = 1,
                                isQuestion = 1,
                                //  doctorId=depId,
                                questionKey = Guid.NewGuid().ToString().ToUpper()


                            };

                            var keywordSave = await keyWordQuesService.SavekeyWordQuesAns(keyword);
                            var keywordId = keywordSave;
                            var botKnowledge = new BotKnowledge()
                            {
                                keyWordQuesAnsId = keywordId,

                                question = Convert.ToString(worksheet.Cells[row, 2].Value),
                                textReply = Convert.ToString(worksheet.Cells[row, 2].Value),

                                botKey = botInfo.botKey,


                                status = 1,



                            };

                            var botKnow = await keyWordQuesService.SaveBotKnowledges(botKnowledge);

                            //var serviceFlow = new ServiceFlow()
                            //{
                            //    keyWordQuesAnsId = keywordId,

                            //    //question = Convert.ToString(worksheet.Cells[row, 2].Value),
                            //    //textReply = Convert.ToString(worksheet.Cells[row, 2].Value),

                            //    botKey = botInfo.botKey,


                            //    status = 1,



                            //};

                            //var serviseflow = await keyWordQuesService.SaveServiceFlows(serviceFlow);
                        }
                    }
                }
                return RedirectToAction(nameof(DepartmentList));
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        [HttpGet("/api/DownloadExcelDepartmentFile")]
        public IActionResult DownloadExcelDepartmentFile()
        {
            var fileName = "DepartmentFileExcel.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        #endregion
        #region-DoctorSpecialization
        public async Task<ActionResult> DoctorSpecializationList()
        {
            var DocSpecList = new DoctorListViewModel
            {

                doctorSpecializations = await _doctorInfoService.GetAllDoctorSpecialization()
            };
            return View(DocSpecList);
        }
        public async Task<ActionResult> DoctorSpecialization()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DoctorSpecialization([FromForm] IFormFile formFile)
        {

            string SpecialisationCode = "";
            string Specialisation = "";
            try
            {
                var userName = HttpContext.User.Identity.Name;
                var botInfo = await botService.GetBotInfoByUserName(userName);


                using (var stream = formFile.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[1];

                        // Read the data from the worksheet
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

                            var data3 = new DoctorSpecialization()
                            {
                                code = Convert.ToString(worksheet.Cells[row, 1].Value),
                                name = Convert.ToString(worksheet.Cells[row, 2].Value),

                                botKey = botInfo.botKey,
                                status = "1"


                            };
                            var DoctorSpec = await _doctorInfoService.SaveDoctorSpecialization(data3);

                        }
                    }
                }
                return RedirectToAction(nameof(DoctorSpecializationList));
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        [HttpGet("/api/DoctorSpecializationExcelFile")]
        public IActionResult DoctorSpecializationExcelFile()
        {
            var fileName = "DoctorSpecializationExcel.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        #endregion
        public async Task<ActionResult> DoctorListUpload()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DoctorListUpload([FromForm] IFormFile formFile)
        {
            string DoctorCode = "";
            string DoctorName = "";
            string DepartmentCode = "";
            string DepartmentName = "";
            string SpecialisationCode = "";
            string Specialisation = "";
            try
            {
                var userName = HttpContext.User.Identity.Name;
                var botInfo = await botService.GetBotInfoByUserName(userName);


                using (var stream = formFile.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[1];

                        // Read the data from the worksheet
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                            //var data1 = new DepartmentInfo()
                            //{
                            //    departmentName = Convert.ToString(worksheet.Cells[row, 4].Value),
                            //    departmentCode = Convert.ToString(worksheet.Cells[row, 3].Value),
                            //    botKey= botInfo.botKey,
                            //    status=1


                            //};
                            //var saveDept = await _doctorInfoService.SaveDepartment(data1);
                            //var data3 = new DoctorSpecialization()
                            //{
                            //    name = Convert.ToString(worksheet.Cells[row, 6].Value),
                            //    code = Convert.ToString(worksheet.Cells[row, 5].Value),
                            //    botKey = botInfo.botKey,
                            //    status="1"


                            //};
                            //var DoctorSpec = await _doctorInfoService.SaveDoctorSpecialization(data3);

                            //  var DeptId = saveDept;
                            // var DoctorSpecId = DoctorSpec;
                            var data = new DoctorInfo()
                            {
                                doctorCode = Convert.ToString(worksheet.Cells[row, 1].Value),
                                name = Convert.ToString(worksheet.Cells[row, 2].Value),
                                botKey = botInfo.botKey,
                                // DepartmentId = DeptId,

                                departmentName = Convert.ToString(worksheet.Cells[row, 4].Value),
                                //doctorSpecializationId = DoctorSpecId,
                                status = 1

                            };
                            var savedata = await _doctorInfoService.SavedDoctorDetails(data);
                            var docID = savedata;
                            var GuId = Guid.NewGuid().ToString().ToUpper();
                            var keyword = new KeyWordQuesAns()
                            {
                                //departmentId = DeptId,
                                facebookPageId = 2,
                                question = Convert.ToString(worksheet.Cells[row, 4].Value),
                                answer = Convert.ToString(worksheet.Cells[row, 4].Value),
                                questionCategoryId = 1,
                                priority = 1,
                                IsLoop = 1,
                                keyWord = Convert.ToString(worksheet.Cells[row, 2].Value),
                                botKey = botInfo.botKey,
                                type = 101,
                                questionOrder = 1,
                                status = 1,
                                isQuestion = 1,
                                doctorId = docID,
                                questionKey = GuId


                            };

                            var keywordSave = await keyWordQuesService.SavekeyWordQuesAns(keyword);
                            var keywordId = Convert.ToInt32(keywordSave);
                            var botKnowledge = new BotKnowledge()
                            {
                                keyWordQuesAnsId = keywordId,

                                question = Convert.ToString(worksheet.Cells[row, 4].Value),
                                textReply = Convert.ToString(worksheet.Cells[row, 4].Value),

                                botKey = botInfo.botKey,


                                status = 1,

                            };

                            var botKnow = await keyWordQuesService.SaveBotKnowledges(botKnowledge);




                        }
                    }
                }
                return RedirectToAction(nameof(DoctorList));
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        #region
        public async Task<ActionResult> DoctorExcelUpload([FromForm] IFormFile formFile)
        {
            try
            {
                var userName = HttpContext.User.Identity.Name;
                var botInfo = await botService.GetBotInfoByUserName(userName);
                using (var stream = formFile.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[1];

                        // Read the data from the worksheet
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var excelData = new DoctorInfoForExcel
                            {
                                Department = worksheet.Cells[row, 3].Value?.ToString(),
                                DoctorSpecialization = worksheet.Cells[row, 4].Value?.ToString(),

                            };

                            var excelDataId = await _doctorInfoService.GetExcelLeadDataId(excelData);

                            var cellValue = worksheet.Cells[row, 1].Value;
                            if (worksheet.Cells[row, 1].Value != null)
                            {


                                int? Department = null;
                                int? DoctorSpecialization = null;

                                if (excelDataId.Department != null)
                                {
                                    Department = Convert.ToInt32(excelDataId.Department);
                                }
                                if (excelDataId.DoctorSpecialization != null)
                                {
                                    DoctorSpecialization = Convert.ToInt32(excelDataId.DoctorSpecialization);
                                }

                                var data = new DoctorInfo()
                                {

                                    doctorCode = (string)worksheet.Cells[row, 1].Value?.ToString(),

                                    name = (string)worksheet.Cells[row, 2].Value?.ToString(),

                                    DepartmentId = Department,
                                    doctorSpecializationId = DoctorSpecialization,
                                    botKey = botInfo.botKey,
                                    status = 1
                                };
                                var savedata = await _doctorInfoService.SavedDoctorDetails(data);
                                var docID = savedata;
                                var keyword = new KeyWordQuesAns()
                                {
                                    //departmentId = DeptId,
                                    facebookPageId = 2,
                                    question = worksheet.Cells[row, 3].Value?.ToString(),
                                    answer = worksheet.Cells[row, 3].Value?.ToString(),
                                    questionCategoryId = 1,
                                    priority = 1,
                                    IsLoop = 1,
                                    keyWord = worksheet.Cells[row, 3].Value?.ToString(),
                                    botKey = botInfo.botKey,
                                    type = 101,
                                    questionOrder = 1,
                                    status = 1,
                                    isQuestion = 1,
                                    doctorId = docID,
                                    questionKey = Guid.NewGuid().ToString().ToUpper(),


                                };

                                var keywordSave = await keyWordQuesService.SavekeyWordQuesAns(keyword);
                                var keywordId = Convert.ToInt32(keywordSave);
                                var botKnowledge = new BotKnowledge()
                                {
                                    keyWordQuesAnsId = keywordId,

                                    question = worksheet.Cells[row, 3].Value?.ToString(),
                                    textReply = worksheet.Cells[row, 3].Value?.ToString(),

                                    botKey = botInfo.botKey,
                                    status = 1,

                                };

                                var botKnow = await keyWordQuesService.SaveBotKnowledges(botKnowledge);


                            }
                        }
                    }
                }
                return RedirectToAction(nameof(DoctorList));
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        #endregion







        [HttpGet("/api/DownloadExcelFile")]
        public IActionResult DownloadExcelFile()
        {
            var fileName = "DoctorExcel.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet("/api/DownloadExcelFile1")]
        public async Task<IActionResult> DownloadExcelFile1()
        {
            int totalRows = 5000;
            int totalColumn = 4;

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");
            var worksheetD = workbook.Worksheets.Add("Dropdowns");



            //Data
            var DoctorInfo = await _doctorInfoService.GetDoctorListWithSpecialist();
            var Specialization = await _doctorInfoService.GetAllDoctorSpecialization();
            var DepartmentInfo = await _doctorInfoService.GetAllDepartmentInfo();

            //Populate Dropdown data

            var DeptColumn = 1;
            worksheetD.Cell(1, DeptColumn).Value = "DepartmentName";
            int c = 2;
            foreach (var item in DepartmentInfo.OrderBy(x => x.departmentName))
            {
                worksheetD.Cell(c, DeptColumn).Value = item.departmentName;
                c++;
            }
            var SpecializationColumn = 2;
            worksheetD.Cell(1, SpecializationColumn).Value = "DoctorSpecialization";
            c = 2;
            foreach (var item in Specialization.OrderBy(x => x.name))
            {
                worksheetD.Cell(c, SpecializationColumn).Value = item.name;
                c++;
            }


            //Heading        
            worksheet.Cell("A1").Value = "DoctorCode";
            worksheet.Cell("B1").Value = "DoctorName";
            worksheet.Cell("C1").Value = "DepartmentName";
            worksheet.Cell("D1").Value = "SpecialisationName";

            //Dropdown_Sectors
            for (int i = 2; i <= totalRows; i++)
            {
                //worksheet.Cell(i, 3).DataValidation.List(worksheetD.Range(2, DeptColumn, DepartmentInfo.Count(), DeptColumn), true);
            }
            for (int i = 2; i <= totalRows; i++)
            {
                //worksheet.Cell(i, 4).DataValidation.List(worksheetD.Range(2, SpecializationColumn, Specialization.Count(), SpecializationColumn), true);
            }





            //Styles
            worksheet.Range(1, 1, 1, totalColumn).Style.Fill.BackgroundColor = XLColor.BabyBlueEyes;
            var rangeWithBorders = worksheet.Range(1, 1, totalRows, totalColumn);

            // Apply border formatting
            rangeWithBorders.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            rangeWithBorders.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rangeWithBorders.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            rangeWithBorders.Style.Border.RightBorder = XLBorderStyleValues.Thin;


            //worksheetD.Visibility = XLWorksheetVisibility.VeryHidden;


            //Download
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                //return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LeadReceivers_" + DateTime.Now.ToString() + ".xlsx");
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DoctorInfoExcel.xlsx");
            }
        }


        #endregion


    }
}