using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Infrastructure;
using Marketplace.Infrastructure.efcore;

namespace Marketplace.ClassifiedAd.efcore
{
    public class ClassifiedAdRepositoryEF : IClassifiedAdRepository
    {
        private readonly MarketPlaceDbContext _dbContext;

        public ClassifiedAdRepositoryEF(MarketPlaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(Domain.ClassifiedAd.ClassifiedAd entity)
        {
            return _dbContext.ClassifiedAds.AddAsync(entity);
        }

        public async Task<bool> Exists(ClassifiedAdId id)
        {
            return await _dbContext.ClassifiedAds.FindAsync(id.Value) != null;
        }
        
        public Task<Domain.ClassifiedAd.ClassifiedAd> Load(ClassifiedAdId id)
        {
            return _dbContext.ClassifiedAds.FindAsync(id.Value);
        }
    }
}