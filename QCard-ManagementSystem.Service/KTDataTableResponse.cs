using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Service
{
    public class KTDataTableResponse
    {
        public KTDataTableResponseMeta meta { get; set; }
        public object[] data { get; set; }
    }

    public class KTDataTableResponseMeta
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public int total { get; set; }
        public string sort { get; set; }
        public string field { get; set; }
    }
}
