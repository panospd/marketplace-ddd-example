using System;
using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain;
using Marketplace.Framework;
using MarketPlace.Framework;

namespace Marketplace.Api
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IClassifiedAdRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ClassifiedAdsApplicationService(
            ICurrencyLookup currencyLookup, 
            IClassifiedAdRepository repository, 
            IUnitOfWork unitOfWork)
        {
            _currencyLookup = currencyLookup;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(object command)
        {
            switch (command)
            {
                case ClassifiedAds.V1.Create cmd:
                    await HandleCreate(cmd);
                    break;
                case ClassifiedAds.V1.SetTitle cmd:
                    await HandleUpdate(cmd.Id, ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
                    break;
                case ClassifiedAds.V1.UpdateText cmd:
                    await HandleUpdate(cmd.Id, ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
                    break;
                case ClassifiedAds.V1.UpdatePrice cmd:
                    await HandleUpdate(cmd.Id,
                        ad => ad.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)));
                    break;
                case ClassifiedAds.V1.RequestToPublish cmd:
                    await HandleUpdate(cmd.Id, ad => ad.RequestToPublish());
                    break;
                default:
                    throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown");
            }
        }
        
        private async Task HandleCreate(ClassifiedAds.V1.Create cmd)
        {
            if (await _repository.Exists(new ClassifiedAdId(cmd.Id)))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var classifiedAd = new ClassifiedAd(new ClassifiedAdId(cmd.Id), new UserId(cmd.OwnerId));

            await _repository.Add(classifiedAd);
            await _unitOfWork.Commit();
        }

        private async Task HandleUpdate(Guid classifiedAdId, Action<ClassifiedAd> operation)
        {
            var classifiedAd = await _repository.Load(new ClassifiedAdId(classifiedAdId));

            if (classifiedAd == null)
                throw new InvalidOperationException($"Entity with id {classifiedAdId} cannot be found");

            operation.Invoke(classifiedAd);

            await _unitOfWork.Commit();
        }
    }
}