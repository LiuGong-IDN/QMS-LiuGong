
namespace QCard.ManagementSystem.Domain.Modules.MyMachines.Request
{
    public class SubmitClaimVoucherRequest
    {
        public long MACHINE_ID { get; set; }
        public int PDC_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
        public List<ClaimedPart>? SELECTED_PART { get; set; }
        public List<PDCEmailVoucherList>? EMAIL_LIST { get; set; }
    }
    public partial class ClaimedPart
    {
        public string? PART_ID { get; set; }
        public string? QTY { get; set; }
    }
    public class PDCEmailVoucherList
    {
        public string? EMAIL { get; set; }
        public string? PDC_NAME { get; set; }
    }
}
