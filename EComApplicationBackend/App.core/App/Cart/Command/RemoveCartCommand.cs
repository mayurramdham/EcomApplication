using App.core.Interface;
using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Cart.Command
{
    public class RemoveCartCommand:IRequest<object>
    {
        public int CartId { get; set; } 
    }
    public class RemoveCartCommandHandler : IRequestHandler<RemoveCartCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        public RemoveCartCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
        }

        public async Task<object> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            var cartId = request.CartId;
            var  removeCart= await _appDbContext.Set<Domain.Entity.Products.CartDetails>().FirstOrDefaultAsync(c=>c.CartId==cartId);
            if (removeCart is null)
            {
                return new { status = 404, message = "Item not found in the cart", data = cartId };
            }
            
             _appDbContext.Set<Domain.Entity.Products.CartDetails>().Remove(removeCart);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            var response = new
            {
                status = 200,
                message = $"Cart {cartId} Successfully deleted from the cart details table",
                data = cartId
            };
            return response;

        }
    }
}
