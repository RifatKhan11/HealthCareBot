using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class QuestionCategoryViewModel
    {
        public int Id { get; set; }
   
        public string questioncategoryName { get; set; }
       
        public IEnumerable<questionCategory> questionCategories { get; set; }

    }
}
