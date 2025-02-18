using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Opus_ChatBot_HealthCare_8.Models.BotModels;
namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class QueriesViewModel
    {
        public int QueriesId { get; set; }

        public string AnswerText { get; set; }

        public IEnumerable<Queries> Queries { get; set; }
        public IEnumerable<QueriesDataViewModel> queriesDataViewModels { get; set; }

    }
}
