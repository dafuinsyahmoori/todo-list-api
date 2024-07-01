using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.DTOs;
using TodoList.Entities;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Policy = "UserOnly")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(ApplicationDbContext applicationDbContext, IPasswordHasher<object> passwordHasher)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMeAsync()
        {
            var currentClaim = HttpContext.User.Claims.First(c => c.Type == "ID");
            var userId = Guid.Parse(currentClaim.Value);

            var users = _applicationDbContext.Set<User>();

            var userDTO = await users.Select(u => new UserDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username
            })
            .FirstAsync(u => u.Id == userId);

            return Ok(userDTO);
        }

        [HttpGet("me/todos")]
        public async Task<IActionResult> GetMyTodosAsync()
        {
            var currentClaim = HttpContext.User.Claims.First(c => c.Type == "ID");
            var userId = Guid.Parse(currentClaim.Value);

            var todos = _applicationDbContext.Set<Todo>();

            var todoDTOs = await todos.Where(t => t.UserId == userId)
                .Select(t => new TodoDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsDone = t.IsDone
                })
                .ToListAsync();
            
            return Ok(todoDTOs);
        }
    }
}