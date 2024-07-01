using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoList.Data;
using TodoList.Entities;
using TodoList.Models;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<object> _passwordHasher;

        public AuthenticationController(ApplicationDbContext applicationDbContext, IPasswordHasher<object> passwordHasher)
        {
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] FormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUserId = Guid.NewGuid();
            var hashedPassword = _passwordHasher.HashPassword(null!, formModel.Password);

            var newUser = new User
            {
                Id = newUserId,
                FirstName = formModel.FirstName,
                LastName = formModel.LastName,
                Email = formModel.Email,
                Username = formModel.Username,
                Password = hashedPassword
            };

            var users = _applicationDbContext.Set<User>();

            await users.AddAsync(newUser);
            await _applicationDbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new ("Role", "User"),
                new ("ID", newUserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Created(nameof(AuthenticationController), null);
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInAsync([FromBody] CredentialsModel credentialsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = _applicationDbContext.Set<User>();

            var user = users.FirstOrDefault(u => u.Username == credentialsModel.Username);

            if (user is null)
            {
                return BadRequest("Wrong username or password");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null!, user.Password, credentialsModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong username or password");
            }

            var claims = new List<Claim>
            {
                new ("Role", "User"),
                new ("ID", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Ok();
        }

        [HttpPost("sign-out")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }
    }
}