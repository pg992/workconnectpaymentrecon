using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PaymentReconciliation.Services;
using System.Collections.Generic;
using PaymentReconciliation.Contracts;

namespace PaymentReconciliation
{
    public class PaymentReconciliation
    {
        private readonly IPaypalService _paypalService;

        public PaymentReconciliation(IPaypalService paypalService)
        {
            _paypalService = paypalService;
        }

        [FunctionName("PaymentReconciliation")]
        public async Task<IActionResult> PaymentReconciliationProcessAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] List<Payments> dbPayments)
        {
            _paypalService.TestOffice365();
            var paypalTransactionDetails = await _paypalService.GetTransactionsAsync();
            var ordersToBeInserted = await _paypalService.CompareAsync(paypalTransactionDetails, dbPayments);
            return new OkObjectResult(ordersToBeInserted);
        }
    }
}
