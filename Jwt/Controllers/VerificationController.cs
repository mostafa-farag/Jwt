using Jwt.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Jwt.Controllers;

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

            SaveVerificationCodeToDatabase(request.Email, verificationCode);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpUsername, _senderEmail));
            message.To.Add(new MailboxAddress("", request.Email));
            message.Subject = "Email Verification Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {verificationCode}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(_googleEmail, _smtpPassword);
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

 [HttpGet("Code")]
    public async Task<ActionResult<VerificationCheckRequest>> GetLastCode( string code)
    {
        var user = await _manager.FindByEmailAsync(code);

        if (user!=null)
        {
          var Codetoken=await _manager.GeneratePasswordResetTokenAsync(user);
        }
          
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(" Code cannot be empty");
        }

        var lastData = _context.VerificationCheckRequest.OrderByDescending(v => v.Id).Select(v => v.VerificationCode).FirstOrDefault();


        if (code!=lastData)
        {
            return BadRequest("The Code Is False");
        }
        return Ok("The Code Is True");
    }
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

}