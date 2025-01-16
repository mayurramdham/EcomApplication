using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Products
{
    public class Product
    {
        [Key]
        public int PrId { get; set; }
        public string PrName { get; set; }
        public string?  PrImage { get; set; }
        public int PrCode { get; set; }
        public string PrCategory {  get; set; }
        public string PrBrand { get; set; }
        public int SellingPrice { get; set; }
        public int PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Stock {  get; set; }
        public bool IsDeleted { get; set; }

    }
}
