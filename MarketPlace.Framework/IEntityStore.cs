using System.Threading.Tasks;

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
}