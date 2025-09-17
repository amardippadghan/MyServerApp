using Microsoft.AspNetCore.Mvc;
using MyServerApp.Models;
using MyServerApp.Models.DTOs;
using MyServerApp.Services;
using MySqlConnector;
using Dapper;

namespace MyServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;  // ADD THIS LINE

        public UsersController(IUserService userService, IConfiguration configuration)  // ADD configuration parameter
        {
            _userService = userService;
            _configuration = configuration;  // ADD THIS LINE
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        // GET: api/users/type/{type}
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByType(UserType type)
        {
            var users = await _userService.GetUsersByTypeAsync(type);
            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                return Conflict("A user with this email already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _userService.UpdateUserAsync(id, updateUserDto);
                if (!success)
                    return NotFound($"User with ID {id} not found.");

                return NoContent();
            }
            catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                return Conflict("A user with this email already exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                    return NotFound($"User with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                var result = await connection.QuerySingleAsync<string>("SELECT 'Connection successful!' as message");
                return Ok(new { message = result, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
