using System.Data.Common;
using System.Threading.Tasks;
using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.ClassifiedAd.efcore
{
    [Route("/ad")]
    public class ClassifiedAdsQueryApiEF : Controller
    {
        private static ILogger _log = Log.ForContext<ClassifiedAdsQueryApiEF>();
        
        private readonly DbConnection _connection;

        public ClassifiedAdsQueryApiEF(DbConnection connection)
            => _connection = connection;

        [HttpGet]
        [Route("list")]
        public Task<IActionResult> Get(QueryModels.GetPublishedClassifiedAds request) 
            => RequestHandler.HandleQuery(() => _connection.Query(request), _log);

        [HttpGet]
        [Route("myads")]
        public Task<IActionResult> Get(QueryModels.GetOwnersClassifiedAd request)
            => RequestHandler.HandleQuery(() => _connection.Query(request), _log);

        [HttpGet]
        public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request)
            => RequestHandler.HandleQuery(() => _connection.Query(request), _log);
    }
}