using System;
using System.Threading.Tasks;
using Billing.API.Model;
using MongoDB.Driver;
using Platibus.Journaling;

namespace Billing.API.Repositories
{
    public class MongoJournalConsumerProgressTracker : IJournalConsumerProgressTracker
    {
        private const string DefaultCollectionName = "journalConsumerProgress";
        private readonly IMongoCollection<JournalConsumerProgress> _collection;

        public MongoJournalConsumerProgressTracker(IMongoDatabase db, string collectionName = DefaultCollectionName)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = DefaultCollectionName;
            }
            _collection = db.GetCollection<JournalConsumerProgress>(collectionName);
        }

        public async Task Update(string consumer, MessageJournalPosition next)
        {
            if (string.IsNullOrWhiteSpace(consumer)) throw new ArgumentNullException(nameof(consumer));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var filter = Builders<JournalConsumerProgress>.Filter.Eq(x => x.Consumer, consumer.Trim().ToLower());
            var update = Builders<JournalConsumerProgress>.Update.Set(sp => sp.Next, next.ToString());
            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task<MessageJournalPosition> GetNext(IMessageJournal messageJournal, string consumer)
        {
            if (messageJournal == null) throw new ArgumentNullException(nameof(messageJournal));
            if (string.IsNullOrWhiteSpace(consumer)) throw new ArgumentNullException(nameof(consumer));

            var filter = Builders<JournalConsumerProgress>.Filter.Eq(x => x.Consumer, consumer.Trim().ToLower());
            var progress = await _collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            return progress == null || string.IsNullOrWhiteSpace(progress.Next)
                ? await messageJournal.GetBeginningOfJournal()
                : messageJournal.ParsePosition(progress.Next);
        }
    }
}