namespace Modules.QCard.Model
{
    public class QCardExpiryInfo
    {
        public string EMAIL { get; set; }
        public string COMPANY_NAME { get; set; }
        public string CARD_NO { get; set; }
        public int VALID_TO_MONTH { get; set; }
        public int VALID_TO_YEAR { get; set; }
        public DateTime EXPIRY_DATE { get; set; }
    }
}