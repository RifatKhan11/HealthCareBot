using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class HotelBooking: BotBase
    {
        [Required]
        public string type { get; set; }

        public int rooms { get; set; }

        public int adults { get; set; }

        public int childrens { get; set; }

        public string checkInDate { get; set; }

        public string checkIn { get; set; }
    }
}
