using StaffManagement.SharedLib.Models;

namespace StaffManagement.APP.Logic
{
    public interface IStaffService
    {
        Task<PagedResult<StaffDto>> GetAllStaffAsync(StaffSearchCriteria criteria);
        Task<StaffDto?> GetStaffByIdAsync(int id);
        Task<StaffDto> CreateStaffAsync(CreateStaffDto createStaffDto);
        Task<StaffDto?> UpdateStaffAsync(int id, UpdateStaffDto updateStaffDto);
        Task<bool> DeleteStaffAsync(int id);
        Task<List<StaffDto>> SearchStaffAsync(string searchTerm);
        Task<byte[]> ExportStaffToCsvAsync();
        Task<byte[]> ExportStaffToExcelAsync();
        Task<List<string>> GetDepartmentsAsync();
        Task<List<string>> GetPositionsAsync();
    }

    public interface IExportService
    {
        Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, string[] headers, Func<T, object[]> dataSelector);
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName = "Data");
        Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, string[] headers, Func<T, object[]> dataSelector);
        string GenerateFileName(string baseName, string extension);
    }

    public interface IValidationService
    {
        bool ValidateStaffDto(StaffDto staffDto, out List<string> errors);
        bool ValidateCreateStaffDto(CreateStaffDto createStaffDto, out List<string> errors);
        bool ValidateUpdateStaffDto(UpdateStaffDto updateStaffDto, out List<string> errors);
        bool ValidateSearchCriteria(StaffSearchCriteria criteria, out List<string> errors);
    }

    public interface IAuditService
    {
        Task LogCreateAsync(int staffId, string createdBy);
        Task LogUpdateAsync(int staffId, string updatedBy, string changes);
        Task LogDeleteAsync(int staffId, string deletedBy);
        Task LogSearchAsync(string searchCriteria, string performedBy);
        Task LogExportAsync(string exportType, string performedBy, int recordCount);
    }
}
