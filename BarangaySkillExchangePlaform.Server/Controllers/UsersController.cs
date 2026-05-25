namespace BarangaySkillExchangePlaform.Server.Controllers
{
    using BarangaySkillExchangePlaform.Server.Data;
    using BarangaySkillExchangePlaform.Server.DTOs;
    using BarangaySkillExchangePlaform.Server.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return await _context.Users
                .OrderByDescending(user => user.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
        {
            var emailExists = await _context.Users.AnyAsync(user => user.Email == request.Email);

            if (emailExists)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                Address = request.Address,
                Role = request.Role,
                Status = "Active"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.FullName = request.FullName;
            user.ContactNumber = request.ContactNumber;
            user.Address = request.Address;
            user.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
