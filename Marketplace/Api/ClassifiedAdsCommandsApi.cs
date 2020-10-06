using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private readonly ClassifiedAdsApplicationService _applicationService;

        public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService _applicationService)
        {
            this._applicationService = _applicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Contracts.ClassifiedAds.V1.Create request)
        {
            _applicationService.Handle(request);
            return Ok();
        }
    }

    public class ClassifiedAdsApplicationService
    {
        public void Handle(Contracts.ClassifiedAds.V1.Create command)
        {
            // we need to create a bew Classified Ad here
        }
    }
}