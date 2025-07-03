using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StaffManagement.APP.Logic.DBContext;
using StaffManagement.SharedLib.Models;
using System.Text;

namespace StaffManagement.APP.Logic
{
    public class StaffService : IStaffService
    {
        private readonly StaffManagementDBContext _context;
        private readonly IMapper _mapper;

        public StaffService(StaffManagementDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<StaffDto>> GetAllStaffAsync(StaffSearchCriteria searchCriteria)
        {
            var query = _context.Staff.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchCriteria.SearchTerm))
            {
                var searchTerm = searchCriteria.SearchTerm.ToLower();
                query = query.Where(s => s.FirstName.ToLower().Contains(searchTerm) ||
                                        s.LastName.ToLower().Contains(searchTerm) ||
                                        s.Email.ToLower().Contains(searchTerm) ||
                                        s.Department.ToLower().Contains(searchTerm) ||
                                        s.Position.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(searchCriteria.Department))
            {
                query = query.Where(s => s.Department.ToLower().Contains(searchCriteria.Department.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchCriteria.Position))
            {
                query = query.Where(s => s.Position.ToLower().Contains(searchCriteria.Position.ToLower()));
            }

            if (searchCriteria.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == searchCriteria.IsActive.Value);
            }

            if (searchCriteria.MinSalary.HasValue)
            {
                query = query.Where(s => s.Salary >= searchCriteria.MinSalary.Value);
            }

            if (searchCriteria.MaxSalary.HasValue)
            {
                query = query.Where(s => s.Salary <= searchCriteria.MaxSalary.Value);
            }

            if (searchCriteria.HireDateFrom.HasValue)
            {
                query = query.Where(s => s.HireDate >= searchCriteria.HireDateFrom.Value);
            }

            if (searchCriteria.HireDateTo.HasValue)
            {
                query = query.Where(s => s.HireDate <= searchCriteria.HireDateTo.Value);
            }

            // Apply sorting
            query = searchCriteria.SortBy.ToLower() switch
            {
                "firstname" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.FirstName) 
                    : query.OrderBy(s => s.FirstName),
                "lastname" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.LastName) 
                    : query.OrderBy(s => s.LastName),
                "email" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.Email) 
                    : query.OrderBy(s => s.Email),
                "department" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.Department) 
                    : query.OrderBy(s => s.Department),
                "position" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.Position) 
                    : query.OrderBy(s => s.Position),
                "salary" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.Salary) 
                    : query.OrderBy(s => s.Salary),
                "hiredate" => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.HireDate) 
                    : query.OrderBy(s => s.HireDate),
                _ => searchCriteria.SortDirection.ToLower() == "desc" 
                    ? query.OrderByDescending(s => s.Id) 
                    : query.OrderBy(s => s.Id)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((searchCriteria.Page - 1) * searchCriteria.PageSize)
                .Take(searchCriteria.PageSize)
                .ToListAsync();

            var staffDtos = _mapper.Map<List<StaffDto>>(items);

            return new PagedResult<StaffDto>
            {
                Items = staffDtos,
                TotalCount = totalCount,
                Page = searchCriteria.Page,
                PageSize = searchCriteria.PageSize
            };
        }

        public async Task<StaffDto?> GetStaffByIdAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            return staff == null ? null : _mapper.Map<StaffDto>(staff);
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto createStaffDto)
        {
            var staff = _mapper.Map<Staff>(createStaffDto);
            staff.CreatedAt = DateTime.UtcNow;
            staff.UpdatedAt = DateTime.UtcNow;

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return _mapper.Map<StaffDto>(staff);
        }

        public async Task<StaffDto?> UpdateStaffAsync(int id, UpdateStaffDto updateStaffDto)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
                return null;

            _mapper.Map(updateStaffDto, staff);
            staff.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<StaffDto>(staff);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
                return false;

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<StaffDto>> SearchStaffAsync(string searchTerm)
        {
            var searchCriteria = new StaffSearchCriteria
            {
                SearchTerm = searchTerm,
                Page = 1,
                PageSize = 100
            };

            var result = await GetAllStaffAsync(searchCriteria);
            return result.Items;
        }

        public async Task<byte[]> ExportStaffToCsvAsync()
        {
            var staff = await _context.Staff.ToListAsync();
            var csvBuilder = new StringBuilder();

            // Add header
            csvBuilder.AppendLine("Id,FirstName,LastName,Email,Phone,Department,Position,Salary,HireDate,IsActive,CreatedAt,UpdatedAt");

            // Add data
            foreach (var member in staff)
            {
                csvBuilder.AppendLine($"{member.Id},{EscapeCsvField(member.FirstName)},{EscapeCsvField(member.LastName)},{EscapeCsvField(member.Email)},{EscapeCsvField(member.Phone ?? "")},{EscapeCsvField(member.Department)},{EscapeCsvField(member.Position)},{member.Salary},{member.HireDate:yyyy-MM-dd},{member.IsActive},{member.CreatedAt:yyyy-MM-dd HH:mm:ss},{member.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> ExportStaffToExcelAsync()
        {
            // For simplicity, we'll return CSV data as Excel format
            // In a real implementation, you'd use a library like EPPlus or ClosedXML
            return await ExportStaffToCsvAsync();
        }

        public async Task<List<string>> GetDepartmentsAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive)
                .Select(s => s.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<List<string>> GetPositionsAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive)
                .Select(s => s.Position)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
