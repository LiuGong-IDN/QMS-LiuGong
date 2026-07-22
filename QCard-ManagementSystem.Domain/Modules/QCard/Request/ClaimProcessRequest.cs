namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class ClaimProcessRequest
    {
        public long ClaimId { get; set; }
        public string? ActionName { get; set; }
        public int? ClaimStatus { get; set; }
        public string? DnNumber { get; set; }
        public string? PgiNo { get; set; }
        public string? SpbNo { get; set; }
        public string? ResiNo { get; set; }
        public int? ShippingAgent { get; set; }
        public string? ReasonReject { get; set; }
        public string? NOTES { get; set; }
        public decimal Grand_Total { get; set; }

        public List<ClaimProsessProgramDetail> ProgramDetails { get; set; } = new();
    }

    public class ClaimProsessProgramDetail
    {
        public string CLAIM_NO { get; set; }
        public int CLAIM_DETAIL_ID { get; set; }
        public int PROGRAM_ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public int TOTAL_CLAIM_QTY { get; set; }
        public int AVAILABLE_QTY { get; set; }
        public int BACK_ORDER_QTY { get; set; }
    }
}