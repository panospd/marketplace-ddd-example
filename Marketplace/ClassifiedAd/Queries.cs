using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {

        public static Task<ClassifiedAdDetails> Query(this IAsyncDocumentSession session,
            QueryModels.GetPublicClassifiedAd query)
        {
            return session.LoadAsync<ClassifiedAdDetails>(query.ClassifiedAdId.ToString());
        }
    }
}