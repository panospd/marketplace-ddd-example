using System.Threading.Tasks;

namespace MarketPlace.Framework
{
    public interface IApplicationService
    {
        Task Handle(object command);
    }
}