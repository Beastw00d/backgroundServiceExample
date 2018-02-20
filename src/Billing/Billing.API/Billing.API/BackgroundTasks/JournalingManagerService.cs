using System.Threading;
using System.Threading.Tasks;
using Billing.API.BackgroundTasks.Base;
using Billing.API.Services;

namespace Billing.API.BackgroundTasks
{
    public class JournalingManagerService : BackgroundService
    {
        private readonly IJournalingUpdateService _journalingUpdateService;

        private const int CheckUpdateTime = 3000;

        public JournalingManagerService(IJournalingUpdateService journalingUpdateService)
        {
            _journalingUpdateService = journalingUpdateService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _journalingUpdateService.Update(stoppingToken);
                await Task.Delay(CheckUpdateTime, stoppingToken);
            }

            await Task.CompletedTask;
        }
    }
}
