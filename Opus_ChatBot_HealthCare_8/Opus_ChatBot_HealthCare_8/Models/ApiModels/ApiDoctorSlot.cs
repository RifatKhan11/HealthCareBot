using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.ApiModels
{
    public class ApiDoctorSlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int slotId { get; set; }
        public string appointmentDate { get; set; }
        public string slotFrom { get; set; }
        public string slotTo { get; set; }
        public string doctorID { get; set; }
        public string facility { get; set; }
        public string remarks { get; set; }
    }
}
