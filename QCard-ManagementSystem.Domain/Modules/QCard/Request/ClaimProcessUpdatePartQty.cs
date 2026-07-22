namespace QCard.ManagementSystem.Domain.Modules.QCard.Request
{
    public class ClaimProcessUpdatePartQty
    {
        public string ClaimNo { get; set; }
        public string? DnNumber { get; set; }
        public List<ClaimProsessProgramDetail> ProgramDetails { get; set; } = new();
    }
}