namespace QCard.ManagementSystem.Domain.Modules.MyQCard.Model
{
    public partial class ClaimedPartHistory
    {
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public DateTime PICKED_UP_DATE { get; set; }
        public string DELIVERY_NOTE { get; set; }
        public string RESI { get; set; }
        public int QTY { get; set; }
        public int AVAILABLE_QTY { get; set; }
        public int PENDING_QTY { get; set; }
        public int CLAIMED_QTY { get; set; }
    }
}