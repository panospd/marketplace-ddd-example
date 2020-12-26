using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.ClassifiedAd
{
    public class ClassifiedAdsQueryApi : Controller
    {
        [HttpGet]
        [Route("list")]
        public Task<IActionResult> Get(QueryModels.GetPublishedClassifiedAds request)
        {
        }
        
        [HttpGet]
        [Route("myads")]
        public Task<IActionResult> Get(QueryModels.GetOwnersClassifiedAd request)
        {
        }

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request)
        {
        }
    }
}