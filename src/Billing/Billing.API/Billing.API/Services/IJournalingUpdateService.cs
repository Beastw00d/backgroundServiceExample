using System.Threading;
using System.Threading.Tasks;

namespace Billing.API.Services
{
    public interface IJournalingUpdateService
    {
        Task Update(CancellationToken cancellationToken);
    }
}
