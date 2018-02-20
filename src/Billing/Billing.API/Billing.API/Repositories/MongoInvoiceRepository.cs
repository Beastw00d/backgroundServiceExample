using System;
using System.Threading.Tasks;
using Billing.API.Model;
using MongoDB.Driver;

namespace Billing.API.Repositories
{
    public class MongoInvoiceRepository: IInvoiceRepository
    {
        private const string DefaultCollectionName = "invoices";
        private readonly IMongoCollection<Invoice> _collection;

        public MongoInvoiceRepository(IMongoDatabase db, string collectionName = DefaultCollectionName)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = DefaultCollectionName;
            }
            _collection = db.GetCollection<Invoice>(collectionName);
        }

        public async Task<Invoice> Get(string invoiceId)
        {
            if (string.IsNullOrEmpty(invoiceId)) throw new ArgumentNullException(nameof(invoiceId));
            var filter = Builders<Invoice>.Filter.Eq(x => x.Id, invoiceId);
            var invoice = await _collection.Find(filter).SingleAsync();
            return invoice;
        }

        public async Task<Invoice> Upsert(Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            var filter = Builders<Invoice>.Filter.Eq(x => x.Id, invoice.Id);
            await _collection.ReplaceOneAsync(filter, invoice, new UpdateOptions { IsUpsert = true});
            return invoice;
        }
    }
}
