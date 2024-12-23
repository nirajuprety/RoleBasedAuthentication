using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RolesBaseIdentification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeApiController:ControllerBase
    {
        [HttpGet("all-employees")]
        [Authorize(Roles = "Super Admin")]
        public IActionResult GetAllEmployees()
        {
            var employees = new[]
            {
            new { Name = "John Doe", Role = "Super Admin", Email = "superadmin@example.com" },
            new { Name = "Jane Smith", Role = "Staff", Email = "staff@example.com" },
            new { Name = "Bob Brown", Role = "Low-Level Staff", Email = "lowlevelstaff@example.com" },
            new { Name = "Alice Johnson", Role = "User", Email = "user@example.com" }
        };

            return Ok(employees);
        }

        [HttpGet("staff-details")]
        [Authorize(Roles = "Super Admin,Staff")]
        public IActionResult GetStaffDetails()
        {
            var staffDetails = new[]
            {
            new { Name = "Jane Smith", Role = "Staff", Email = "staff@example.com" },
            new { Name = "Bob Brown", Role = "Low-Level Staff", Email = "lowlevelstaff@example.com" }
        };

            return Ok(staffDetails);
        }

        [HttpGet("low-level-staff-details")]
        [Authorize(Roles = "Super Admin,Staff,Low-Level Staff")]
        public IActionResult GetLowLevelStaffDetails()
        {
            var lowLevelStaffDetails = new[]
            {
            new { Name = "Bob Brown", Role = "Low-Level Staff", Email = "lowlevelstaff@example.com" }
        };

            return Ok(lowLevelStaffDetails);
        }

        [HttpGet("user-details")]
        [Authorize(Roles = "Super Admin,Staff,Low-Level Staff,User")]
        public IActionResult GetUserDetails()
        {
            var userDetails = new[]
            {
            new { Name = "Alice Johnson", Role = "User", Email = "user@example.com" }
        };

            return Ok(userDetails);
        }
    }
}
