using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Cart
{
    public class DecrementCartQuantityDto
    {
        public int PrId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }
}
