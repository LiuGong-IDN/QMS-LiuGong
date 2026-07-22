using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_R_FILE")]
    public partial class FileModel
    {   
        [Key]
        public int ID { get; set; }
        [Key]
        public string OBJECT_KEY { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_MIMETYPE { get; set; }
        public int FILE_SIZE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}
