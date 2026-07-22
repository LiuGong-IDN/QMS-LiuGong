namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class ClaimProcessVerifyRequest
    {
        public long CLAIM_ID { get; set; }
        public List<VerifyClaimedPart>? PART_LIST { get; set; }
    }
    public partial class VerifyClaimedPart
    {
        public string? PART_ID { get; set; }
        public int DISCOUNT { get; set; }
        public int QTY { get; set; }
    }
}
