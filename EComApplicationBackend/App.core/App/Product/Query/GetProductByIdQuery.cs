using App.core.Interface;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Product.Query
{
    public class GetProductByIdQuery:IRequest<object>
    {
        public int PrId { get; set; }   
    }
    public class GetProductByIdQueryHandler:IRequestHandler<GetProductByIdQuery,object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public GetProductByIdQueryHandler(IAppDbContext appDbContext,IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public async Task<object> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);

            const string sql = "SELECT * FROM [Product] WHERE PrId = @PrId";

            // Execute the query
            //var user = await connection<Domain.Entity.Products.Product>(sql, new { Id = request.PrId });

            var user = await connection.QueryFirstOrDefaultAsync<Domain.Entity.Products.Product>(sql, new { PrId = request.PrId });

            if (sql == null)
            {
                return new
                {
                    message = "User not found",
                    status = 404,
                    data = (object)null
                };
            }

            var response = new
            {
                message = "Get All Product By Product By Id",
                status = 200,
                data = user
            };
            return response;
        }
    }
}
    

