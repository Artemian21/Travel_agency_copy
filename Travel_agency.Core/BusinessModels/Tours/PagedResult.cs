namespace Travel_agency.Core.BusinessModels.Tours
{
    public class PagedResult<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
