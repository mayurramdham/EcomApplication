using App.core.Model.Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Interface
{
    public interface IStripePaymentService
    {
        Task<PaymentResponseDto> CreateStripePayment(StripeDto striprequestmodel);
    }
}
