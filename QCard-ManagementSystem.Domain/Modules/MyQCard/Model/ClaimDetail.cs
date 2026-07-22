namespace Modules.MyQCard.Model
{
    public partial class ClaimDetail
    {
        public long CLAIM_ID { get; set; }
        public string CATEGORY_CLAIM { get; set; }
        public string STATUS { get; set; }
        public long STATUS_ID { get; set; }
        public string CLAIM_NO { get; set; }
        public DateTime CLAIM_DATE { get; set; }
        public string SERIAL_NO { get; set; }
        public string MODEL_NAME { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public string SERVICE_PARTY_NAME { get; set; }
        public string BRANCH { get; set; }
        public string PDC_NAME { get; set; }
        public decimal CURRENT_AMOUNT { get; set; }
        public decimal CLAIM_AMOUNT { get; set; }
        public decimal PPN_VALUE { get; set; }
        public int PPN { get; set; }
        public decimal GRAND_TOTAL { get; set; }
        public string IMAGE_PATH { get; set; }
        public string NOTES { get; set; }

        public string? SPB_NO { get; set; }
        public string? RESI_NO { get; set; }
        public string? SHIPPING_AGENT_NAME { get; set; }
        public string? SHIPPING_NOTES { get; set; }
        public bool IsUnconfirmDeliveryNote { get; set; }
        public int TotalUnconfirmDeliveryNote { get; set; }
    }
}