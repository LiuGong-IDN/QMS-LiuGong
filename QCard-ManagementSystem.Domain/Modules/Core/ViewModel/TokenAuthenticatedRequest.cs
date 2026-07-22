using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class TokenAuthenticatedRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
