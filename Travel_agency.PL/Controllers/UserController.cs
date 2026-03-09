using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.Enums;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUserAsync();
            return Ok(_mapper.Map<List<UserResponse>>(users));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest user)
        {
            if (user == null)
            {
                return BadRequest("User data is null.");
            }

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (currentUserRole != "Administrator" && currentUserRole != "Manager" && currentUserId != id.ToString())
            {
                return Forbid();
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            var updatedUser = await _userService.UpdateUserProfileAsync(existingUser);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserResponse>(updatedUser));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPut("role/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] UserRole? userRole)
        {
            if (userRole == null)
            {
                return BadRequest("New user role is null.");
            }

            var result = await _userService.AssignUserRoleAsync(id, userRole);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
