using App.core.Interface;
using App.core.Model.Cart;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Cart.Query
{
    public class GetInvoiceDetailsQuery : IRequest<object>
    {
        public int SalesId { get; set; }


    }

    public class GetInvoiceDetailsQueryHandler : IRequestHandler<GetInvoiceDetailsQuery, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public GetInvoiceDetailsQueryHandler(IAppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public async Task<object> Handle(GetInvoiceDetailsQuery request, CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);

            // SQL query to join SalesMaster, SalesDetails, Users, and Products
            const string sql = @"
                SELECT 
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Mobile,
                    u.Address AS UserAddress,
                    u.Zipcode AS UserZipCode,
                    sm.InvoiceId,
                    sm.OrderDate,
                    sm.TotalAmount,
                    sm.DeliveryAddress,
                    sm.DeliveryZipCode,
                    sm.DeliveryState,
                    sm.DeliveryCountry,
                    p.PrName AS ProductName,
                    sd.ProductCode,
                    sd.SalesQty,
                    sd.SellingPrice
                FROM SalesMaster sm
                INNER JOIN [User] u ON sm.UserId = u.UserId
                INNER JOIN SalesDetails sd ON sm.SalesId = sd.InvoiceId
                INNER JOIN Product p ON sd.PrId = p.PrId
                WHERE sm.SalesId = @SalesId";

            var parameters = new { SalesId = request.SalesId };

            // Execute the query using Dapper
            var invoiceDetails = await connection.QueryAsync<InvoiceDetailsDto>(sql, parameters);

            // Map the result to a response object
            var groupedInvoice = invoiceDetails
          .GroupBy(x => new
          {
              x.InvoiceId,
              x.UserId,
              x.FirstName,
              x.LastName,
              x.Email,
              x.Mobile,
              x.DeliveryAddress,
              x.DeliveryZipCode,
              x.DeliveryState,
              x.DeliveryCountry,
              x.OrderDate,
              x.TotalAmount
          })
          .Select(g => new
          {
              InvoiceId = g.Key.InvoiceId,
              UserId = g.Key.UserId,
              FirstName = g.Key.FirstName,
              LastName = g.Key.LastName,
              Email = g.Key.Email,
              Mobile = g.Key.Mobile,
              DeliveryAddress = g.Key.DeliveryAddress,
              DeliveryZipCode = g.Key.DeliveryZipCode,
              DeliveryState = g.Key.DeliveryState,
              DeliveryCountry = g.Key.DeliveryCountry,
              OrderDate = g.Key.OrderDate,
              TotalAmount = g.Key.TotalAmount,
              Products = g.Select(p => new
              {
                  ProductName = p.ProductName,
                  ProductCode = p.ProductCode,
                  SalesQty = p.SalesQty,
                  SellingPrice = p.SellingPrice
              }).ToList()
          })
          .FirstOrDefault();

            // Response object
            var response = new
            {
                message = groupedInvoice != null ? "Invoice details retrieved successfully." : "Invoice not found.",
                status = groupedInvoice != null ? 200 : 404,
                invoice = groupedInvoice
            };

            return response;
        }

    }
}