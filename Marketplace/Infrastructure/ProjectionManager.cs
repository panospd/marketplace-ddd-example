using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
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

        public ProjectionManager(IEventStoreConnection connection, params IProjection[] projections)
        {
            _connection = connection;
            _projections = projections;
        }

        public void Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500, Log.IsEnabled(LogEventLevel.Verbose), true, "try-out-subscription");

            _subscription = _connection.SubscribeToAllFrom(Position.Start, settings, EventAppeared);
        }
        
        public void Stop() => _subscription.Stop();
        
        private Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return Task.CompletedTask;
            
            var @event = resolvedEvent.Deserialize();
            
            Log.Debug("Projecting event {type}", @event.GetType().Name);
            
            return Task.WhenAll(_projections.Select(x => x.Project(@event)));
        }
    }
}