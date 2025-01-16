using App.core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.User.Command
{
    public class ForgotPasswordCommand:IRequest<object>
    {
        public string Email { get; set; }
    }
    public class ForgotPasswordCommandHandler:IRequestHandler<ForgotPasswordCommand,object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IEmailService _emailService;
        public ForgotPasswordCommandHandler(IAppDbContext appDbContext, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _emailService = emailService;
        }

        public async Task<object> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _appDbContext.Set<Domain.Entity.Register.User>()
               .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                return new
                {
                    status = 404,
                    message = "Invalid email. No user found with this email."
                };
            }
            string newPassword = GenerateRandomPassword(8);
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            // Send new password via email
            var emailSent = await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset",
                "Your new password",
                $"Your new password is: {newPassword}");

            if (!emailSent)
            {
                return new
                {
                    status = 500,
                    message = "Failed to send email. Please try again later."
                };
            }

            return new
            {
                status = 200,
                message = "New password sent successfully to your email."
            };
        }



        //Generate random password
        string GenerateRandomPassword(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var random = new Random();
                var password = new char[length];

                for (int i = 0; i < length; i++)
                {
                    password[i] = chars[random.Next(chars.Length)];
                }

                return new string(password);
            }

        }
    }
