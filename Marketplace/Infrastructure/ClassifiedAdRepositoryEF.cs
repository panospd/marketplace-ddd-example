using System.Threading.Tasks;
using Marketplace.Domain;

namespace Marketplace.Infrastructure
{
    public class ClassifiedAdRepositoryEF : IClassifiedAdRepository
    {
        private readonly ClassifiedAdDbContext _dbContext;

        public ClassifiedAdRepositoryEF(ClassifiedAdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(ClassifiedAd entity)
        {
            return _dbContext.ClassifiedAds.AddAsync(entity);
        }

        public async Task<bool> Exists(ClassifiedAdId id)
        {
            return await _dbContext.ClassifiedAds.FindAsync(id.Value) != null;
        }
        
        public Task<ClassifiedAd> Load(ClassifiedAdId id)
        {
            return _dbContext.ClassifiedAds.FindAsync(id.Value);
        }
    }
}