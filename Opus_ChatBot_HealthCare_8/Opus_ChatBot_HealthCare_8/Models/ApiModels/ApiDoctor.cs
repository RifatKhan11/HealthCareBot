using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.ApiModels
{
    public class ApiDoctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int doctorId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string facility { get; set; }
        public string department { get; set; }
        public string gender { get; set; }
        public string specialization { get; set; }
        public string uniqueKey { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public int isDelete { get; set; } = 0;

    }
}
