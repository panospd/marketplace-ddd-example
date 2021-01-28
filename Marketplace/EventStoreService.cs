using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Microsoft.Extensions.Hosting;

namespace Marketplace
{
    public class EventStoreService : IHostedService
    {
        private readonly IEventStoreConnection _esConnection;
        private readonly ProjectionManager _subscription;

        public EventStoreService(IEventStoreConnection esConnection, ProjectionManager subscription)
        {
            _esConnection = esConnection;
            _subscription = subscription;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _esConnection.ConnectAsync();
            await _subscription.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription.Stop();
            _esConnection.Close();
            
            return Task.CompletedTask;
        }
    }
}