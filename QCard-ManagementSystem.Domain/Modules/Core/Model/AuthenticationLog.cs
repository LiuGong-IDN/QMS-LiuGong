using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    [Table("TB_L_AUTHENTICATION_LOG")]
    public partial class AuthenticationLog
    {
        [Key]
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public string TOKEN { get; set; }
        public DateTime VALID_TO { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
    }
}
