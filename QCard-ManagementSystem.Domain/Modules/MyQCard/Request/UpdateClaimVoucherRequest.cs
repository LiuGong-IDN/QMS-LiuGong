namespace QCard.ManagementSystem.Domain.Modules.MyQCard.Request
{
    public class UpdateClaimVoucherRequest
    {
        public long CLAIM_ID { get; set; }
        public int PDC_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
        public List<UpdatedClaimedPart>? PART_LIST { get; set; }
    }
    public partial class UpdatedClaimedPart
    {
        public string? PART_ID { get; set; }
        public int DISCOUNT { get; set; }
        public int QTY { get; set; }
    }
}