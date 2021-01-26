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
        private readonly IList<ReadModels.ClassifiedAdDetails> _items;
        private EventStoreAllCatchUpSubscription _subscription;

        public ProjectionManager(IEventStoreConnection connection, IList<ReadModels.ClassifiedAdDetails> items)
        {
            _connection = connection;
            _items = items;
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

            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    _items.Add(new ReadModels.ClassifiedAdDetails
                    {
                        ClassifiedAdId = e.Id
                    });
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    UpdateItem(e.Id, ad => ad.Title = e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    UpdateItem(e.Id, ad => ad.Description = e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    UpdateItem(e.Id, ad =>
                    {
                        ad.Price = e.Price;
                        ad.CurrencyCode = e.CurrencyCode;
                    });
                    break;
            }
            
            return Task.CompletedTask;
            // return Task.WhenAll(_projections.Select(x => x.Project(@event)));
        }

        private void UpdateItem(Guid id, Action<ReadModels.ClassifiedAdDetails> update)
        {
            var item = _items.FirstOrDefault(x => x.ClassifiedAdId == id);
            
            if(item == null) return;

            update(item);
        }
    }
}