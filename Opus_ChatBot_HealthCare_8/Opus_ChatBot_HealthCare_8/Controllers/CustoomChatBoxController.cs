using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Services;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    public class CustoomChatBoxController : Controller
    {
        private readonly IBotFlowService _botFlowService;
        private readonly IUserInfoService _userInfo;
        private readonly IBotService _botService;

        public CustoomChatBoxController(IBotFlowService botFlowService, IUserInfoService _userInfo, IBotService _botService)
        {
            _botFlowService = botFlowService;
            this._userInfo = _userInfo;
            this._botService = _botService;
        }

        // GET: /<controller>/
        [HttpGet]
        //[AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MyChatbot(string botKey)
        {
            var username = User.Identity.Name;

            var data = await _botFlowService.GetBotInfoByBotKey(botKey);

            var doc = await _botService.GetALlDoctorInfo(botKey);
            var dept = await _botService.GetAllDepartmentInfo(botKey);

            List<string> doctors = new List<string>();// { "Type your message here..", "Ask me something..", "Type Dr. Name..", "Type Department Name.." };
            List<string> departments = new List<string>();// { "Type your message here..", "Ask me something..", "Type Dr. Name..", "Type Department Name.." };

            foreach (var item in doc.Select(x => x.name).Take(3))
            {
                doctors.Add(item);
            }

            foreach (var item in dept.Select(x => x.departmentName).Take(3).ToList())
            {
                departments.Add(item);
            }

            var model = new MyChatbotViewModel
            {
                ChatbotInfo = data,
                BotRackInfoDetails = await _botFlowService.GetBotRackInfoByBotKey(botKey),
                DoctorInfos = doctors,
                DepartmentInfos = departments,
                wrapperHeaderImgs = await _botFlowService.GetWrapperHeaderImageList(botKey)
            };

            username = "evercare";

            if (username != null)
            {
                var botInfo = await _userInfo.GetBotInfoByUserName(username);
                botKey = botInfo.botKey;
            }

            ViewBag.botKey = botKey;

            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MyChatbotCTG(string botKey)
        {
            var username = User.Identity.Name;

            var data = await _botFlowService.GetBotInfoByBotKey(botKey);

            var doc = await _botService.GetALlDoctorInfo(botKey);
            var dept = await _botService.GetAllDepartmentInfo(botKey);

            List<string> doctors = new List<string>();// { "Type your message here..", "Ask me something..", "Type Dr. Name..", "Type Department Name.." };
            List<string> departments = new List<string>();// { "Type your message here..", "Ask me something..", "Type Dr. Name..", "Type Department Name.." };

            foreach (var item in doc.Select(x => x.name).Take(3))
            {
                doctors.Add(item);
            }

            foreach (var item in dept.Select(x => x.departmentName).Take(3).ToList())
            {
                departments.Add(item);
            }

            var model = new MyChatbotViewModel
            {
                ChatbotInfo = data,
                BotRackInfoDetails = await _botFlowService.GetBotRackInfoByBotKey(botKey),
                DoctorInfos = doctors,
                DepartmentInfos = departments,
                wrapperHeaderImgs = await _botFlowService.GetWrapperHeaderImageList(botKey)
            };

            if (username != null)
            {
                var botInfo = await _userInfo.GetBotInfoByUserName(username);
                botKey = botInfo.botKey;
            }

            ViewBag.botKey = botKey;

            return View(model);
        }




        [HttpGet]
        //[AllowAnonymous]
        public IActionResult ChatIcon()
        {
            return View();
        }
    }
}
