using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_SHIPPING_AGENT")]
    public partial class ShippingAgent
    {
        public int ID { get; set; }
        public string SHIPPING_AGENT_NAME { get; set; }
    }
}