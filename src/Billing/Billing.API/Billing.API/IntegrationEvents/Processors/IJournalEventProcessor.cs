using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.API.IntegrationEvents.Processors
{
    public interface IJournalEventProcessor
    {
        IDictionary<Type, Func<object, Task>> Handlers { get; }
    }
}
