namespace Jwt.Models
{
    public class VerificationCheckRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }
      
    }
}
