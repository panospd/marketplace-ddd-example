using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using MarketPlace.Framework;
using static Marketplace.ClassifiedAd.Contracts;

namespace Marketplace.ClassifiedAd
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IAggregateStore _store;

        public ClassifiedAdsApplicationService(
            IAggregateStore store,
            ICurrencyLookup currencyLookup)
        {
            _currencyLookup = currencyLookup;
            _store = store;
        }

        public async Task Handle(object command)
        {
            switch (command)
            {
                case V1.Create cmd:
                    await HandleCreate(cmd);
                    break;
                case V1.SetTitle cmd:
                    await HandleUpdate(cmd.Id, ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
                    break;
                case V1.UpdateText cmd:
                    await HandleUpdate(cmd.Id, ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
                    break;
                case V1.UpdatePrice cmd:
                    await HandleUpdate(cmd.Id,
                        ad => ad.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)));
                    break;
                case V1.RequestToPublish cmd:
                    await HandleUpdate(cmd.Id, ad => ad.RequestToPublish());
                    break;
                case V1.Publish cmd:
                    await HandleUpdate(
                        cmd.Id,
                        c => c.Publish(new UserId(cmd.ApprovedBy))
                    );
                    break;
                default:
                    throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown");
            }
        }
        
        private async Task HandleCreate(V1.Create cmd)
        {
            if (await _store.Exists<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(new ClassifiedAdId(cmd.Id)))
                throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

            var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(new ClassifiedAdId(cmd.Id), new UserId(cmd.OwnerId));

            await _store.Save<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(classifiedAd);
        }

        private Task HandleUpdate(Guid id, Action<Domain.ClassifiedAd.ClassifiedAd> update)
        {
            return this.HandleUpdate(_store, new ClassifiedAdId(id), update);
        }
    }
}