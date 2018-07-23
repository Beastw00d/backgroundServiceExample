using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Billing.API.IntegrationEvents.Events;
using Billing.API.IntegrationEvents.Processors;
using Billing.API.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Platibus;
using Platibus.Journaling;
using Platibus.Serialization;

namespace Billing.API.Services
{
    public class JournalingUpdateService : IJournalingUpdateService
    {
        private readonly IMessageJournal _messageJournal;
        private readonly IJournalConsumerProgressTracker _tracker;
        private readonly IJournalEventProcessor _eventProcessor;
        private readonly ISerializationService _serializationService;
        private readonly IMessageNamingService _messageNamingService;
        
        private const string Id = "JournalEventProducer";
        private const int BatchSize = 50;
        private readonly MessageJournalFilter Filter = new MessageJournalFilter
        {
            Categories = new[] { MessageJournalCategory.Published },
            Topics = new[] { new TopicName("CustomerEvents") }
        };

        public JournalingUpdateService(IMessageJournal messageJournal,
            ISerializationService serializationService,
            IJournalConsumerProgressTracker tracker,
            IMongoDatabase database,
            IMessageNamingService messageNamingService, 
            IConfiguration configuration)
        {
            _messageJournal = messageJournal;
            _serializationService = serializationService;
            _tracker = tracker;
            _messageNamingService = messageNamingService;

            _eventProcessor = new CustomerJournalEventProcessor(database);

            // MessageJournalFilter does not work with mongo2 go
            if (configuration.GetValue<bool>("UseMongo2Go"))
            {
                Filter = null;
            }
        }

        public async Task Update(CancellationToken cancellationToken)
        {
            var start = await _tracker.GetNext(_messageJournal, Id);
            var journalResult = await _messageJournal.Read(start, BatchSize, Filter, cancellationToken);

            if (journalResult.Entries.Any())
            {
                foreach (var entry in journalResult.Entries.Where(x => x != null))
                {
                    var t = _messageNamingService.GetTypeForName(GetEventName(entry));

                    if (t == null) continue;

                    var serializer = _serializationService.GetSerializer(entry.Data.Headers.ContentType);
                    var journalEvent = serializer.Deserialize(entry.Data.Content, t);


                    if (_eventProcessor.Handlers.ContainsKey(t))
                    {
                        await _eventProcessor.Handlers[t](journalEvent);
                    }
                }

                start = journalResult.Next;
                await _tracker.Update(Id, start);
            }
        }


        // This is a hacky way to avoid creating a nuget package for the 
        // integration events in the Customer API
        private string GetEventName(MessageJournalEntry messageName)
        {
            return typeof(CustomerNameChangedEvent).Namespace + '.' +
                messageName.Data.Headers.MessageName.ToString().Split('.').LastOrDefault();
        }
    }
}
