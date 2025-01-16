using App.core.Interface;
using App.core.Model.Register;
using BCrypt.Net;
using Domain.Entity.Register;
using Microsoft.Extensions.Configuration;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using App.core.Model;

namespace App.core.App.User.Command
{
    public class RegisterUserCommand:IRequest<object>
    {
        public UserDto UserDto { get; set; }
    }


    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IDataProtector _dataProtectionProvider;
        private readonly IEmailService _emailService;
        private readonly IImageService _imageService;
        private readonly IEmailTemplateService _emailTemplateService;

        public RegisterUserCommandHandler(IAppDbContext appDbContext,IDataProtectionProvider provider, IConfiguration configuration, IEmailService emailService,IImageService imageService,IEmailTemplateService emailTemplateService)                       
        {
            _appDbContext = appDbContext;
            _dataProtectionProvider = provider.CreateProtector(configuration["DataProtector:EnCryptionKey"]);
            _emailService = emailService;   
            _imageService = imageService;
            _emailTemplateService = emailTemplateService;
        }
        public async  Task<object> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = request.UserDto;

            var existingUser = await _appDbContext.Set<Domain.Entity.Register.User>().FirstOrDefaultAsync(u => u.Email == user.Email && u.isDeleted == false);
            if (existingUser != null)
            {
                var responses = new
                {
                    status = 409,
                    message = "User Already Exists With The Email Id"
                };
                return responses;
            }

            //add image in the handler
            var imageUploadResult = await _imageService.Upload(user.ProfileImage);
            if (imageUploadResult is ResponseDto uploadResponse && uploadResponse.Status != 200)
            {
                return uploadResponse;
            }

            string uploadedImageUrl = (imageUploadResult as ResponseDto)?.Data?.ToString();

            var name = GenerateUsername(user.FirstName, user.LastName, user.DOB);
            string plainPassword = GenerateRandomPassword();
            var newUser = user.Adapt<Domain.Entity.Register.User>();

            var userExist = await _appDbContext.Set<Domain.Entity.Register.User>().FirstOrDefaultAsync(user => user.UserName == name);

            if (userExist != null) {
                name = $"{userExist.UserName}1";
            }

            newUser.UserName = name;
            // newUser.UserName = _dataProtectionProvider.Protect(name);
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, 13);
            newUser.Password=hashPassword;
            newUser.ProfileImage = uploadedImageUrl;

          
           string emailContet = _emailTemplateService.GenerateRegistrationEmail(user.FirstName,name,plainPassword);


            var isSend = await _emailService.SendEmailAsync(newUser.Email, "Welcome to our platform  ", "Login Credentials", emailContet);
            DateTime now = DateTime.Now;
            DateTime twoMinutesLater = now.AddMinutes(5);


            await _appDbContext.Set<Domain.Entity.Register.User>().AddAsync(newUser);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            

            string GenerateRandomPassword()
            {
                return Guid.NewGuid().ToString("N").Substring(0, 8);
            }
            string GenerateUsername(string firstName, string lastName, DateTime dob)
            {
                return $"EC_{lastName.ToUpper()}{firstName.ToUpper()[0]}{dob:ddMMyy}";
            }

            var response = new
            {
                status = 200,
                message = "User Added Successfully and Credentials Send To Your Email Id",
                data = newUser,
               
            };
            return response;
        }
    }

}
