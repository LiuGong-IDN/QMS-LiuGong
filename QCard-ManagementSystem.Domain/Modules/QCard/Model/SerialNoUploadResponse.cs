namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class SerialNoUploadResponse
    {
        public string TEMPCODE { get; set; }
        public int NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string UPLOADED_PRODUCT_GROUP { get; set; }
        public string UPLOADED_PRODUCT_MODEL { get; set; }
        public string REGISTERED_PRODUCT_GROUP { get; set; }
        public string REGISTERED_PRODUCT_MODEL { get; set; }
        public string SERIAL_NO_MATCH_STATUS { get; set; }
        public string PURCHASE_DATE { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ACTION { get; set; }
    }
}