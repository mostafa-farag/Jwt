using Jwt.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace Jwt.Services;

public class ResetPasswordService : IResetPasswordService
{
    private readonly string _smtpServer = "smtp-mail.outlook.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUsername = "AppCurrencyCheker";
    private readonly string _googleEmail = "the.guru.se@gmail.com";
    private readonly string _smtpPassword = "fxaxficnsbqifguc";

    private readonly UserManager<ApplicationUser> _userManager;
    public ResetPasswordService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task ForgetPassword(VerificationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
           ?? throw new ArgumentNullException(nameof(request));

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

        using var client = new MailKit.Net.Smtp.SmtpClient();
        client.Connect(_smtpServer, _smtpPort);
        client.Authenticate(_googleEmail, _smtpPassword);
        client.Send(message);
        client.Disconnect(true);
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

    public async Task ResetPassword(ResetPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email)
           ?? throw new ArgumentNullException(nameof(model));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
    }
}