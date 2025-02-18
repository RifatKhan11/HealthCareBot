using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class BankAccountDetails
    {
        [Key]
        public int Id { get; set; }
        public string acNumber { get; set; }
        public string   name { get; set; }
        public string  birthdate { get; set; }
        public string mothersName { get; set; }
        public string mobile { get; set; }
        public decimal accountBlns { get; set; }
    }
}
