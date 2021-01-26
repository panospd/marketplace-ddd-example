﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;
using MarketPlace.Framework;
using Newtonsoft.Json;
using Raven.Client;

namespace Marketplace.Infrastructure
{
    public partial class EsAggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        public EsAggregateStore(IEventStoreConnection connection)
        {
            _connection = connection;
        } 

        public async Task<bool> Exists<T, TId>(TId aggregateId)
        {
            var stream = GetStreamName<T, TId>(aggregateId);
            var result = await _connection.ReadEventAsync(stream, 1, false);

            return result.Status != EventReadStatus.NoStream;
        }

        public async Task Save<T, TId>(T aggregate) where T : AggregateRoot<TId>
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            
            var changes = aggregate.GetChanges()
                .Select(@event =>
                    new EventData(
                        eventId: Guid.NewGuid(),
                        type: @event.GetType().Name,
                        isJson: true,
                        data: Serialize(@event),
                        metadata: Serialize(new EventMetadata
                            {ClrType = @event.GetType().AssemblyQualifiedName})
                    ))
                .ToArray();
            
            if (!changes.Any()) return;
            
            var streamName = GetStreamName<T, TId>(aggregate);
            
            await _connection.AppendToStreamAsync(
                streamName,
                aggregate.Version,
                changes);

            aggregate.ClearChanges();
        }

        public async Task<T> Load<T, TId>(TId aggregateId) where T : AggregateRoot<TId>
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            var stream = GetStreamName<T, TId>(aggregateId);

            var aggregate = (T) Activator.CreateInstance(typeof(T), true);

            var page = await _connection.ReadStreamEventsForwardAsync(stream, 0, 1024, false);
            
            aggregate.Load(page.Events
                .Select(resolvedEvent => resolvedEvent.Deserialize())
                .ToArray());

            return aggregate;
        }

        private static string GetStreamName<T, TId>(TId aggregateId)
        {
            return $"{typeof(T).Name}-{aggregateId.ToString()}";
        }

        private static string GetStreamName<T, TId>(T aggregate)
            where T: AggregateRoot<TId>
        {
            return $"{typeof(T).Name}-{aggregate.Id.ToString()}";
        }

        private static byte[] Serialize(object data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }
}