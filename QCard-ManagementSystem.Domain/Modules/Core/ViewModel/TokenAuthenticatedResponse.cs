using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class TokenAuthenticatedResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public DateTime ValidTo { get; set; }
    }

}
