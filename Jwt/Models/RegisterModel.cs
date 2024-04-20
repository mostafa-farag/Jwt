using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class RegisterModel
    {
        [Required,StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(120)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(256)]
        public string ConfirmPassword { get; set; }

        [Required, StringLength(256)]
        public string PhoneNumber { get; set; }
    }
}
