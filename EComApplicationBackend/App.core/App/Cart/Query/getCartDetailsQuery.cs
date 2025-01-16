using App.core.Interface;
using App.core.Model.Cart;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Cart.Query
{
    public class GetCartDetailsQuery : IRequest<object>
    {
        public int UserId { get; set; }

        public GetCartDetailsQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetCartDetailsQueryHandler : IRequestHandler<GetCartDetailsQuery, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public GetCartDetailsQueryHandler(IAppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public async Task<object> Handle(GetCartDetailsQuery request, CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);

            const string sql = @"
                                SELECT 
                                 cd.Id AS CartDetailsId,
                                 cd.CartId,
                                 cd.PrId,
                                 cd.Quantity,
                                 p.PrName AS ProductName,
                                  p.PrCategory AS Category,
                                  p.PrBrand AS Brand,
                                  p.SellingPrice AS Price,
                                  p.PrImage AS ImageUrl
                                  FROM CartMaster cm
                                  INNER JOIN CartDetails cd ON cm.CartId = cd.CartId
                                  INNER JOIN Product p ON cd.PrId = p.PrId
                                  WHERE cm.UserId = @UserId
                                  AND p.IsDeleted = 0";

            var parameters = new { UserId = request.UserId };

            // Execute the query using Dapper
            var cartDetails = await connection.QueryAsync<CartDetailsDto>(sql, parameters);

            var response = new
            {
                message = "Successfully retrieved cart details.",
                status = 200,
                cartItems = cartDetails.ToList()
            };

            return response;
        }
    }

}

