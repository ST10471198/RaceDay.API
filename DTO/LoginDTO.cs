using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTO
{
    public class LoginDto
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}