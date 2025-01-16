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
    public class IncrementCartCommand:IRequest<object>
    {
        public IncrementCartQuantityDto incrementCartQuantityDto;
    }
    public class IncrementCartCommandHandler : IRequestHandler<IncrementCartCommand,object>
    {
        private readonly IAppDbContext _appDbContext;
        public IncrementCartCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> Handle(IncrementCartCommand request, CancellationToken cancellationToken)
        {
            var productRequest = request.incrementCartQuantityDto;
            var checkProduct = await _appDbContext.Set<Domain.Entity.Products.CartDetails>().
                                       FirstOrDefaultAsync(p => p.CartId == productRequest.CartId && p.PrId==productRequest.PrId,cancellationToken);

            var product= await _appDbContext.Set<Domain.Entity.Products.Product>().
                                FirstOrDefaultAsync(p=>p.PrId== productRequest.PrId,cancellationToken);
            if (checkProduct is null)
            {
                return new { status = 404, message = "Item not found in the cart", data = productRequest };
            }

            if ((checkProduct.Quantity+productRequest.Quantity) > product.Stock)
            {
                return new { status = 404, message = "product not in stock",  };
            }

           
            checkProduct.Quantity += productRequest.Quantity;
            await _appDbContext.SaveChangesAsync(cancellationToken);
            var response = new
            {
                status = 200,
                message = "Quanity updated successfully",
                cartQuantity = checkProduct.Quantity,
                avaibleStock=product.Stock
            };
            return response;

                                  
        }
    }
}
