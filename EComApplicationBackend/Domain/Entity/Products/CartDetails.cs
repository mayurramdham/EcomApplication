
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Products
{
    public class CartDetails
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CartMaster")]
        public int CartId { get; set; }
        public CartMaster CartMaster { get; set; }
        [ForeignKey("Product")]
        public int PrId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
