using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Models.BotModels;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class ApplicationUser: IdentityUser
    {
        public FacebookPage FacebookPage { get; set; }
        public int? branchId { get; set; }
        public BranchInfo branch { get; set; }
    }
}
