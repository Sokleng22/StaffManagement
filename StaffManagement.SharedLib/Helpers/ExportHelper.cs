using System.Text;
using StaffManagement.SharedLib.Models;

namespace StaffManagement.SharedLib.Helpers
{
    public static class ExportHelper
    {
        public static byte[] ExportToCsv<T>(IEnumerable<T> data, string[] headers, Func<T, object[]> dataSelector)
        {
            var csvBuilder = new StringBuilder();
            
            // Add headers
            csvBuilder.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));
            
            // Add data rows
            foreach (var item in data)
            {
                var values = dataSelector(item).Select(v => EscapeCsvField(v?.ToString() ?? ""));
                csvBuilder.AppendLine(string.Join(",", values));
            }
            
            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public static byte[] ExportStaffToCsv(IEnumerable<StaffDto> staffData)
        {
            var headers = new[]
            {
                "ID", "First Name", "Last Name", "Email", "Phone", "Department", 
                "Position", "Salary", "Hire Date", "Status", "Created At", "Updated At"
            };

            return ExportToCsv(staffData, headers, staff => new object[]
            {
                staff.Id,
                staff.FirstName,
                staff.LastName,
                staff.Email,
                staff.Phone ?? "",
                staff.Department,
                staff.Position,
                staff.Salary,
                staff.HireDate.ToString("yyyy-MM-dd"),
                staff.IsActive ? "Active" : "Inactive",
                staff.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                staff.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        public static string GenerateExportFileName(string baseFileName, string extension, DateTime? timestamp = null)
        {
            var time = timestamp ?? DateTime.UtcNow;
            return $"{baseFileName}_{time:yyyy-MM-dd_HH-mm-ss}.{extension}";
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            
            return field;
        }
    }
}
