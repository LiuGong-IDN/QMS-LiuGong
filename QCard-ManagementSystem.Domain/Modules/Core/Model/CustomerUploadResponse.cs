namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class CustomerUploadResponse
    {
        public string TEMPCODE { get; set; }
        public string ACTION { get; set; }
        public int NO { get; set; }
        public string UPLOADED_COMPANY_CODE { get; set; }
        public string UPLOADED_COMPANY_NAME { get; set; }
        public string UPLOADED_ADDRESS { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
    }
}