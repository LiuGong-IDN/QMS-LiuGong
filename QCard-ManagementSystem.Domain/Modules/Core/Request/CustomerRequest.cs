    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace QCard.ManagementSystem.Domain.Modules.Core.Request
    {
        public class CustomerRequest
        {
            public long ID { get; set; }
            public string COMPANY_CODE { get; set; }
            public string COMPANY_NAME { get; set; }
            public string ADDRESS { get; set; }
            public string CREATED_BY { get; set; }
            public DateTime CREATED_DATE { get; set; }
            public string UPDATED_BY { get; set; }
            public DateTime? UPDATED_DATE { get; set; }
    }
    }
