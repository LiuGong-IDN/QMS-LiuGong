namespace Modules.QCard.Model
{
    public class TopupInfo
    {
        public int ID { get; set; }
        public int NEXT_APPROVER_ROLE_ID { get; set; }
        public List<ApprovalEmail> EMAIL_LIST { get; set; }
        public string SERIAL_NO { get; set; }
        public string CARD_NO { get; set; }
        public string CATEGORY_PROGRAM { get; set; }
        public string COMPANY_NAME { get; set; }
        public int STATUS_ID { get; set; }
    }

    public class ApprovalEmail
    {
        public string EMAIL { get; set; }
        public string APPROVAL_NAME { get; set; }
    }
}