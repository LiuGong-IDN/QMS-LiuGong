namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class UserUploadResponse
    {
        public string TEMPCODE { get; set; }
        public string ACTION { get; set; }
        public int NO { get; set; }
        public string UPLOADED_FIRST_NAME { get; set; }
        public string UPLOADED_LAST_NAME { get; set; }
        public string UPLOADED_PHONE_NUMBER { get; set; }
        public string UPLOADED_EMAIL{ get; set; }
        public string UPLOADED_ROLE_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
    }
}