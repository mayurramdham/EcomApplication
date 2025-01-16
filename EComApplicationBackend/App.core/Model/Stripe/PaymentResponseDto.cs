using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Model.Stripe
{
    public class PaymentResponseDto
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Id { get; set; }   
    }

}
