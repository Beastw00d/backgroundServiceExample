using System.Threading.Tasks;
using Platibus.Journaling;

namespace Billing.API.Repositories
{
    public interface IJournalConsumerProgressTracker
    {
        Task Update(string consumer, MessageJournalPosition next);
        Task<MessageJournalPosition> GetNext(IMessageJournal messageJournal, string consumer);
    }
}
