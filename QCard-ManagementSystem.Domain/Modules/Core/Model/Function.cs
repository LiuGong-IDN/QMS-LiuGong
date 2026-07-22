using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_FUNCTION")]
    public partial class Function
    {
        public string APPLICATION { get; set; }
        public string ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CHANGED_BY { get; set; }
        public DateTime CHANGED_DATE { get; set; }
        public string FUNCTION_ID { get;set; }
        public string MODULE_ID { get;set; }
        public string FUNCTION_TYPE { get;set; }
        public string FUNCTION_NAME { get; set;}
        public DateTime CREATED_DT { get; set; }
        public DateTime CHANGED_DT { get; set; }
    }
}
