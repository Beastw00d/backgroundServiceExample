namespace Customer.API.IntegrationEvents.Events
{
    public class CustomerNameChangedEvent
    {
        public string CustomerId { get; private set; }
        public string NewCustomerName { get; private set; }

        public CustomerNameChangedEvent(string customerId, string newCustomerName)
        {
            CustomerId = customerId;
            NewCustomerName = newCustomerName;
        }
    }
}
