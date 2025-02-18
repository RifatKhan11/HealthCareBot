using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.SupportModels
{
    public class PassportInfo : BotBase
    {
        public string name { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public DateTime? dob { get; set; }
        public DateTime? applyDate { get; set; }
        public DateTime? issueDate { get; set; }
        public DateTime? expireDate { get; set; }
        public DateTime? expectedDeliveryDate { get; set; }

        public string issuePlace { get; set; }
        public string currentContact { get; set; }
        public string natonality { get; set; }
        public string passportNo { get; set; }
        public string refNo { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string reason { get; set; }
        public string gender { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string remarks { get; set; }
    }
}
