using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BotBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string entryby { get; set; }
        public DateTime? entryDate { get; set; }
        public string botKey { get; set; }
        public int? isDelete { get; set; } = 0;

        public int? branchInfoId { get; set; }
        public BranchInfo branchInfo { get; set; }
    }
}
