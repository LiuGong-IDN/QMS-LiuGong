using System.ComponentModel.DataAnnotations.Schema;
namespace QCard.ManagementSystem.Domain.Modules.QCard.Model
{
    public partial class SerialNoUpload
    {
        public string TEMPCODE { get; set; }
        public int NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string PRODUCT_GROUP { get; set; }
        public string PRODUCT_MODEL { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int ORDER_INDEX { get; set; }
        public string MESSAGE { get; set; }
        public string STATUS { get; set; }
        public string ACTION { get; set; }
    }
}