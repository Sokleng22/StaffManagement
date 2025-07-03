using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StaffManagement.SharedLib.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace StaffManagement.Tests.IntegrationTests
{
    public class StaffControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public StaffControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAllStaff_ShouldReturnStaffList()
        {
            // Act
            var response = await _client.GetAsync("/api/staff");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<StaffDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.True(result.TotalCount > 0);
            Assert.NotEmpty(result.Items);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetStaffById_ShouldReturnStaff_WhenExists()
        {
            // Arrange - Get the first staff member from the seeded data
            var allStaffResponse = await _client.GetAsync("/api/staff");
            allStaffResponse.EnsureSuccessStatusCode();
            var allStaffContent = await allStaffResponse.Content.ReadAsStringAsync();
            var allStaffResult = JsonSerializer.Deserialize<PagedResult<StaffDto>>(allStaffContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var firstStaff = allStaffResult!.Items.First();

            // Act
            var response = await _client.GetAsync($"/api/staff/{firstStaff.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var staff = JsonSerializer.Deserialize<StaffDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(staff);
            Assert.Equal(firstStaff.Id, staff.Id);
            Assert.Equal(firstStaff.FirstName, staff.FirstName);
            Assert.Equal(firstStaff.LastName, staff.LastName);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateStaff_ShouldCreateNewStaff()
        {
            // Arrange
            var newStaff = new CreateStaffDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@example.com",
                Phone = "555-0123",
                Department = "Testing",
                Position = "Test Engineer",
                Salary = 70000,
                HireDate = DateTime.Today,
                IsActive = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/staff", newStaff);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var createdStaff = JsonSerializer.Deserialize<StaffDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(createdStaff);
            Assert.Equal(newStaff.FirstName, createdStaff.FirstName);
            Assert.Equal(newStaff.LastName, createdStaff.LastName);
            Assert.Equal(newStaff.Email, createdStaff.Email);
            Assert.True(createdStaff.Id > 0);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SearchStaff_ShouldReturnFilteredResults()
        {
            // Act
            var response = await _client.GetAsync("/api/staff/search?searchTerm=john");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var staff = JsonSerializer.Deserialize<List<StaffDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(staff);
            Assert.All(staff, s => Assert.Contains("john", s.FirstName.ToLower() + " " + s.LastName.ToLower() + " " + s.Email.ToLower()));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ExportToCsv_ShouldReturnCsvFile()
        {
            // Act
            var response = await _client.GetAsync("/api/staff/export/csv");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Id,FirstName,LastName", content);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetDepartments_ShouldReturnUniqueDepartments()
        {
            // Act
            var response = await _client.GetAsync("/api/staff/departments");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var departments = JsonSerializer.Deserialize<List<string>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(departments);
            Assert.NotEmpty(departments);
            Assert.Contains("Engineering", departments);
        }
    }
}
