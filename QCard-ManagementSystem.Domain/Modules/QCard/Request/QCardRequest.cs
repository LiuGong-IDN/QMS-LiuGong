using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class QCardRequest
    {
        public int ID { get; set; }
        public int? CUSTOMER_ID { get; set; }
        public string CARD_NO { get; set; }
        public string VALID_TO_MONTH { get; set; }
        public string VALID_TO_YEAR { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}
