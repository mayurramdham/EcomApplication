using App.core.Interface;
using App.core.Model.Cart;
using Domain.Entity.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Cart.Command
{
    public class DecrementCartCommand : IRequest<object>
    {
        public DecrementCartQuantityDto decrementCartQuantityDto;
    }
    public class DecrementCartCommandHandler : IRequestHandler<DecrementCartCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        public DecrementCartCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> Handle(DecrementCartCommand request, CancellationToken cancellationToken)
        {
            var decrementQuantity = request.decrementCartQuantityDto;
            var checkProduct = await _appDbContext.Set<Domain.Entity.Products.CartDetails>().
                                       FirstOrDefaultAsync(p => p.CartId == decrementQuantity.CartId && p.PrId==decrementQuantity.PrId, cancellationToken);

            if (checkProduct is null)
            {
                return new { status = 404, message = "Item not found in the cart", data = decrementQuantity };
            }
            if (checkProduct.Quantity == 1)
            {

                 _appDbContext.Set<Domain.Entity.Products.CartDetails>().Remove(checkProduct);
            }
            else
            {

            checkProduct.Quantity -= decrementQuantity.Quantity;
            }


            await _appDbContext.SaveChangesAsync(cancellationToken);
            var response = new
            {
                status = 200,
                message = "Quanity updated successfully",
                cartQuantity = checkProduct.Quantity,
                
            };
            return response;
        }
    }
}
