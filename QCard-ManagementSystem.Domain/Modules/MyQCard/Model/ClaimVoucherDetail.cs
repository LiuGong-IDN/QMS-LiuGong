namespace Modules.MyQCard.Model
{
    public partial class ClaimVoucherDetail
    {
        public long PART_ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public decimal RETAIL_PRICE { get; set; }
        public DateTime? PICKUP_DATE { get; set; }
        public int DISCOUNT { get; set; }
        public string DELIVERY_NOTE_NO { get; set; }
        public int CLAIM_QTY { get; set; }
        public int AVAILABLE_QTY { get; set; }
        public int PENDING_QTY { get; set; }
        public int CLAIMED_QTY { get; set; }
        public decimal TOTAL_PART_PRICE { get; set; }
    }
}