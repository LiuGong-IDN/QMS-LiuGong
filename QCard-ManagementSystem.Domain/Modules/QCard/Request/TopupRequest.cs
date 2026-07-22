using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class TopupRequest
    {
        public int ID { get; set; }               
        public int CARD_ID { get; set; }
        public DateTime TOPUP_DATE { get; set; }
        public int? APPROVAL_LEVEL_ID { get; set; }
        public List<TopupDetailRequest> DETAILS { get; set; } = new();
    }
    public class TopupDetailRequest
    {
        public string SERIAL_NO { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int? UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}
