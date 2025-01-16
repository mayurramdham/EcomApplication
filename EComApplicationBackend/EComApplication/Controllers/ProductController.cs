using App.core.App.Cart;
using App.core.App.Cart.Command;
using App.core.App.Product.Command;
using App.core.App.Product.Query;
using App.core.App.User.Command;
using App.core.Model.Cart;
using App.core.Model.Product;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            // Send the request to the mediator to add the product
            var response = await _mediator.Send(new AddProductCommad { product = productDto });
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            // Send the request to the mediator to add the product
            var response = await _mediator.Send(new getAllProductQuery { });
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto products)
        {
            var update = await _mediator.Send(new UpdateProductCommand { Product = products });
            return Ok(update);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducttById(int id)
        {
            var isDeleted = await _mediator.Send(new DeleteProductCommand { PrId = id });
            return Ok(isDeleted);
        }

       


    }
}
