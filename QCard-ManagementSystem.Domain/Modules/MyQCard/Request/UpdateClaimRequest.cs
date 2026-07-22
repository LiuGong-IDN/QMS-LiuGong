
namespace QCard.ManagementSystem.Domain.Modules.MyQCard.Request
{
    public class UpdateClaimRequest
    {
        public long CLAIM_ID { get; set; }
        public int PDC_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
        public List<UpdateClaimDetail>? UPDATED_CLAIM { get; set; }
    }
    public partial class UpdateClaimDetail
    {
        public string? PROGRAM_ID { get; set; }
        public string? CLAIM_QTY { get; set; }
    }
}
