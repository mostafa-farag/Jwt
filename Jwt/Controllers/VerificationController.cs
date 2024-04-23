using Jwt.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;

[Route("api/[controller]")]
[ApiController]
public class VerificationController : ControllerBase
{
    // private readonly string _smtpServer = "smtp-mail.outlook.com";
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUsername = "AppCurrencyCheker";
    private readonly string _senderEmail = "mostafafarag1233@gmail.com";
    private readonly string _googleEmail = "the.guru.se@gmail.com";
    private readonly string _smtpPassword = "fxaxficnsbqifguc";
    
  
    private readonly AppDBContext _context;
    private readonly UserManager<ApplicationUser> _manager;
    public VerificationController(AppDBContext context, UserManager<ApplicationUser> manager)
    {
        _context=context;
        _manager=manager;
    }
    [HttpPost("send")]
    public IActionResult SendVerificationEmail([FromBody] VerificationRequest request)
    {
        try
        {
            string verificationCode = GenerateVerificationCode();

                // Create a new email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("AppCurrencyCheker", "mostafafarag1233@gmail.com")); 
                message.To.Add(new MailboxAddress("", request.Email)); 
                message.Subject = "Email Verification Code";
                message.Body = new TextPart("plain")
                {
                    Text = $"Your verification code is: {verificationCode}"
                };

               
                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort);
                    client.Authenticate(_smtpUsername, _smtpPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

            return Ok("Verification email sent successfully!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to send verification email: {ex.Message}");
        }
    }

    private void SaveVerificationCodeToDatabase(string email, string verificationCode)
    {
        // Assuming you are using Entity Framework Core
        
            var verificationEntry = new VerificationCheckRequest
            {
                Email = email,
                VerificationCode = verificationCode,
               
            };

            _context.VerificationCheckRequest.Add(verificationEntry);
            _context.SaveChanges();
        
    }

 //[HttpGet("Code")]
 //   public async Task<ActionResult<VerificationCheckRequest>> GetLastCode( string code)
 //   {
 //       var user = await _manager.FindByEmailAsync(code);

           
 //           const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
 //           var random = new Random();
 //           var code = new char[6];
 //           for (int i = 0; i < code.Length; i++)
 //           {
 //               code[i] = chars[random.Next(chars.Length)];
 //           }
 //           return new string(code);
 //   }

    private static string GenerateVerificationCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new char[6];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = chars[random.Next(chars.Length)];
        }
        return new string(code);
    }

    //    private readonly string _twilioAccountSid = "AC7c642c3fb5ee9dedf16ff749b8468019";
    //    private readonly string _twilioAuthToken = "f7ea9cb7038100e5e0ac6dfc822cf7af";
    //    private readonly string _verifyServiceSid = "ZSdf3f884f86ea92cf1d838aebd8f3f1d1";


    //    [HttpPost("send")]
    //    public IActionResult SendOtp([FromBody] VerificationRequest request)
    //    {
    //        // Generate a random OTP
    //        var otp = GenerateRandomOtp();

    //        // Initialize Twilio client
    //        TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

    //        try
    //        {
    //            // Send OTP via SMS using Twilio Verify service


    //            var verification = VerificationResource.Create(to: request.PhoneNumber,channel: "sms",pathServiceSid: _verifyServiceSid);

    //            return Ok("OTP sent successfully!");
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, $"Failed to send OTP: {ex.Message}");
    //        }
    //    }
    //    private string GenerateRandomOtp()
    //    {
    //        // Generate a random 6-digit OTP
    //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    //        var random = new Random();
    //        var code = new char[6];
    //        for (int i = 0; i < code.Length; i++)
    //        {
    //            code[i] = chars[random.Next(chars.Length)];
    //        }
    //        return new string(code);
    //    }
    //}



    //    private readonly string _twilioAccountId = "AC7c642c3fb5ee9dedf16ff749b8468019";
    //    private readonly string _twilioAuthToken = "f7ea9cb7038100e5e0ac6dfc822cf7af";
    //    private readonly string _verifyServiceSid = "ZSdf3f884f86ea92cf1d838aebd8f3f1d1";

    //    [HttpPost("sent")]
    //    public IActionResult StartVerification([FromBody] VerificationRequest request)
    //    {
    //        TwilioClient.Init(_twilioAccountId, _twilioAuthToken);

    //        var verification = VerificationResource.Create(
    //            to: request.PhoneNumber,
    //            channel: "sms",
    //            pathServiceSid: _verifyServiceSid
    //        );

    //        return Ok(new { VerificationSid = verification.Sid });
    //    }

    //    [HttpPost("check")]
    //    public IActionResult CheckVerification([FromBody] VerificationCheckRequest request)
    //    {
    //        TwilioClient.Init(_twilioAccountId, _twilioAuthToken);

    //        var verificationCheck = VerificationCheckResource.Create(
    //            to: request.PhoneNumber,
    //            code: request.VerificationCode,
    //            pathServiceSid: _verifyServiceSid
    //        );

}