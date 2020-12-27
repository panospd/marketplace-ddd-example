using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Marketplace.Domain.ClassifiedAd;
using static Marketplace.ClassifiedAd.QueryModels;
using static Marketplace.ClassifiedAd.ReadModels;

namespace Marketplace.ClassifiedAd.efcore
{
    public static class QueriesDapper
    {
        public static Task<IEnumerable<PublicClassifiedAdListItem>> Query(this DbConnection connection, GetPublishedClassifiedAds query)
        {
            return connection.QueryAsync<PublicClassifiedAdListItem>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\", \"Title_Value\" " +
                "FROM \"ClassifiedAds\" WHERE \"State\"=@State LIMIT @PageSize OFFSET @Offset", new
                {
                    State = (int) ClassifiedAdState.Active,
                    PageSize = query.PageSize,
                    Offset = Offset(query.Page, query.PageSize)
                });
        }
        
        public static Task<IEnumerable<PublicClassifiedAdListItem>> Query(
            this DbConnection connection,
            QueryModels.GetOwnersClassifiedAd query)
            => connection.QueryAsync<PublicClassifiedAdListItem>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\" price, \"Title_Value\" title " +
                "FROM \"ClassifiedAds\" WHERE \"OwnerId_Value\"=@OwnerId LIMIT @PageSize OFFSET @Offset",
                new
                {
                    OwnerId = query.OwnerId,
                    PageSize = query.PageSize,
                    Offset = Offset(query.Page, query.PageSize)
                });

        public static Task<ClassifiedAdDetails> Query(
            this DbConnection connection,
            QueryModels.GetPublicClassifiedAd query)
            => connection.QuerySingleOrDefaultAsync<ClassifiedAdDetails>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\" price, \"Title_Value\" title, " +
                "\"Text_Value\" description, \"DisplayName_Value\" sellersdisplayname " +
                "FROM \"ClassifiedAds\", \"UserProfiles\" " +
                "WHERE \"ClassifiedAdId\" = @Id AND \"OwnerId_Value\"=\"UserProfileId\"",
                new { Id = query.ClassifiedAdId });
        
        private static int Offset(int page, int pageSize) => page * pageSize;
    }
}