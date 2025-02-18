using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.SupportModels
{
    public class OperatorAssign:BotBase
    {
        public int? OperatorId { get; set; }
        public Operator Operator { get; set; }
    }
}
