using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class RegisterViewModel
    {
        public string? ID { get; set; }
        [Required(ErrorMessage = "First Name field is required.")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        [Required(ErrorMessage = "Email field is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone Number field is required.")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "QCard Number field is required.")]
        public string? QCard { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        public string? UserPwd { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        public string? UserPwdConfirm { get; set; }
    }
}