using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        [NotMapped]
        public string Password { get; set; }

        public string Role { get; set; } = "HR";
    }
}