using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Models
{
    public class FormModel
    {
        [Required]
        [StringLength(15)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(15)]
        public string? LastName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Remote(action: "CheckEmail", controller: "Validation")]
        public string? Email { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 8)]
        [Remote(action: "CheckUsername", controller: "Validation")]
        public string? Username { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 8)]
        public string? Password { get; set; }
    }
}