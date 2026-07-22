
namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class MachineDetail
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string PJB_NO { get; set; }
        public string EVIDENCE_PJB { get; set; }

        public int MACHINE_TYPE_ID { get; set; }
        public string MACHINE_TYPE { get; set; }
        public int MODEL_ID { get; set; }
        public string MODEL { get; set; }
        public DateTimeOffset PURCHASE_DATE { get; set; }
        public int SERVICE_PARTY_ID { get; set; }
        public string BRANCH { get; set; }
        public string SERVICE_PARTY { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string IMAGE_PATH { get; set; }
        public DateTimeOffset CREATED_DATE { get; set; }
        public bool IS_PROGRAM { get; set; }
        public bool IS_VOUCHER { get; set; }
        public decimal VOUCHER_AMOUNT { get; set; }
        public decimal CURRENT_AMOUNT { get; set; }
    }
}