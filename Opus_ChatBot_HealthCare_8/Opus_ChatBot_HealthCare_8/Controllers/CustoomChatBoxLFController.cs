using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Opus_ChatBot_HealthCare_8.Controllers
{
    public class CustoomChatBoxLFController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        //[AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
    }
}
