using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Stripe
{
    public class StripeDto
    {
            public int UserId { get; set; }
            public string SourceToken { get; set; } // Stripe's PaymentMethod ID
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "usd";
            public string Address { get; set; }
            public string StateName { get; set; }
            public string CountryName { get; set; }
            public string ZipCode { get; set; }
           public string CustomerName { get; set; }
           public string Email { get; set; }
        }

    
}
