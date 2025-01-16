using App.core.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Product.Command
{
    public class DeleteProductCommand:IRequest<object>
    {
        public int PrId { get; set; }
    }
    public class DeleteProductCommandHandler:IRequestHandler<DeleteProductCommand,object>
    {
        private readonly IAppDbContext _appDbContext;
        public DeleteProductCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var id = request.PrId;

            var user = await _appDbContext.Set<Domain.Entity.Products.Product>().FindAsync(id);

            if (user == null)
            {
                return false;
            }
            user.IsDeleted = true;
            await _appDbContext.SaveChangesAsync(cancellationToken);
            var response = new
            {
                status = 200,
                message = "user deleted successfully",
                data = user
            };
            return response;

        }

    }
    }
