using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class PartNoRequest
    {
        public int ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public decimal RETAIL_PRICE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}
