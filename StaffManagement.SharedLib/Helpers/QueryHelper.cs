using StaffManagement.SharedLib.Models;

namespace StaffManagement.SharedLib.Helpers
{
    public static class QueryHelper
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, StaffSearchCriteria criteria)
        {
            return query.ApplyPaging(criteria.Page, criteria.PageSize);
        }

        public static string NormalizeSearchTerm(string? searchTerm)
        {
            return searchTerm?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public static PagedResult<T> CreatePagedResult<T>(IEnumerable<T> items, int totalCount, StaffSearchCriteria criteria)
        {
            return new PagedResult<T>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };
        }

        public static PagedResult<TResult> CreatePagedResult<TSource, TResult>(
            IEnumerable<TSource> items, 
            int totalCount, 
            StaffSearchCriteria criteria, 
            Func<TSource, TResult> mapper)
        {
            return new PagedResult<TResult>
            {
                Items = items.Select(mapper).ToList(),
                TotalCount = totalCount,
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };
        }
    }
}
