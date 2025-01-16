using App.core.Interface;
using App.core.Model.Cart;
using Domain.Entity.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App.core.App.Cart.Command
{
    public class AddPaymentCommand : IRequest<object>
    {
        public CartPaymentDto CartPaymentDto { get; set; }
    }

    internal class AddPaymentCommandWithStripe : IRequestHandler<AddPaymentCommand, object>
    {
        private readonly IAppDbContext _appDbContext;

        public AddPaymentCommandWithStripe(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentAndOrderDto = request.CartPaymentDto;

            //Validate card
            var card = await _appDbContext.Set<Domain.Entity.Products.Card>()
                            .FirstOrDefaultAsync(c => c.CardNumber == paymentAndOrderDto.CardNumber &&
                            c.Cvv == paymentAndOrderDto.Cvv, cancellationToken);

            if (card is null)
            {
                return new { status = 404, message = "Item not found in the cart", data = card };
            }


            if (card.ExpiryDate?.ToString("MM/dd/yyyy") != paymentAndOrderDto.ExpiryDate?.ToString("MM/dd/yyyy"))
            {
                return "Wrong Expiry Date";
            }




            var cartDetailsList = await (from cartMaster in _appDbContext.Set<Domain.Entity.Products.CartMaster>()
                                         join cartDetail in _appDbContext.Set<Domain.Entity.Products.CartDetails>()
                                         on cartMaster.CartId equals cartDetail.CartId
                                         where (cartMaster.UserId == paymentAndOrderDto.UserId)
                                         select new Domain.Entity.Products.CartDetails
                                         {
                                             Id = cartDetail.Id,
                                             CartId = cartDetail.CartId,
                                             CartMaster = cartDetail.CartMaster,
                                             Product = cartDetail.Product,
                                             Quantity = cartDetail.Quantity,
                                             PrId = cartDetail.PrId,
                                         }).ToListAsync(cancellationToken: cancellationToken);

            if (!cartDetailsList.Any())
                return $"No Item in Cart";

            //var cartProducntList = new List<Domain.Entities.Product>();
            float subTotal = 0;
            foreach (var item in cartDetailsList)
            {
                var product = await _appDbContext.Set<Domain.Entity.Products.Product>()
                                    .FirstOrDefaultAsync(p => p.PrId == item.PrId, cancellationToken: cancellationToken);

                if (product is null || !(product.Stock >= item.Quantity))
                {
                    return new { status = 404, message = "Item not in the stock", data = product };
                }

                // Find Subtotal
                subTotal += product.SellingPrice * (item.Quantity);
                //cartProducntList.Add(product);
            }
            int totalSalesMasterEntity = await _appDbContext.Set<Domain.Entity.Products.SalesMaster>().
                                          CountAsync(cancellationToken: cancellationToken);
            totalSalesMasterEntity++;
            // Add entry to SalesMaster Table
            SalesMaster salesMaster = new SalesMaster()
            {
                OrderDate = DateTime.Now,
                TotalAmount = subTotal,
                DeliveryAddress = paymentAndOrderDto.Address,
                DeliveryState = paymentAndOrderDto.StateName,
                DeliveryCountry = paymentAndOrderDto.CountryName,
                DeliveryZipCode = paymentAndOrderDto.ZipCode,
                UserId = paymentAndOrderDto.UserId,
                InvoiceId = "ORD" + totalSalesMasterEntity.ToString().PadLeft(3, '0'),
            };



            // Add Entry in SalesMaster Table 
            await _appDbContext.Set<Domain.Entity.Products.SalesMaster>()
                  .AddAsync(salesMaster, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            salesMaster.InvoiceId = "ORDER" + salesMaster.SalesId.ToString().PadLeft(3, '0');
            await _appDbContext.SaveChangesAsync(cancellationToken);

            foreach (var item in cartDetailsList)
            {
                var product = await _appDbContext.Set<Domain.Entity.Products.Product>()
                                   .FirstOrDefaultAsync(p => p.PrId == item.PrId, cancellationToken: cancellationToken);

                SalesDetails salesDetails = new SalesDetails()
                {
                    InvoiceId = salesMaster.SalesId,
                    ProductCode = product.PrCode,
                    PrId = product.PrId,
                    SalesQty = item.Quantity,
                    SalesMaster = salesMaster,
                    SellingPrice = product.SellingPrice,

                };

                await _appDbContext.Set<Domain.Entity.Products.SalesDetails>()
                       .AddAsync(salesDetails, cancellationToken);

                product.Stock = product.Stock - item.Quantity;

                _appDbContext.Set<Domain.Entity.Products.CartDetails>()
                    .Remove(item);

                // Saving the all changes
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }


            var response = new
            {
                status = 200,
                message = "Card Order Placed Successfully",
                data = salesMaster
            };
            return response;


        }
    }
}



