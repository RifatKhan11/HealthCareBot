using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Opus_ChatBot_HealthCare_8.Models
{
    public class WebHookLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string type { get; set; }
        public DateTime? logTime { get; set; }
        public string requestBody { get; set; }


        public string integrationType { get; set; }

        public string messageType { get; set; } //Button,Text
        public string from { get; set; }
        public string to { get; set; }
        public string description { get; set; }

        public string messageId { get; set; }
        public string pairedMessageId { get; set; }

        public string status { get; set; }
        public DateTime? sentAt { get; set; }
        public DateTime? seenAt { get; set; }
        public string name { get; set; }
    }
}
