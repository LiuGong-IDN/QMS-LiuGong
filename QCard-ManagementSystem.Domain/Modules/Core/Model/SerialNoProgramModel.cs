using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class SerialNoProgramModel
    {
        public int ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public int PACKAGE_QTY { get; set; }
        public int QTY { get; set; }
        public string PRIORITY { get; set; }
        public int CLAIM_QTY { get; set; }
        public DateTimeOffset CREATED_DATE { get; set; }
        public int UPDATED_BY { get; set; }
        public DateTimeOffset UPDATED_DATE { get; set; }
    }
}