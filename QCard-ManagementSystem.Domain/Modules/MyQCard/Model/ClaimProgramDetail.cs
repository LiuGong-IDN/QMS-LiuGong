namespace Modules.MyQCard.Model
{
    public partial class ClaimProgramDetail
    {
        public long CLAIM_DETAIL_ID { get; set; }
        public long PROGRAM_ID { get; set; }
        public string CLAIM_NO { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int PACKAGE_QTY { get; set; }
        public int CLAIMED_QTY { get; set; }
        public int REMAINING_QTY { get; set; }
        public int CLAIM_UP_TO { get; set; }
        public int CLAIM_QTY { get; set; }
        public int AVAILABLE_QTY { get; set; }
    }
}