using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models.DTOs
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        public UserType? Type { get; set; }
    }
}
