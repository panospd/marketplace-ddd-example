using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using MarketPlace.Framework;

namespace Marketplace.ClassifiedAd
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
                case Contracts.V1.Create cmd:
                    await HandleCreate(cmd);
                    break;
                case Contracts.V1.SetTitle cmd:
                    await HandleUpdate(cmd.Id, ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
                    break;
                case Contracts.V1.UpdateText cmd:
                    await HandleUpdate(cmd.Id, ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
                    break;
                case Contracts.V1.UpdatePrice cmd:
                    await HandleUpdate(cmd.Id,
                        ad => ad.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)));
                    break;
                case Contracts.V1.RequestToPublish cmd:
                    await HandleUpdate(cmd.Id, ad => ad.RequestToPublish());
                    break;
                default:
                    throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown");
            }
        }
        
        private async Task HandleCreate(Contracts.V1.Create cmd)
        {
            if (await _repository.Exists(new ClassifiedAdId(cmd.Id)))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(new ClassifiedAdId(cmd.Id), new UserId(cmd.OwnerId));

            await _repository.Add(classifiedAd);
            await _unitOfWork.Commit();
        }

        private async Task HandleUpdate(Guid classifiedAdId, Action<Domain.ClassifiedAd.ClassifiedAd> operation)
        {
            var classifiedAd = await _repository.Load(new ClassifiedAdId(classifiedAdId));

            if (classifiedAd == null)
                throw new InvalidOperationException($"Entity with id {classifiedAdId} cannot be found");

            operation.Invoke(classifiedAd);

            await _unitOfWork.Commit();
        }
    }
}