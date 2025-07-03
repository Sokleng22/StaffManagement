using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StaffManagement.APP.Logic;
using StaffManagement.APP.Logic.DBContext;
using StaffManagement.SharedLib.Models;
using StaffManagement.SharedLib.Profiles;
namespace StaffManagement.Tests;

public class StaffServiceTests : IDisposable
{
    private readonly StaffManagementDBContext _context;
    private readonly IMapper _mapper;
    private readonly StaffService _staffService;

    public StaffServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<StaffManagementDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StaffManagementDBContext(options);

        // Create AutoMapper configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StaffProfile>();
        });
        _mapper = config.CreateMapper();

        _staffService = new StaffService(_context, _mapper);
    }

    [Fact]
    public async Task GetAllStaffAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        await SeedTestData();
        var searchCriteria = new StaffSearchCriteria
        {
            Page = 1,
            PageSize = 2
        };

        // Act
        var result = await _staffService.GetAllStaffAsync(searchCriteria);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount); // We seeded 3 staff members
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetStaffByIdAsync_ShouldReturnStaff_WhenExists()
    {
        // Arrange
        await SeedTestData();
        var staff = await _context.Staff.FirstAsync();

        // Act
        var result = await _staffService.GetStaffByIdAsync(staff.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.Id, result.Id);
        Assert.Equal(staff.FirstName, result.FirstName);
        Assert.Equal(staff.LastName, result.LastName);
    }

    [Fact]
    public async Task GetStaffByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _staffService.GetStaffByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateStaffAsync_ShouldCreateNewStaff()
    {
        // Arrange
        var createDto = new CreateStaffDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "555-1234",
            Department = "IT",
            Position = "Developer",
            Salary = 75000,
            HireDate = DateTime.Today,
            IsActive = true
        };

        // Act
        var result = await _staffService.CreateStaffAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(createDto.FirstName, result.FirstName);
        Assert.Equal(createDto.LastName, result.LastName);
        Assert.Equal(createDto.Email, result.Email);

        // Verify in database
        var staffInDb = await _context.Staff.FindAsync(result.Id);
        Assert.NotNull(staffInDb);
        Assert.Equal(createDto.FirstName, staffInDb.FirstName);
    }

    [Fact]
    public async Task UpdateStaffAsync_ShouldUpdateExistingStaff()
    {
        // Arrange
        await SeedTestData();
        var staff = await _context.Staff.FirstAsync();
        var updateDto = new UpdateStaffDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = staff.Email,
            Phone = staff.Phone,
            Department = staff.Department,
            Position = staff.Position,
            Salary = staff.Salary,
            HireDate = staff.HireDate,
            IsActive = staff.IsActive
        };

        // Act
        var result = await _staffService.UpdateStaffAsync(staff.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("Name", result.LastName);

        // Verify in database
        var updatedStaff = await _context.Staff.FindAsync(staff.Id);
        Assert.NotNull(updatedStaff);
        Assert.Equal("Updated", updatedStaff.FirstName);
        Assert.Equal("Name", updatedStaff.LastName);
    }

    [Fact]
    public async Task DeleteStaffAsync_ShouldDeleteStaff_WhenExists()
    {
        // Arrange
        await SeedTestData();
        var staff = await _context.Staff.FirstAsync();

        // Act
        var result = await _staffService.DeleteStaffAsync(staff.Id);

        // Assert
        Assert.True(result);

        // Verify deletion
        var deletedStaff = await _context.Staff.FindAsync(staff.Id);
        Assert.Null(deletedStaff);
    }

    [Fact]
    public async Task DeleteStaffAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Act
        var result = await _staffService.DeleteStaffAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchStaffAsync_ShouldReturnFilteredResults()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _staffService.SearchStaffAsync("jane@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains("Jane", result.First().FirstName);
    }

    [Fact]
    public async Task GetDepartmentsAsync_ShouldReturnUniqueDepartments()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _staffService.GetDepartmentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(result.Count, result.Distinct().Count()); // All should be unique
    }

    [Fact]
    public async Task GetPositionsAsync_ShouldReturnUniquePositions()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _staffService.GetPositionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(result.Count, result.Distinct().Count()); // All should be unique
    }

    private async Task SeedTestData()
    {
        var staff = new List<Staff>
        {
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Phone = "555-0001",
                Department = "Engineering",
                Position = "Software Engineer",
                Salary = 80000,
                HireDate = DateTime.Today.AddYears(-2),
                IsActive = true
            },
            new()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Phone = "555-0002",
                Department = "Marketing",
                Position = "Marketing Manager",
                Salary = 75000,
                HireDate = DateTime.Today.AddYears(-1),
                IsActive = true
            },
            new()
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@example.com",
                Phone = "555-0003",
                Department = "Engineering",
                Position = "Senior Developer",
                Salary = 95000,
                HireDate = DateTime.Today.AddMonths(-6),
                IsActive = false
            }
        };

        await _context.Staff.AddRangeAsync(staff);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
