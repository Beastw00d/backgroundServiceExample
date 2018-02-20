using MongoDB.Bson.Serialization.Attributes;

namespace Billing.API.Model
{
    [BsonIgnoreExtraElements]
    public class JournalConsumerProgress
    {
        [BsonId]
        public string Consumer { get; set; }

        public string Next { get; set; }
    }
}
