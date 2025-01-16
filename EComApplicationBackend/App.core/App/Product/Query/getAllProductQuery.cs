using App.core.Interface;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Product.Query
{
    public class getAllProductQuery:IRequest<object>
    {
    }
    public class getAllProductQueryHandler : IRequestHandler<getAllProductQuery,object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public getAllProductQueryHandler(IAppDbContext appDbContext,IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        public async Task<object> Handle(getAllProductQuery request, CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);

            const string sql = "SELECT * FROM [Product] WHERE Stock>=1 and IsDeleted = 0";

            // Execute the query
            var user = await connection.QueryAsync<Domain.Entity.Products.Product>(sql);

            

            var response = new
            {
                message = "Get User By Id",
                status = 200,
                product = user
            };
            return response;
        }
        //var productList=await _appDbContext.Set<Domain.Entity.Products.Product>().Where(p => p.IsDeleted == false).ToListAsync(cancellationToken);
        //    var response = new
        //    {
        //        status = 200,
        //        message = "Product List",
        //        product = productList
        //    };
        //    return response;
        }
    }
