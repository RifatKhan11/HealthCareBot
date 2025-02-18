using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class ApiProcessLogInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? totalDept { get; set; }
        public int? totalDoct { get; set; }
        public int? totalSpec { get; set; }
        public DateTime? processDate { get; set; } = DateTime.Now;
    }
}
