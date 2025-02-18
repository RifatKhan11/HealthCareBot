using System;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class UserInfo
    {
        //This is Combined Id Which is pageid+userid(always unique)
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string passport { get; set; }

        public string otpMsg { get; set; }

        public string bankaccountNumber { get; set; }

        public string keyWordQues { get; set; }
        public string dateOfBirth { get; set; }
        public string UHID { get; set; }
        public string gender { get; set; }

        public int? branchInfoId { get; set; }
        public BranchInfo branchInfo { get; set; }

    }
}
