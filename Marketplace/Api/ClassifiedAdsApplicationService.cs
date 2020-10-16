using System;
using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain;
using Marketplace.Framework;

namespace Marketplace.Api
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly IEntityStore _store;
        private readonly ICurrencyLookup _currencyLookup;

        public ClassifiedAdsApplicationService(IEntityStore store, ICurrencyLookup currencyLookup)
        {
            _store = store;
            _currencyLookup = currencyLookup;
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
                    await HandleUpdate(cmd.Id, ad => ad.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)));
                    break;
                case ClassifiedAds.V1.RequestToPublish cmd:
                    await HandleUpdate(cmd.Id, ad => ad.RequestToPublish());
                    break;
                default:
                    throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown");
            }
        }

        private async Task HandleUpdate(Guid classifiedAdId, Action<ClassifiedAd> operation)
        {
            var classifiedAd = await _store.Load<ClassifiedAd, ClassifiedAdId>(classifiedAdId.ToString());

            if (classifiedAd == null)
                throw new InvalidOperationException($"Entity with id {classifiedAdId} cannot be found");

            operation.Invoke(classifiedAd);

            await _store.Save<ClassifiedAd, ClassifiedAdId>(classifiedAd);
        }

        private async Task HandleCreate(ClassifiedAds.V1.Create cmd)
        {
            if (await _store.Exists<ClassifiedAd>(cmd.Id.ToString()))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var classifiedAd = new ClassifiedAd(new ClassifiedAdId(cmd.Id), new UserId(cmd.OwnerId));

            await _store.Save<ClassifiedAd, ClassifiedAdId>(classifiedAd);
        }
    }
}