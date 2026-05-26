namespace BarangaySkillExchangePlaform.Server.Controllers
{
    using BarangaySkillExchangePlaform.Server.DTOs;
    using BarangaySkillExchangePlaform.Server.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var email = request.Email.Trim();

            var emailExists = await _userManager.FindByEmailAsync(email);

            if (emailExists is not null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            var user = new User
            {
                UserName = email,
                Email = email,
                FullName = request.FullName,
                ContactNumber = request.ContactNumber,
                Address = request.Address,
                Role = request.Role,
                Status = "Active"
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registration failed.",
                    errors = result.Errors.Select(error => error.Description)
                });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return CreatedAtAction(
                nameof(UsersController.GetUser),
                "Users",
                new { id = user.Id },
                new
                {
                    message = "Registration successful.",
                    user = ToAuthUser(user)
                });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (user.Status == "Suspended")
            {
                return Unauthorized(new { message = "Suspended users cannot log in." });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new
            {
                message = "Login successful.",
                user = ToAuthUser(user)
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logout successful." });
        }

        private static object ToAuthUser(User user)
        {
            return new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.ContactNumber,
                user.Address,
                user.Role,
                user.Status,
                user.CreatedAt
            };
        }
    }
}
