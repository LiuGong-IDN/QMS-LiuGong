using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class SerialNoRequest
    {
        public string ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public List<string> ROLE { get; set; }
        public string LOCATION_CODE { get; set; }
        public string PASSWORD { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CHANGED_BY { get; set; }
        public DateTime CHANGED_DATE { get; set; }
    }
}
