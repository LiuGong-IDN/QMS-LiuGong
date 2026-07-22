using Kendo.Mvc;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class DataSourceRequestModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Filter { get; set; }
        public string SortBy { get; set; }
        public string SortType { get; set; }
    }
}
