using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billing.API.IntegrationEvents.Events;
using Billing.API.Model;
using MongoDB.Driver;

namespace Billing.API.IntegrationEvents.Processors
{
    public class CustomerJournalEventProcessor : IJournalEventProcessor
    {
        private readonly IMongoCollection<Invoice> _invoices;
        public IDictionary<Type, Func<object, Task>> Handlers { get; } = new Dictionary<Type, Func<object, Task>>();

        public CustomerJournalEventProcessor(IMongoDatabase database)
        {
            _invoices = database.GetCollection<Invoice>("invoices");

            RegisterHandler<CustomerNameChangedEvent>(OnCustomerNameChanged);
        }

        private void RegisterHandler<TEvent>(Func<TEvent, Task> handler)
        {
            Handlers.Add(typeof(TEvent), new Func<object, Task>((o) => handler((TEvent)o)));
        }

        public async Task OnCustomerNameChanged(CustomerNameChangedEvent e)
        {
            var filter = Builders<Invoice>.Filter.Eq(invoice => invoice.CustomerId, e.CustomerId);
            var update = Builders<Invoice>.Update.Set(invoice => invoice.CustomerName, e.NewCustomerName);
            await _invoices.UpdateManyAsync(filter, update);
        }
    }
}
