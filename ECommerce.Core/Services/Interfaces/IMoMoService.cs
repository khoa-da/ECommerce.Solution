using ECommerce.Shared.Payload.Request.Momo;
using ECommerce.Shared.Payload.Response.Momo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface IMoMoService
    {
        string CreatePayment(MoMoRequest request);
         //Task<(Payment transaction, MomoIPNResponse iPNResponse)> HandlePaymentResponeIPN(MoMoResponse momoResponse);
    }
}
