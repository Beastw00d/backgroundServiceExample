using MongoDB.Bson.Serialization.Attributes;

namespace Customer.API.Model
{
    public class Customer
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
