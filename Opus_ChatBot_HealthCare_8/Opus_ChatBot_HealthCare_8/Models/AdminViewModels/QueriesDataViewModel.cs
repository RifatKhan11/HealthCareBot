using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Opus_ChatBot_HealthCare_8.Models.BotModels;
namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class QueriesDataViewModel
    {
        public int? Id { get; set; }

        public string queriesText { get; set; }

        public int? qCount { get; set; }
        public string answerText { get; set; }
        public DateTime? entryDate { get; set; }

    }
}
