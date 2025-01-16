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

namespace App.core.App.User.Query
{
    public class GetUserByIdQuery:IRequest<object>
    {
     public int UserId { get; set; }



    }
    public class GetUserByIdQueryHandler:IRequestHandler<GetUserByIdQuery,object>
    {
        private readonly IAppDbContext _appDbContext;
        public GetUserByIdQueryHandler(IAppDbContext appDbContext)
        {
             _appDbContext = appDbContext;  
        }

        public async Task<object> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user=await _appDbContext.Set<Domain.Entity.Register.User>().FirstOrDefaultAsync(u=>u.UserId == request.UserId);
            var state = await _appDbContext.Set<Domain.Entity.Register.State>()
                .FirstOrDefaultAsync(st => st.StateId == user.StateId, cancellationToken);
            var country = await _appDbContext.Set<Domain.Entity.Register.Country>()
              .FirstOrDefaultAsync(st => st.CountryId == user.CountryId, cancellationToken);
            var stateName = state.StateName;
            var countrName = country.CountryName;

            var response = new
            {
                status = 200,
                message = "User by Id",
                userData=user,
                stateName= stateName,
                countryName= countrName
            };

            return response;

        }
    }
}
