using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class CredentialsModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}