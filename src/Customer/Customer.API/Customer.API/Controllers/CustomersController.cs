using System.Net;
using System.Threading.Tasks;
using Customer.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Platibus;

namespace Customer.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class CustomersController : Controller
    {
        private readonly IBus _bus;
        public CustomersController(IBus bus)
        {
            _bus = bus;
        }
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateCustomer([FromBody] Model.Customer customer)
        {
            var customerNameChangedEvent = new CustomerNameChangedEvent(customer.Id, customer.Name);
            await _bus.Publish(customerNameChangedEvent, "CustomerEvents");
            return Ok();
        }
    }
}
