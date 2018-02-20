using System.Threading.Tasks;
using Billing.API.Model;

namespace Billing.API.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> Get(string invoiceId);
        Task<Invoice> Upsert(Invoice invoice);
    }
}
