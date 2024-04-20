using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class AddRoleModel
    {
        [Required]
        public String UserID { get; set;}
        [Required]
        public String Role { get; set;}
    }
}
