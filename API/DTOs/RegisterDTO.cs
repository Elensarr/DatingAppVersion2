using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required] // validation, part of API
        public string Password { get; set; }
    }
}
