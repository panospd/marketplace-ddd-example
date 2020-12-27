using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;
using Marketplace.Infrastructure.efcore;

namespace Marketplace.UserProfile.efcore
{
    public class UserProfileRepositoryEF : IUserProfileRepository, IDisposable
    {
        private readonly MarketPlaceDbContext _dbContext;

        public UserProfileRepositoryEF(MarketPlaceDbContext dbContext) 
            => _dbContext = dbContext;
        
        public Task Add(Domain.UserProfile.UserProfile entity) 
            => _dbContext.UserProfiles.AddAsync(entity);

        public async Task<bool> Exists(UserId id) 
            => await _dbContext.UserProfiles.FindAsync(id.Value) != null;

        public Task<Domain.UserProfile.UserProfile> Load(UserId id)
            => _dbContext.UserProfiles.FindAsync(id.Value);

        public void Dispose() => _dbContext.Dispose();
    }
}