using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QCard.ManagementSystem.Service
{
    public class KTDataTableRequest
    {
        public KTDataTableSort Sort { get; set; }
        public KTDataTableFilter[] Filters { get; set; }
        public KTDataTablePagination Pagination { get; set; }
    }

    public class KTDataTableSort
    {
        public string Field { get; set; }
        public string Sort { get; set; }
    }

    public class KTDataTableFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
        public KTDataTableFilter[] Filters { get; set; }
    }

    public class KTDataTablePagination
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Perpage { get; set; }
        public int Total { get; set; }
    }
}