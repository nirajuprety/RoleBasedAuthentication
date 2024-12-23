using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RolesBaseIdentification.Model.DTOs.Request;

namespace RolesBaseIdentification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleApiController:ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RoleApiController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Super Admin")] 
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roleExists = await _userManager.IsInRoleAsync(user, request.Role);
            if (roleExists)
            {
                return BadRequest(new { message = "User already assigned to this role" });
            }

            var result = await _userManager.AddToRoleAsync(user, request.Role);
            if (result.Succeeded)
            {
                return Ok(new { message = $"Role '{request.Role}' assigned to user '{request.Email}' successfully" });
            }

            return BadRequest(new { errors = result.Errors });
        }
    }
}
