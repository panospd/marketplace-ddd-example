using System;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketPlace.Framework;

namespace Marketplace.Framework
{
    public interface IEntityStore
    {
        /// <summary>
        /// Loads an entity by id
        /// </summary>
        Task<T> Load<T, TId>(string entityId) where T : AggregateRoot<TId> where TId : Value<TId>;

        /// <summary>
        /// Persists an entity
        /// </summary>
        Task Save<T, TId>(T entity) where T : AggregateRoot<TId> where TId : Value<TId>;

        /// <summary>
        /// Check if entity with a given id alreadu exists
        /// <typeparam name="T">Entity type</typeparam>
        /// </summary>
        Task<bool> Exists<T>(string entityId);
    }

    public static class ApplicationServiceExtensions
    {
        public static async Task HandleUpdate<T, TId>(
            this IApplicationService service, 
            IAggregateStore store, 
            TId aggregateId, 
            Action<T> operation)
            where T : AggregateRoot<TId>
        {
            var aggregate = await store.Load<T, TId>(aggregateId);

            if (aggregate == null)
                throw new InvalidOperationException($"Entity with id {aggregateId.ToString()} cannot be found");

            operation(aggregate);

            await store.Save<T, TId>(aggregate);
        }
    }
}