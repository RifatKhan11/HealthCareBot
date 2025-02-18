using Opus_ChatBot_HealthCare_8.Models.BotModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.ApiModels
{
    public class ApiDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string facility { get; set; }

        public string uniqueKey { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public int isDelete { get; set; } = 0;

    }
}
