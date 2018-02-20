using MongoDB.Bson.Serialization.Attributes;

namespace Billing.API.Model
{
    public class Invoice
    {
        [BsonId]
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
