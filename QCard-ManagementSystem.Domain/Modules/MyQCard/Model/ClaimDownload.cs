namespace Modules.MyQCard.Model
{
    public partial class ClaimDownload
    {
        public long CLAIM_ID { get; set; } // dari c.ID
        public string CLAIM_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime? CLAIM_DATE { get; set; }
        public string PDC_NAME { get; set; }
        public string STATUS_NAME { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int? PACKAGE_QTY { get; set; }
        public int? PACKAGE_PART_QTY { get; set; }
        public int? CLAIM_QTY { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }

        public DateTime? PICKED_UP_DATE { get; set; }

        public string DELIVERY_NOTE_NO { get; set; }
        public string RESI { get; set; }
        public int? QTY { get; set; }
        public int? AVAILABILITY_QTY { get; set; }
        public int? PENDING_QTY { get; set; }
        public int? CLAIMED_QTY { get; set; }
    }
}
