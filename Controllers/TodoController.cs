using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Entities;
using TodoList.Models;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("todos")]
    [Authorize(Policy = "UserOnly")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TodoController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] TodoModel todoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentClaim = HttpContext.User.Claims.First(c => c.Type == "ID");
            var userId = Guid.Parse(currentClaim.Value);

            var todos = _applicationDbContext.Set<Todo>();

            await todos.AddAsync(new Todo
            {
                Title = todoModel.Title,
                Description = todoModel.Description,
                IsDone = todoModel.IsDone,
                UserId = userId
            });

            await _applicationDbContext.SaveChangesAsync();

            return Created(nameof(TodoController), null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModifyAsync([FromRoute] int id, [FromBody] TodoModel todoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todos = _applicationDbContext.Set<Todo>();

            var myTodo = await todos.FirstAsync(t => t.Id == id);

            myTodo.Title = todoModel.Title;
            myTodo.Description = todoModel.Description;
            myTodo.IsDone = todoModel.IsDone;

            await _applicationDbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] int id)
        {
            var todos = _applicationDbContext.Set<Todo>();

            var myTodo = await todos.FirstAsync(t => t.Id == id);

            todos.Remove(myTodo);

            await _applicationDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}