using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.KeyWord
{
    public class KeyWordQuesAns  : BotBase
    {
        public string nodeName { get; set; }
        public int facebookPageId { get; set; }
        public FacebookPage facebookPage { get; set; }
        public int? questionCategoryId { get; set; }
        public questionCategory questionCategory { get; set; }

        public string question { get; set; }
        public string answer { get; set; }

        public string more { get; set; }

        public int? priority { get; set; }
        
        public int? IsLoop { get; set; }
        public int? status { get; set; } = 1;

        public int? type { get; set; } = 1; //1=text, 2=button, 3=btn group, 4=html, 5=Iframe Url, 6=card group, 7=input group, 110=by refname

        public string questionKey { get; set; } //use same key if return both at same time

        public int questionOrder { get; set; }

        public string keyWord { get; set; }

        public int? keyWordQuesAnsId { get; set; }
        //public KeyWordQuesAns keyWordQuesAns { get; set; }

        public int? isQuestion { get; set; } = 0;

        public int? nextNodeId { get; set; }
        public KeyWordQuesAns nextNode { get; set; }

        public string nodes { get; set; }

        public string responseApi { get; set; }
        public int? smsOtp { get; set; } = 0;
        public string refName { get; set; }
        
        public int? departmentId { get; set; }
        public DepartmentInfo department { get; set; }
        
        public int? doctorId { get; set; }
        public DoctorInfo doctor { get; set; }
        public int? specializationId { get; set; }
        public DoctorSpecialization specialization { get; set; }

        public string keyText { get; set; }
        public string uniqueKey { get; set; }
    }
}
