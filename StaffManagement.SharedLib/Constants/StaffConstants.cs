namespace StaffManagement.SharedLib.Constants
{
    public static class StaffConstants
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const int MinPageSize = 1;

        public const string DefaultSortBy = "Id";
        public const string DefaultSortDirection = "asc";

        public const decimal MinSalary = 0;
        public const decimal MaxSalary = 999999999;

        public const int FirstNameMaxLength = 100;
        public const int LastNameMaxLength = 100;
        public const int EmailMaxLength = 255;
        public const int PhoneMaxLength = 20;
        public const int DepartmentMaxLength = 100;
        public const int PositionMaxLength = 100;

        public static readonly string[] ValidSortDirections = { "asc", "desc" };
        public static readonly string[] ValidSortFields = 
        { 
            "Id", "FirstName", "LastName", "Email", "Department", 
            "Position", "Salary", "HireDate", "IsActive", "CreatedAt", "UpdatedAt" 
        };
    }

    public static class ApiConstants
    {
        public const string ApiVersion = "v1";
        public const string ApiTitle = "Staff Management API";
        public const string ApiDescription = "A comprehensive staff management system API";

        public const string CorsPolicy = "AllowReactApp";
        public const string DefaultCorsOrigin = "http://localhost:3000";

        public const string ContentTypeJson = "application/json";
        public const string ContentTypeCsv = "text/csv";
        public const string ContentTypeExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string ContentTypePdf = "application/pdf";
    }

    public static class ErrorMessages
    {
        public const string StaffNotFound = "Staff member not found";
        public const string InvalidSearchCriteria = "Invalid search criteria provided";
        public const string DuplicateEmail = "A staff member with this email already exists";
        public const string InvalidSortField = "Invalid sort field specified";
        public const string InvalidSortDirection = "Invalid sort direction specified";
        public const string InvalidPageSize = "Page size must be between {0} and {1}";
        public const string InvalidPage = "Page number must be greater than 0";
        public const string ExportFailed = "Failed to export staff data";
    }

    public static class SuccessMessages
    {
        public const string StaffCreated = "Staff member created successfully";
        public const string StaffUpdated = "Staff member updated successfully";
        public const string StaffDeleted = "Staff member deleted successfully";
        public const string ExportCompleted = "Export completed successfully";
    }
}
