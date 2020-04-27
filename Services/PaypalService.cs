using Microsoft.Extensions.Configuration;
using PaymentReconciliation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentReconciliation.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly IConfiguration _configuration;
        private readonly IRestClient _restClient;
        private string _token;

        public PaypalService(IConfiguration configuration, IRestClient restClient)
        {
            _configuration = configuration;
            _restClient = restClient;
        }

        public async Task<List<Payments>> CompareAsync(List<TransactionDetail> paypalTransactionDetails, List<Payments> dbPayments)
        {
            var transactions = new List<string>();
            var orders = new List<Payments>();

            dbPayments.ForEach(p =>
            {
                var purchaseUnits = p.PurchaseUnits.ToList();
                purchaseUnits.ForEach(pu =>
                {
                    pu.Payments.Captures.ToList().ForEach(c => transactions.Add(c.Id));
                });
            });
            
            var missingTransactions = paypalTransactionDetails
                .Where(c => !transactions.Contains(c.TransactionInfo.TransactionId))
                .Select(c => c.TransactionInfo.TransactionId)
                .ToList();

            foreach(var mt in missingTransactions)
            {
                var paymentDetail = await _restClient.GetAsync<PaymentDetails>($"{_configuration["PaypalPaymentUrl"]}{mt}").ConfigureAwait(false);
                var order = await _restClient
                .GetAsync<Payments>($"{paymentDetail.Links.First(l => l.Href.AbsoluteUri.Contains("v2/checkout/orders")).Href.AbsoluteUri}")
                .ConfigureAwait(false);

                orders.Add(order);
            }
            
            return orders;
        }

        public async Task<List<TransactionDetail>> GetTransactionsAsync()
        {
            var nowDate = _configuration["ToDay"] != null ?
                DateTime.ParseExact(_configuration["ToDay"], "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture) :
                DateTime.UtcNow;
            var fromDate = _configuration["FromDay"] != null ? 
                DateTime.ParseExact(_configuration["FromDay"], "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture) : 
                nowDate.AddDays(-1);
            _token = await GetPaypalTokenAsync();
            var pageCounter = 0;
            var totalPages = 1;
            var details = new List<TransactionDetail>();
            var sb = new StringBuilder();
            _restClient.AddHeaders("Authorization", _token);
            sb.Append("?fields=all");
            sb.Append($"&page_size={_configuration["PaypalTransactionPageSize"]}");
            sb.Append($"&start_date={fromDate:yyyy-MM-dd}T00:00:00-0000");
            sb.Append($"&end_date={nowDate:yyyy-MM-dd}T23:59:59-0000");
            var queryParam = sb.ToString();

            while (pageCounter < totalPages)
            {
                var page = $"&page={pageCounter+1}";
                var response = await _restClient.GetAsync<TransactionDetails>($"{_configuration["PaypalTransactionUrl"]}{queryParam}{page}");
                details.AddRange(response.TransactionDetailsTransactionDetails);
                totalPages = response.TotalPages;
                pageCounter++;
            }

            return details;
        }

        private async Task<string> GetPaypalTokenAsync()
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
            _restClient.AddHeaders("Authorization", $"Basic {Base64Encode($"{_configuration["PaypalClientId"]}:{_configuration["PaypalClientSecret"]}")}");
            _restClient.AddHeaders("cache-control", "no-cache");
            var url = $"{_configuration["PaypalApiUrl"]}?grant_type=client_credentials";
            var (_, output) = await _restClient.ExecutePostAsync<dynamic, dynamic>(url, body, "application/x-www-form-urlencoded");
            _restClient.RemoveHeaders("Authorization");
            _restClient.RemoveHeaders("cache-control");
            return $"{output["token_type"]} {output["access_token"]}";
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
