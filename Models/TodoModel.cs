using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class TodoModel
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsDone { get; set; }
    }
}