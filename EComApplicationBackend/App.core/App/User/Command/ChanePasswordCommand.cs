using App.core.Interface;
using App.core.Model.Register;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.User.Command
{
    public class ChangePasswordCommand : IRequest<object>
    {
        public ChangePasswordDto ChangePasswordDto { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, object>
    {
        private readonly IAppDbContext _context;

        public ChangePasswordCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<object> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var model = request.ChangePasswordDto;
            var user = await _context.Set<Domain.Entity.Register.User>()
                .FirstOrDefaultAsync(x => x.Email == model.UserName);

            if (user == null)
            {
                return false;
            }


            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            //_context.Set<Domain.User>().Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new
            {
                status = 200,
                message = "Password Change Successfully",
                data = user
            };
            return response;
        }
    }
}
