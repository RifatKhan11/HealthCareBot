using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "The {0} at most {1} characters long.")]
        [Display(Name = "PageId")]
        public string PageId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} at most {1} characters long.")]
        [Display(Name = "PageAccessToken")]
        public string PageAccessToken { get; set; }

        [Required]
        public string PageGreetingMessage { get; set; }

        [StringLength(50, ErrorMessage = "The {0} at most {1} characters long.")]
        [Display(Name = "UserFbId")]
        public string UserFbId { get; set; }
        public string BotName { get; set; }
        public string Phone { get; set; }
        public string GrettingsNameEn { get; set; }
        public string GrettingsNameBn { get; set; }

    }
}
