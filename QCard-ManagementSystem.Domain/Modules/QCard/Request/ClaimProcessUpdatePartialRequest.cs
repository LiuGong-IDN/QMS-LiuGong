namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class ClaimProcessUpdatePartialRequest
    {
        public string Type { get; set; }
        public int ClaimId { get; set; }
        public string PgiNo { get; set; }
        public string SpbNo { get; set; }
        public string ResiNo { get; set; }
        public string NOTES { get; set; }
        public long ShippingAgent { get; set; }
    }
}