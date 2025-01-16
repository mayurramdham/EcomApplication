using App.core.Interface;
using App.core.Model.Register;
using Domain.Entity.Register;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.User.Command
{
    public class SendOtpCommand:IRequest<object>
    {
        public LoginDto LoginDto { get; set; }
    }
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand,object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        public SendOtpCommandHandler(IAppDbContext appDbContext, IJwtService jwtService,IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _jwtService = jwtService;
            _emailService = emailService;
        }
        public async Task<object> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = request.LoginDto;
            var userCheck = await _appDbContext.Set<Domain.Entity.Register.User>().FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (userCheck == null || !BCrypt.Net.BCrypt.Verify(user.Password, userCheck.Password) || userCheck.isDeleted == true)
            {
                return new
                {
                    status = 401,
                    message = "Invalid email or password or Roles"
                };
            }

            GenerateOtp generateOtp=new GenerateOtp();
            var otp = generateOtp.GenerateOtps();

            var emailText = $" your otp is {otp}";
            var otpdata = await _emailService.SendEmailAsync(userCheck.Email, "user", "otp for verification", emailText);
            if (!otpdata)
            {
                return new
                {
                    status = 500,
                    message = "Failed to send OTP email"
                };
            }
            DateTime now = DateTime.UtcNow;
            DateTime otpValidity = now.AddMinutes(2);
           

           

            await _appDbContext.SaveChangesAsync(cancellationToken);
            var otpTable = new Otp
            {
                OtpCode = otp,
                Expiration = otpValidity,
                UserName = userCheck.UserName,
                email=userCheck.Email

               
            };

           
            await  _appDbContext.Set<Domain.Entity.Register.Otp>().AddAsync(otpTable,cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            //var role= await _appDbContext.Set<Domain.Entity.Register.UserType>().FirstOrDefaultAsync(r => r.UserTypeId == userCheck.UserTypeId);
            //if(role is null)
            //{
            //    return "role not found";
            //}

            //Console.WriteLine("roles is ", role);
            //var accessToken = await _jwtService.Authenticate(userCheck.UserId, userCheck.Email, userCheck.FirstName, userCheck.LastName, role.UserTypeName);
            //await _appDbContext.SaveChangesAsync(cancellationToken);

            var response = new
            {
                status = 200,
                message = "Otp Sent successfuly",
                otpData=otpTable
                
            };
            return response;
        }
    }
}
