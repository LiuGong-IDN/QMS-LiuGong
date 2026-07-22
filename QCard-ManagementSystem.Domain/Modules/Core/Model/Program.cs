using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_PROGRAM")]
    public partial class Program
    {
        [Key]
        public int ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME{ get; set; }
        public string MODEL_ID { get; set; }
        public string PRIORITY { get; set; }
        public string PACKAGE_QTY { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}