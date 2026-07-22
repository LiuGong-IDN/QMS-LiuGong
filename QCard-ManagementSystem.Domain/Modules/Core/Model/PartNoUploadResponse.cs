namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class PartNoUploadResponse
    {
        public string TEMPCODE { get; set; }
        public string ACTION { get; set; }
        public int NO { get; set; }
        public string UPLOADED_PART_NO { get; set; }
        public string UPLOADED_PART_NAME { get; set; }
        public int UPLOADED_RETAIL_PRICE { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
    }
}