using App.core.Interface;
using App.core.Model.Cart;
using App.core.Model.Stripe;
using Azure;
using Domain.Entity.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace App.core.App.Cart.Command
{
    public class PaymentWithStripe : IRequest<PaymentResponseDto>
    {
        public StripeDto CartPaymentDto { get; set; }

        public class PaymentWithStripeHandler : IRequestHandler<PaymentWithStripe, PaymentResponseDto>
        {
            private readonly IAppDbContext _appDbContext;
            private readonly IConfiguration _configuration;
            private readonly IStripePaymentService _stripePaymentService;

            public PaymentWithStripeHandler(IAppDbContext appDbContext, IConfiguration configuration, IStripePaymentService stripePaymentService)
            {
                _appDbContext = appDbContext;
                _configuration = configuration;
                _stripePaymentService = stripePaymentService;
            }

            public async Task<PaymentResponseDto> Handle(PaymentWithStripe request, CancellationToken cancellationToken)
            {
                var paymentAndOrderDto = request.CartPaymentDto;

                var user = await _appDbContext.Set<Domain.Entity.Register.User>()
                    .FirstOrDefaultAsync(u => u.UserId == paymentAndOrderDto.UserId, cancellationToken);

                if(user is null)
                {
                    return new PaymentResponseDto
                    {
                        Message = "User not found",
                        Status = 404
                    };
                }


                //var cartDetailsList = await (from cartMaster in _appDbContext.Set<Domain.Entity.Products.CartMaster>()
                //                             join cartDetails in _appDbContext.Set<Domain.Entity.Products.CartDetails>()
                //                             on cartMaster.CartId equals cartDetails.CartId
                //                             where (cartMaster.UserId == paymentAndOrderDto.UserId)
                //                             select new Domain.Entity.Products.CartDetails
                //                             {
                //                                 Id = cartDetails.CartId,
                //                                 CartId = cartDetails.CartId,
                //                                 CartMaster = cartDetails.CartMaster,
                //                                 Product = cartDetails.Product,
                //                                 Quantity = cartDetails.Quantity,
                //                                 PrId = cartDetails.PrId,
                //                             }).ToListAsync(cancellationToken: cancellationToken);


                var cartDetailsList = await (from cartMaster in _appDbContext.Set<Domain.Entity.Products.CartMaster>()
                                             join cartDetail in _appDbContext.Set<Domain.Entity.Products.CartDetails>()
                                             on cartMaster.CartId equals cartDetail.CartId
                                             where (cartMaster.UserId == paymentAndOrderDto.UserId)
                                             select cartDetail).ToListAsync(cancellationToken: cancellationToken);



                if (!cartDetailsList.Any())
                    return new PaymentResponseDto { Message= $"No Item in Cart" };


                float subTotal = 0;
                foreach (var item in cartDetailsList)
                {
                    var product = await _appDbContext.Set<Domain.Entity.Products.Product>()
                                        .FirstOrDefaultAsync(p => p.PrId == item.PrId, cancellationToken: cancellationToken);

                    if (product is null || !(product.Stock >= item.Quantity))
                    {
                        return new PaymentResponseDto{ Status = 404, Message = "Item not in the stock", Data = product };
                    }

                    // Find Subtotal
                    subTotal += product.SellingPrice * (item.Quantity);
                   
                }

                var paymentInfo = await _stripePaymentService.CreateStripePayment(paymentAndOrderDto);
                if (paymentInfo is null)
                {
                    return new PaymentResponseDto
                    {
                       Message = "Your Payment Fail",
                       Status=500
                    };
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
                    PaymentId = paymentInfo.Id,

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


                    if (string.IsNullOrWhiteSpace(paymentAndOrderDto.SourceToken) || paymentAndOrderDto.Amount <= 0)
                {
                    return new PaymentResponseDto { Status = 400, Message = "Invalid payment details" };
                }

                    
                  _appDbContext.Set<Domain.Entity.Products.CartDetails>()
                   .Remove(item);

                    await _appDbContext.SaveChangesAsync(cancellationToken);
                  
                    
                    
                }


                return new PaymentResponseDto
                {
                    Status = 200,
                    Message = "Card Order Placed Successfully",
                    Data = salesMaster
                };
               


            }
        }
    }
}
