using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTO
{
    public class OrganiserRegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }
    }
}