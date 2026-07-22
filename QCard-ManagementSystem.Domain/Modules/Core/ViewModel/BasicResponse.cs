using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class BasicResponse
    {
        public bool Success { get; set; }
        public bool Validate { get; set; } = true;
        public string Message { get; set; }
    }
}
