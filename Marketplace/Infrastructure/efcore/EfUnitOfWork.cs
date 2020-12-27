using System.Threading.Tasks;
using Marketplace.Framework;

namespace Marketplace.Infrastructure.efcore
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MarketPlaceDbContext _dbContext;

        public EfUnitOfWork(MarketPlaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Commit()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}