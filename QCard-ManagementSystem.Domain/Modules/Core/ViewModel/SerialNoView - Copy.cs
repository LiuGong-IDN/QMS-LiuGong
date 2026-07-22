using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public partial class SerialNoViewCopy
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string ASSET_TYPE_CODE { get; set; }
        public string MODEL_CODE { get; set; }
        public string COMPANY_NAME { get; set; }
        public string LOCATION { get; set; }
        public string PURCHASE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
    }
}