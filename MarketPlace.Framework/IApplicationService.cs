using System.Threading.Tasks;
using Marketplace.Framework;

namespace MarketPlace.Framework
{
    public interface IApplicationService
    {
        Task Handle(object command);
    }

    public interface IAggregateStore
    {
        Task<bool> Exists<T, TId>(TId aggregateId);
        Task Save<T, TId>(T aggregate) where T : AggregateRoot<TId>;
        Task<T> Load<T, TId>(TId aggregateId) where T : AggregateRoot<TId>;
    }
}