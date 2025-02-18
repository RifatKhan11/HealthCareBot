using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class Remittance
    {
        [Key]
        public int Id { get; set; }

        public int? bankAccountDetailsId { get; set; }
        public BankAccountDetails bankAccountDetails { get; set; }

        public string refNumber { get; set; }
        public string status { get; set; }
    }
}
