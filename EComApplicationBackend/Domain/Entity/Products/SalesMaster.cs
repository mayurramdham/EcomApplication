using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Products
{
    public class SalesMaster
    {
        [Key]
        public int SalesId { get; set; }
        [Required]
        public string? InvoiceId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        [Required,MaxLength(6)]
        public string DeliveryZipCode { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryCountry { get; set; }
        public string PaymentId { get; set; }




    }
}
