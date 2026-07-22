using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class ProgramRequest
    {
        public int ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int? MODEL_ID { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string PRIORITY { get; set; }
        public long CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }

        public List<ProgramDetailRequest> DETAILS { get; set; } = new();
    }

    public class ProgramDetailRequest
    {
        public string PART_NO { get; set; }
        public int QTY { get; set; }
    }

}
