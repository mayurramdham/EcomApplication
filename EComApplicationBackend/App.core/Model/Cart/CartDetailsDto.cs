using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Cart
{
    public class CartDetailsDto
    {
        public int CartId { get; set; }
        public int PrId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
