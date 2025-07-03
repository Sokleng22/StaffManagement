namespace StaffManagement.SharedLib.Enums
{
    public enum StaffStatus
    {
        Active = 1,
        Inactive = 0,
        Suspended = 2,
        OnLeave = 3
    }

    public enum Department
    {
        Engineering,
        HumanResources,
        Finance,
        Marketing,
        Sales,
        Operations,
        IT,
        Legal,
        Executive,
        CustomerService,
        Research,
        Quality
    }

    public enum EmploymentType
    {
        FullTime,
        PartTime,
        Contract,
        Intern,
        Consultant,
        Temporary
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum ExportFormat
    {
        Csv,
        Excel,
        Pdf,
        Json
    }

    public enum SearchField
    {
        All,
        FirstName,
        LastName,
        Email,
        Department,
        Position,
        Phone
    }
}
