namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class SerialNoQCard
    {
        public int Id { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string CATEGORY_PROGRAM {  get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public DateTime TOPUP_DATE { get; set; }
    }
}