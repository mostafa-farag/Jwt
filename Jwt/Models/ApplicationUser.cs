using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class ApplicationUser:IdentityUser 
    {
        [Required,MaxLength(100)]
        public string Firstname { get; set; }
        
        [Required, MaxLength(100)]
        public string Lastname { get; set; }
    }
}
