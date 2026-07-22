using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class BasicFileResponse
    {
        public byte[] FileContent { get; set; }
        public string FileMimeType { get; set; }
        public string FileName { get; set; }
    }
}
    