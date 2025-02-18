using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class OperatorViewModel
    {
        public int Id { get; set; }

        public string questioncategoryName { get; set; }

        public string NameBn { get; set; }

        public string NameEn { get; set; }
        public string status { get; set; }


        public IEnumerable<Operator> operators { get; set; }

        public IEnumerable<LastGrettings> lastGrettings { get; set; }

        public IEnumerable<ComplainSuggestion> complainSuggestions { get; set; }
    }
}
