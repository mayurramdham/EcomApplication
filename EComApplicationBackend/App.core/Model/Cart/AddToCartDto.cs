using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Cart
{
    public class AddToCartDto
    {
        public int PrId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
