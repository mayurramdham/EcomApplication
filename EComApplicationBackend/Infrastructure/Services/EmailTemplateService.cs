using App.core.Interface;
using System;

namespace Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GenerateRegistrationEmail(string name, string username, string password)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome Email</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f4f4f9; color: #333;'>
    <div style='width: 100%; max-width: 600px; margin: 20px auto; background: #fff; border-radius: 10px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); overflow: hidden;'>
        <div style='background: #4caf50; color: #fff; padding: 20px; text-align: center; font-size: 1.5rem;'>
            Welcome to Our E-Commerce Platform
        </div>
        <div style='padding: 20px;'>
            <p>Hello,</p>
            <p>Dear {name}, Your account has been successfully created. Below are your login details:</p>
            <div style='background: #f9f9f9; padding: 10px; border: 1px solid #ddd; border-radius: 5px;'>
                <p><strong>Username:</strong> {username}</p>
                <p><strong>Password:</strong> {password}</p>
            </div>
            <p>Please make sure to change your password after your first login for security reasons.</p>
            <p>Thank you for joining us. If you have any questions, feel free to <a href='mailto:support@example.com' style='color: #4caf50; text-decoration: none;'>contact our support team</a>.</p>
        </div>
        <div style='text-align: center; padding: 15px; font-size: 0.9rem; color: #666; background: #f1f1f1;'>
            Best regards,<br>
            <strong>The E-Commerce Team</strong><br>
            <a href='https://www.example.com' style='color: #4caf50; text-decoration: none;'>Visit Our Website</a>
        </div>
    </div>
</body>
</html>";
        }
    }
}
