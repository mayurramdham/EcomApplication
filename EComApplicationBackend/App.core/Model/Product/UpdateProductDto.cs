using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Product
{
    public class UpdateProductDto
    {
        public int PrId { get; set; }
        public string PrName { get; set; }
        public IFormFile PrImageFile { get; set; }
        public string PrCategory { get; set; }
        public string PrBrand { get; set; }
        public int SellingPrice { get; set; }
        public int PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Stock { get; set; }
    }
}
