using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using MarketPlace.Framework;
using Marketplace.Projections;
using Serilog;
using Serilog.Events;
using Events = Marketplace.Domain.ClassifiedAd.Events;

namespace Marketplace.Infrastructure
{
    public class ProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly IProjection[] _projections;
        private EventStoreAllCatchUpSubscription _subscription;
        private readonly ICheckpointStore _checkpointStore; 

        public ProjectionManager(IEventStoreConnection connection, ICheckpointStore checkpointStore, params IProjection[] projections)
        {
            _connection = connection;
            _checkpointStore = checkpointStore;
            _projections = projections;
        }

        public async Task Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500, Log.IsEnabled(LogEventLevel.Verbose), false, "try-out-subscription");

            var position = await _checkpointStore.GetCheckpoint();
            
            _subscription = _connection.SubscribeToAllFrom(position, settings, EventAppeared);
        }
        
        public void Stop() => _subscription.Stop();
        
        private async Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;
            
            var @event = resolvedEvent.Deserialize();
            
            Log.Debug("Projecting event {type}", @event.GetType().Name);
            
            await Task.WhenAll(_projections.Select(x => x.Project(@event)));

            await _checkpointStore.StoreCheckpoint(resolvedEvent.OriginalPosition.Value);
        }
    }
}