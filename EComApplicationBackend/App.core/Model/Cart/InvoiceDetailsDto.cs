using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Cart
{
    public class InvoiceDetailsDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int Zipcode { get; set; }
        public string InvoiceId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryZipCode { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryCountry { get; set; }
        public string ProductName { get; set; }
        public int ProductCode { get; set; }
        public int SalesQty { get; set; }
        public float SellingPrice { get; set; }
    }
}
