using Microsoft.AspNetCore.Mvc;
using StaffManagement.APP.Logic;
using StaffManagement.SharedLib.Models;

namespace StaffManagement.Api.Controllers
{
    /// <summary>
    /// Controller for managing staff operations including CRUD operations, search, and export functionality
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffController> _logger;

        /// <summary>
        /// Initializes a new instance of the StaffController
        /// </summary>
        /// <param name="staffService">Service for staff operations</param>
        /// <param name="logger">Logger for this controller</param>
        public StaffController(IStaffService staffService, ILogger<StaffController> logger)
        {
            _staffService = staffService;
            _logger = logger;
        }

        /// <summary>
        /// Get all staff with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<StaffDto>>> GetAllStaff([FromQuery] StaffSearchCriteria searchCriteria)
        {
            try
            {
                var result = await _staffService.GetAllStaffAsync(searchCriteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff");
                return StatusCode(500, "An error occurred while retrieving staff");
            }
        }

        /// <summary>
        /// Get staff by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetStaffById(int id)
        {
            try
            {
                var staff = await _staffService.GetStaffByIdAsync(id);
                if (staff == null)
                    return NotFound($"Staff member with ID {id} not found");

                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff by ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the staff member");
            }
        }

        /// <summary>
        /// Create a new staff member
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StaffDto>> CreateStaff([FromBody] CreateStaffDto createStaffDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var staff = await _staffService.CreateStaffAsync(createStaffDto);
                return CreatedAtAction(nameof(GetStaffById), new { id = staff.Id }, staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff");
                return StatusCode(500, "An error occurred while creating the staff member");
            }
        }

        /// <summary>
        /// Update an existing staff member
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<StaffDto>> UpdateStaff(int id, [FromBody] UpdateStaffDto updateStaffDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var staff = await _staffService.UpdateStaffAsync(id, updateStaffDto);
                if (staff == null)
                    return NotFound($"Staff member with ID {id} not found");

                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the staff member");
            }
        }

        /// <summary>
        /// Delete a staff member
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff(int id)
        {
            try
            {
                var result = await _staffService.DeleteStaffAsync(id);
                if (!result)
                    return NotFound($"Staff member with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the staff member");
            }
        }

        /// <summary>
        /// Search staff by term
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<StaffDto>>> SearchStaff([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                    return BadRequest("Search term is required");

                var staff = await _staffService.SearchStaffAsync(searchTerm);
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching staff with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching staff");
            }
        }

        /// <summary>
        /// Export staff to CSV
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<ActionResult> ExportToCsv()
        {
            try
            {
                var csvData = await _staffService.ExportStaffToCsvAsync();
                var fileName = $"staff_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting staff to CSV");
                return StatusCode(500, "An error occurred while exporting staff data");
            }
        }

        /// <summary>
        /// Export staff to Excel
        /// </summary>
        [HttpGet("export/excel")]
        public async Task<ActionResult> ExportToExcel()
        {
            try
            {
                var excelData = await _staffService.ExportStaffToExcelAsync();
                var fileName = $"staff_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting staff to Excel");
                return StatusCode(500, "An error occurred while exporting staff data");
            }
        }

        /// <summary>
        /// Get all departments
        /// </summary>
        [HttpGet("departments")]
        public async Task<ActionResult<List<string>>> GetDepartments()
        {
            try
            {
                var departments = await _staffService.GetDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting departments");
                return StatusCode(500, "An error occurred while retrieving departments");
            }
        }

        /// <summary>
        /// Get all positions
        /// </summary>
        [HttpGet("positions")]
        public async Task<ActionResult<List<string>>> GetPositions()
        {
            try
            {
                var positions = await _staffService.GetPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting positions");
                return StatusCode(500, "An error occurred while retrieving positions");
            }
        }
    }
}
