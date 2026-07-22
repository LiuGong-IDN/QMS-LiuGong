using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email / Mobile No. field is required.")]
        public string PhoneNumberOrEmail { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        public string UserPwd { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string Token { get; set; }
    }
}