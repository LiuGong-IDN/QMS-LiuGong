namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class MachineProgram
    {
        public int ID { get; set; }
        public int MACHINE_ID { get; set; }
        public int PROGRAM_ID { get; set; }
        public int PACKAGE_QTY { get; set; }
        public int CLAIM_QTY { get; set; }
        public int CURRENT_PACKAGE_QTY { get; set; }
        public int CLAIMED_QTY { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_ID { get; set; }
        public string MODEL_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public int USER_ID { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int SERVICE_PARTY_ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string PRIORITY  { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public int ORDER_INDEX { get; set; }
    }
}