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
    public class getCartItermCount : IRequest<object>
    {
        public int UserId { get; set; }
    }
    public class getCartItermCountHandler : IRequestHandler<getCartItermCount,object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public getCartItermCountHandler(IAppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public async Task<object> Handle(getCartItermCount request, CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            const string sql = @"select cd.PrId from CartMaster CM
                                 inner join CartDetails CD on CM.CartId = CD.CartId
                                where CM.UserId = @UserId";

            var parameters = new { UserId = request.UserId };

            // Execute the query using Dapper
            var cartCount = await connection.QueryAsync<int>(sql, parameters);

            // Map the result to a response object
            var response = new
            {
                message = "Invoice details retrieved successfully.",
                status = 200,
                CartProductId = cartCount
            };

            return response;
        }

        
    
    }
}
 