using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public partial class PartNoView
    {
        public int ID { get; set; }
        public string PART_NO { get; set; }
        public string PART_NAME { get; set; }
        public decimal RETAIL_PRICE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}