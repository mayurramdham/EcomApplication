
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Products
{
    public class SalesDetails
    {
        [Key]
        public int SalesDetailsId { get; set; }
        [Required]
        [ForeignKey("SalesMaster")]
        public int InvoiceId { get; set; }
        public SalesMaster SalesMaster { get; set; }
        [ForeignKey("Product")]
        public int PrId { get; set; }
       
        public Product Product { get; set; }
        [Required]
        public int ProductCode { get; set; }
        [Required]
        public int SalesQty { get; set; }
        [Required]
        public float SellingPrice { get; set; }
    }
}
