using PaymentReconciliation.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentReconciliation.Services
{
    public interface IPaypalService
    {
        Task<List<TransactionDetail>> GetTransactionsAsync();
        Task<List<Payments>> CompareAsync(List<TransactionDetail> paypalTransactionDetails, List<Payments> dbPayments);
    }
}
