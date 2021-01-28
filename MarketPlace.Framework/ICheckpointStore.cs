using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace MarketPlace.Framework
{
    public interface ICheckpointStore
    {
        Task<Position> GetCheckpoint();
        Task StoreCheckpoint(Position position);
    }
}