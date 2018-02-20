using System.Threading.Tasks;
using Billing.API.Model;
using Billing.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Billing.API.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public InvoicesController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        // GET api/invoices/5
        [HttpGet("{id}")]
        public async Task<Invoice> Get(string id)
        {
            return await _invoiceRepository.Get(id);
        }

        // POST api/invoices
        [HttpPost]
        public async Task<Invoice> Post([FromBody]Invoice invoice)
        {
            return await _invoiceRepository.Upsert(invoice);
        }

    }
}
