using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class MachineRequest
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string PJB_NO { get; set; }
        public string EVIDENCE_PJB { get; set; }
        public int? ASSET_TYPE_ID { get; set; }
        public int? MODEL_ID { get; set; }
        public int? CUSTOMER_ID { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int? SERVICE_PARTY_ID { get; set; }
        public string? BRANCH { get; set; }
        public bool IS_PROGRAM { get; set; }
        public bool IS_VOUCHER { get; set; }
        public decimal VOUCHER_AMOUNT { get; set; }

        public List<MachineDetailRequest> DETAILS { get; set; } = new();
    }

    public class MachineDetailRequest
    {
        public int PROGRAM_ID { get; set; }
        public int PACKAGE_QTY { get; set; }
        public int CLAIM_QTY { get; set; }
        public int CURRENT_PACKAGE_QTY { get; set; }
        
    }

}
