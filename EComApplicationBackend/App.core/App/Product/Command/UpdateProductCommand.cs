using App.core.Interface;
using App.core.Model;
using App.core.Model.Product;
using Domain.Entity.Register;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.Product.Command
{
    public class UpdateProductCommand:IRequest<object>
    {
        public UpdateProductDto Product { get; set; }
    }
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IImageService _imageService;
        public UpdateProductCommandHandler(IAppDbContext appDbContext, IImageService imageService)
        {
            _appDbContext = appDbContext;
            _imageService = imageService;
        }

        public async Task<object> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
           var productData=request.Product;
            if (productData is  null)
            {
              return  "Provide all the data";
            }
            var selectedProduct = await _appDbContext.Set<Domain.Entity.Products.Product>()
                                       .FirstOrDefaultAsync(p => p.PrId == productData.PrId);
            // var updateProduct=await _appDbContext.Set<Domain.Entity.Products.Product>().Adapt(selectedProduct);

            var imageUploadResult = await _imageService.Upload(productData.PrImageFile);
            if (imageUploadResult is ResponseDto uploadResponse && uploadResponse.Status != 200)
            {
                return uploadResponse;
            }

             string uploadedImageUrl = (imageUploadResult as ResponseDto)?.Data?.ToString();

            //var productUpdated=  productData.Adapt(selectedProduct);
            selectedProduct.PrImage = uploadedImageUrl;
            //    _appDbContext.Set<Domain.Entity.Products.Product>().Update(productUpdated);
            //_appDbContext.Set<Domain.Entity.Products.Product>()
            selectedProduct.PrName = productData.PrName;
            selectedProduct.PrCategory = productData.PrCategory;
            selectedProduct.PrBrand = productData.PrBrand;
            selectedProduct.SellingPrice = productData.SellingPrice;
            selectedProduct.PurchasePrice = productData.PurchasePrice;
            selectedProduct.PurchaseDate = productData.PurchaseDate;
            selectedProduct.Stock = productData.Stock;

            await _appDbContext.SaveChangesAsync();
            var response = new
            {
                status = 200,
                message = "Product updated successfully",
                data = selectedProduct
            };
            return response;
        }
    }

}
