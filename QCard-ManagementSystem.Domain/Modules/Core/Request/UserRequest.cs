using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class UserRequest
    {
        public int ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public List<string> ROLE { get; set; }
        public int? PDC { get; set; }
        public string PASSWORD { get; set; }
        public string PASSWORD_CONFIRM { get; set; }
        public string COMPANY { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string EMAIL { get; set; }
        public string QCARD { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CHANGED_BY { get; set; }
        public DateTime CHANGED_DATE { get; set; }
    }
}
