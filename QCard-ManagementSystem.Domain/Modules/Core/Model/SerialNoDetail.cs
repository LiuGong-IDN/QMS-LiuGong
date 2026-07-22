using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class SerialNoDetail
    {
        public int ID { get; set; }
        public string SERIAL_NO { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public int MODEL_ID { get; set; }
        public DateTime PURCHASE_DATE { get; set; }
        public int SERVICE_PARTY_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int CUSTOMER_ID { get; set; }

    }
}