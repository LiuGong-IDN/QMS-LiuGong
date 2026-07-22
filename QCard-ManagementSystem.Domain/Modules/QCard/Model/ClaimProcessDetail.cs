namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class ClaimProcessDetail
    {
        public ClaimProcessDetailHeader Header { get; set; }
        public List<ClaimProcessDetailProgram> ProgramDetail { get; set; }
    }

    public partial class ClaimProcessDetailHeader
    {
        public long CLAIM_ID { get; set; }
        public int STATUS_ID { get; set; }
        public string STATUS { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public decimal CURRENT_AMOUNT { get; set; }
        public decimal CLAIM_AMOUNT {  get; set; }
        public int PPN {  get; set; }
        public decimal PPN_VALUE {  get; set; }
        public decimal GRAND_TOTAL { get; set; }
        public DateTime? CLAIM_DATE { get; set; }
        public string SERIAL_NO { get; set; }
        public string MODEL_NAME { get; set; }
        public string CATEGORY_CLAIM { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public DateTime? PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY_NAME { get; set; }
        public string BRANCH { get; set; }
        public string PDC_NAME { get; set; }
        public string IMAGE_PATH { get; set; }
        public string NOTES { get; set; }
        public string UNCONFIRM_DN { get; set; }
        public string? SPB_NO { get; set; }
        public string? RESI_NO { get; set; }
        public string? SHIPPING_AGENT_NAME { get; set; }
        public string? SHIPPING_NOTES { get; set; }
    }
    public partial class ClaimProcessDetailProgram
    {
        public long CLAIM_DETAIL_ID { get; set; }
        public long PROGRAM_ID { get; set; }
        public string CLAIM_NO { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int PACKAGE_QTY { get; set; }
        public int CLAIM_QTY { get; set; }
        public long PART_ID { get; set; }
        public int DISCOUNT { get; set; }
        public decimal RETAIL_PRICE { get; set; }
        public decimal PRICE_BEFORE_DISC { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int STATUS { get; set; }
        public int TOTAL_CLAIM_QTY { get; set; }
        public int AVAILABLE_QTY { get; set; }
        public int BACK_ORDER_QTY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public DateTime? PARTIAL_CONFIRMATION_DATE { get; set; }
        public string PGI { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal DISCOUNT_TOTAL_PRICE { get; set; }
    }
}