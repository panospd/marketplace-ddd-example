using System.Threading.Tasks;
using Marketplace.Framework;

namespace Marketplace.Infrastructure
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ClassifiedAdDbContext _dbContext;

        public EfUnitOfWork(ClassifiedAdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Commit()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}