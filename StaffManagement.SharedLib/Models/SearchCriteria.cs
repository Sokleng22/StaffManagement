namespace StaffManagement.SharedLib.Models
{
    public class StaffSearchCriteria
    {
        public string? SearchTerm { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool? IsActive { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "asc";
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
