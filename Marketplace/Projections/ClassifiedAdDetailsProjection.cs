using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.UserProfile.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;

namespace Marketplace.Projections
{
    public class ClassifiedAdDetailsProjection : RavenDbProjection<ReadModels.ClassifiedAdDetails>
    {
        
        private readonly Func<Guid, Task<string>> _getUserDisplayName;

        public ClassifiedAdDetailsProjection(Func<IAsyncDocumentSession> getSession, Func<Guid, Task<string>> getUserDisplayName)
            : base(getSession)
        {
            _getUserDisplayName = getUserDisplayName;
        }

        public override Task Project(object @event)
        {
            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    Create(async () => new ReadModels.ClassifiedAdDetails
                    {
                        Id = e.Id.ToString(),
                        SellerId = e.OwnerId,
                        SellersDisplayName = await _getUserDisplayName(e.OwnerId)
                    });
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    UpdateOne(e.Id, ad => ad.Title = e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    UpdateOne(e.Id, ad => ad.Description = e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    UpdateOne(e.Id, ad =>
                    {
                        ad.Price = e.Price;
                        ad.CurrencyCode = e.CurrencyCode;
                    });
                    break;
                case UserDisplayNameUpdated e:
                    UpdateWhere(item => item.SellerId == e.UserId, item => item.SellersDisplayName = e.DisplayName);
                    break;
                case V1.ClassifiedAdPublished e:
                    UpdateOne(e.Id, ad => ad.SellersPhotoUrl = e.SellersPhotoUrl);
                    break;
            }
            
            return Task.CompletedTask;
        }
    }
}