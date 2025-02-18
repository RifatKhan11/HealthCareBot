using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Opus_ChatBot_HealthCare_8.Models.ApiModelData
{
    public class ApiDepartmentData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string facility { get; set; }
        public int isDelete { get; set; } = 0;

        public string uniqueKey { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
}
