using App.core.Interface;
using App.core.Model;
using App.core.Model.Product;
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
    public class AddProductCommad : IRequest<object>
    {
        public ProductDto product;
    }
    public class AddProductCommadHandler : IRequestHandler<AddProductCommad, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IImageService _imageService;
        public AddProductCommadHandler(IAppDbContext appDbContext, IImageService imageService)
        {
            _appDbContext = appDbContext;
            _imageService = imageService;
        }
        public async Task<object> Handle(AddProductCommad request, CancellationToken cancellationToken)
        {
            var productDto = request.product;

            // Upload the product image if provided
            string uploadedImageUrl = null;
            if (productDto.PrImageFile != null)
            {
                var imageResponse = await _imageService.Upload(productDto.PrImageFile);
                if (imageResponse is ResponseDto uploadResponse && uploadResponse.Status == 200)
                {
                    uploadedImageUrl = uploadResponse.Data.ToString();
                }
                else
                {
                    return imageResponse; // Return the failure response from ImageService
                }
            }

            // Adapt DTO to Product entity
            var productEntity = productDto.Adapt<Domain.Entity.Products.Product>();
            productEntity.IsDeleted = false;
            productEntity.PrImage = uploadedImageUrl; // Save the uploaded image URL
            productEntity.PrCode = await GenerateProductCodeAsync(cancellationToken); // Generate product code

            // Save product to the database
            await _appDbContext.Set<Domain.Entity.Products.Product>().AddAsync(productEntity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            var response = new
            {
                status = 200,
                message = "Product added successfully",
                data = productEntity
            };
            return response;
        }


        private async Task<int> GenerateProductCodeAsync(CancellationToken cancellationToken)
        {
            var lastProduct = await _appDbContext.Set<Domain.Entity.Products.Product>()
                .OrderByDescending(p => p.PrCode)
                .FirstOrDefaultAsync(cancellationToken);

            int newCode = 1;
            if (lastProduct != null)
            {
                newCode = lastProduct.PrCode + 1;
            }

            return newCode;
        }
    }
}