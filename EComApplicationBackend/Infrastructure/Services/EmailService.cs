
using App.core.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toE, string name, string subject, string message)
        {
            
            var smtpHost = _configuration["SMTP:Host"]; 
            var smtpPort = int.Parse(_configuration["SMTP:Port"]); 
            var smtpUser = _configuration["SMTP:Username"]; 
            var smtpPass = _configuration["SMTP:Password"]; 
            var fromEmail = _configuration["SMTP:FromEmail"]; 
            var fromName = _configuration["SMTP:FromName"]; 

            try
            {
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true 
                };

                mailMessage.To.Add(new MailAddress(toE, name));

                
                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtpClient.EnableSsl = true; 

                    
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return true; 
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; 
            }
        }
    }
}
