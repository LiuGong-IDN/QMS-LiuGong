using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_M_MESSAGE")]
    public partial class Message
    {
        [Key]
        public string MESSAGE_ID { get; set; }
        public string MESSAGE_TEXT { get; set; }
        public string MESSAGE_TYPE { get; set; }
        public DateTime CREATED_DT { get; set; }
        public DateTime CHANGED_DT { get; set; }
        public string CREATED_BY { get; set; }
        public string CHANGED_BY { get; set; }
    }
}
