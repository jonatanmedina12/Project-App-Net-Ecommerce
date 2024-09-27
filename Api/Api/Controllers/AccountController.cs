using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Web;

namespace Api.Controllers
{

    public class AccountController : BaseController
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AccountController(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.GetEmailUser(model.Email);
            if (user == null)
                // Don't reveal that the user does not exist or is not confirmed
                return BadRequest(new { message = "El correo no existe", errors = ModelState });

            var resetLink = $"{_configuration["AppUrl"]}/reset-password?email={model.Email}";

            await SendPasswordResetEmail(user.Email, resetLink);

            return Ok();
        }
        private async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var fromAddress = new MailAddress("@outlook.com", _configuration["AppName"]);
            var toAddress = new MailAddress(email);
            string subject = "Password Reset Request";
            string body = $@"
            <h2>Password Reset Request</h2>
            <p>Dear user,</p>
            <p>You have requested to reset your password for your {_configuration["AppName"]} account. Please click the link below to reset your password:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you did not request this, please ignore this email.</p>
            <p>Best regards,<br>The {_configuration["AppName"]} Team</p>";

            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(
                    "@outlook.com",
                    _configuration["EmailSettings:Password"]);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }
        }
    
}
}
