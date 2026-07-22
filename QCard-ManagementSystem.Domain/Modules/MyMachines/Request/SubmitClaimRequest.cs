
using Modules.MyMachines.Model;

namespace QCard.ManagementSystem.Domain.Modules.MyMachines.Request
{
    public class SubmitClaimRequest
    {
        public int MACHINE_ID { get; set; }
        public int PDC_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string CLAIM_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
        public List<ListDataGrid>? SELECTED_PROGRAM { get; set; }
        public List<PDCEmailList>? EMAIL_LIST { get; set; }
    }
    public partial class ListDataGrid
    {
        public string? PROGRAM_ID { get; set; }
        public string? CLAIM_QTY { get; set; }
    }

    public class PDCEmailList
    {
        public string? EMAIL { get; set; }
        public string? PDC_NAME { get; set; }
    }
}
