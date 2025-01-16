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
    public class AddToCartCommand : IRequest<object>
    {
        public AddToCartDto AddToCartDto { get; set; }
    }
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        public AddToCartCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<object> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var cartDto     =  request.AddToCartDto;
            var checkUserId =  await _appDbContext.Set<CartMaster>()
                              .FirstOrDefaultAsync(c => c.UserId == cartDto.UserId);
            //checkUserId.UserId= cartDto.UserId;
            if (checkUserId is null)
            {
                //checkUserId.UserId = cartDto.UserId;
                var cartMaster = new CartMaster()
                {
                    UserId = cartDto.UserId,
                };
                await _appDbContext.Set<CartMaster>()
                              .AddAsync(cartMaster);

                await _appDbContext.SaveChangesAsync();
                checkUserId = cartMaster;
            }
            var product = await _appDbContext.Set<Domain.Entity.Products.Product>()
                        .FirstOrDefaultAsync(p => p.PrId == cartDto.PrId);

            if (product is null)
            {
                return "Product not found";
            }

            var cartDetails = new CartDetails()
            {
                CartId = checkUserId.CartId,
                PrId = cartDto.PrId,
                Quantity = cartDto.Quantity,

            };
            await _appDbContext.Set<CartDetails>()
                .AddAsync(cartDetails);
            await _appDbContext.SaveChangesAsync();


            var response = new
            {
                status = 200,
                CartMaster = checkUserId,
                Product = product,
                Quanity = cartDto.Quantity

            };
            return response;


        }
    }
}
