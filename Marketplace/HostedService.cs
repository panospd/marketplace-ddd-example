using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace Marketplace
{
    public class HostedService : IHostedService
    {
        private readonly IEventStoreConnection _esconnection;

        public HostedService(IEventStoreConnection esconnection)
        {
            _esconnection = esconnection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _esconnection.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _esconnection.Close();
            
            return Task.CompletedTask;
        }
    }
}