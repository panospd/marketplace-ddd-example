using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;

namespace Marketplace.ClassifiedAd
{
    [Route("/ad")]
    public class ClassifiedAdsQueryApi : Controller
    {
        private readonly IAsyncDocumentSession _session;
        private static readonly ILogger _log = Log.ForContext<ClassifiedAdsQueryApi>();

        public ClassifiedAdsQueryApi(IAsyncDocumentSession session)
        {
            _session = session;
        }

        [HttpGet]
        public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request)
        {
            return RequestHandler.HandleQuery(() => _session.Query(request), _log);
        }
    }
}