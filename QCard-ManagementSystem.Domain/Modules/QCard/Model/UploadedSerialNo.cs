namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class UploadedSerialNo
    {
        public int ID { get; set; }
        public string TEMPCODE { get; set; }
        public int MACHINE_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public int MODEL_ID { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}