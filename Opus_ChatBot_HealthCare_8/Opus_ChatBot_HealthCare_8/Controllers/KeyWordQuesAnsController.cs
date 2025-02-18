using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.IServices.IServices;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    public class KeyWordQuesAnsController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ISMSService _sMSService;
        private readonly IAppointmentService _appointmentService;

        public KeyWordQuesAnsController(IUserInfoService userInfoService, ISMSService sMSService, IAppointmentService appointmentService)
        {
            _userInfoService = userInfoService;
            _sMSService = sMSService;
            _appointmentService = appointmentService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveQuery(UserFeedbackViewModel model)
        {

            string message = "success";
            var data = new UserFeedback
            {
                name = model.name,
                email = model.email,
                phone = model.phone,
                message = model.message,
                entryDate = DateTime.Now,
                effectiveDate = DateTime.Now,
                entryby = "mehedi",
                botKey = "20dddd7c-734e-4c80-bbed-fae386e0291e",
                replied = 0
            };
            await _appointmentService.SaveUserFeedback(data);

            return Json(message);
        }

    }
}
 
 
     