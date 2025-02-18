using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class PassportInfoViewModel
    {
        public int Id { get; set; }

        public string name { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public DateTime? dob { get; set; }
        public DateTime? applyDate { get; set; }
        public DateTime? issueDate { get; set; }
        public DateTime? expireDate { get; set; }

        public string issuePlace { get; set; }
        public string natonality { get; set; }
        public string passportNo { get; set; }
        public string refNo { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string gender { get; set; }
        public string status { get; set; }
        public string message { get; set; }


        public int?[] heads { get; set; }
        public decimal?[] amounts { get; set; }

        public string[] dbField { get; set; }
        public string[] headName { get; set; }
        public string[] col1 { get; set; }
        public string[] col2 { get; set; }
        public string[] col3 { get; set; }
        public string[] col4 { get; set; }
        public string[] col5 { get; set; }
        public string[] col6 { get; set; }
        public string[] col7 { get; set; }
        public string[] col8 { get; set; }
        public string[] col9 { get; set; }
        public string[] col10 { get; set; }
        public string[] col11 { get; set; }
        public string[] col12 { get; set; }
        public string[] col13 { get; set; }


        public IEnumerable<PassportInfo> passportInfos { get; set; }
    }
}
