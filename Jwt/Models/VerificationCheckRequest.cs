namespace Jwt.Models
{
    public class VerificationCheckRequest
    {
        public string PhoneNumber { get; set; }
        public string VerificationCode { get; set; }
    }
}
