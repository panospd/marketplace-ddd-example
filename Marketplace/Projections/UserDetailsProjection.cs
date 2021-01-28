using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class UserDetailsProjection : RavenDbProjection<ReadModels.UserDetails>
    {
        public UserDetailsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public override Task Project(object @event)
        {
            switch (@event)
            {
                case Events.UserRegistered e:
                    Create(() => Task.FromResult(new ReadModels.UserDetails
                    {
                        UserId = e.UserId.ToString(),
                        DisplayName = e.DisplayName
                    }));
                    break;
                case Events.UserDisplayNameUpdated e:
                    UpdateOne(e.UserId, i => i.DisplayName = e.DisplayName);
                    break;
                case Events.ProfilePhotoUploaded e:
                    UpdateOne(e.UserId, i => i.PhotoUrl = e.PhotoUrl);
                    break;
            }
            
            return Task.CompletedTask;
        }
    }
}