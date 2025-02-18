using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Helpers;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFacebookService facebookService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService fileService;

        public FileController(IFacebookService facebookService, UserManager<ApplicationUser> userManager, IFileService fileService)
        {
            this.facebookService = facebookService;
            _userManager = userManager;
            this.fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));
            FileViewModel fileViewModel = new FileViewModel
            {
                //baseUrl = "https://tota.azurewebsites.net/",
                baseUrl = "http://115.127.99.113:239/",
                //baseUrl = "https://opusbot.opus-bd.com:93/",
                //baseUrl = "http://localhost:23997/",
                //baseUrl = "https://dataqbd.com/",
                Files = await fileService.GetAllFiles(FbPageId)
            };

            return View(fileViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FileViewModel model)
        {
            bool status = true;
            if (!ModelState.IsValid)
            {
                status = false;
            }

            ///File Upload Check
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".txt" };
            var extension = Path.GetExtension(model.MyFile.FileName);

            if (!allowedExtensions.Contains(extension.ToLower()) || (model.MyFile.Length > 2000000))
            {
                Errors.AddErrorToModelState("img", "Select jpg,jpeg, png, pdf, docx, xlsx or txt less than 2Μ.", ModelState);
                status = false;
            }

            var fileName = Path.Combine("Files", DateTime.Now.Ticks + extension);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
            fileName = fileName.Replace('\\', '/');
            try
            {

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.MyFile.CopyToAsync(stream);
                }

            }
            catch
            {
                status = false;
                Errors.AddErrorToModelState("Message", "Somethins Went Wrong", ModelState);
            }

            int FbPageId = await facebookService.GetPageIdByLoggedInUserId(_userManager.GetUserId(HttpContext.User));

            if (status)
                await fileService.SaveNewFile(fileName, "Image", FbPageId);

            FileViewModel fileViewModel = new FileViewModel
            {
                //baseUrl = "https://tota.azurewebsites.net/",
                baseUrl = "http://103.95.38.180/",
                // baseUrl = "https://opusbot.opus-bd.com/",
                // baseUrl = "https://424f108a.ngrok.io/",
                //baseUrl = "http://localhost:23997/",
                //baseUrl = "https://dataqbd.com/",
                Files = await fileService.GetAllFiles(FbPageId)
            };

            return View(fileViewModel);

        }

    }
}