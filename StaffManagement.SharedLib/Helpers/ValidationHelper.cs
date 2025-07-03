using StaffManagement.SharedLib.Constants;
using StaffManagement.SharedLib.Models;

namespace StaffManagement.SharedLib.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Phone is optional

            // Basic phone validation - can be enhanced based on requirements
            var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            return cleanPhone.Length >= 10 && cleanPhone.All(char.IsDigit);
        }

        public static bool IsValidSortDirection(string sortDirection)
        {
            return StaffConstants.ValidSortDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidSortField(string sortField)
        {
            return StaffConstants.ValidSortFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidPageSize(int pageSize)
        {
            return pageSize >= StaffConstants.MinPageSize && pageSize <= StaffConstants.MaxPageSize;
        }

        public static bool IsValidPage(int page)
        {
            return page >= 1;
        }

        public static string SanitizeSearchTerm(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return string.Empty;

            return searchTerm.Trim().Replace("'", "").Replace("\"", "");
        }

        public static StaffSearchCriteria SanitizeSearchCriteria(StaffSearchCriteria criteria)
        {
            criteria.SearchTerm = SanitizeSearchTerm(criteria.SearchTerm);
            criteria.Department = SanitizeSearchTerm(criteria.Department);
            criteria.Position = SanitizeSearchTerm(criteria.Position);

            if (!IsValidSortField(criteria.SortBy))
                criteria.SortBy = StaffConstants.DefaultSortBy;

            if (!IsValidSortDirection(criteria.SortDirection))
                criteria.SortDirection = StaffConstants.DefaultSortDirection;

            if (!IsValidPageSize(criteria.PageSize))
                criteria.PageSize = StaffConstants.DefaultPageSize;

            if (!IsValidPage(criteria.Page))
                criteria.Page = 1;

            return criteria;
        }
    }
}
