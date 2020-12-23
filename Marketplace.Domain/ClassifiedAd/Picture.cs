using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class Picture : Entity<PictureId>
    {
        // Properties to handle the persistence
        public Guid PictureId
        {
            get => Id.Value;
            set {}
        }
        
        public Picture(Action<object> applier)
            :base(applier)
        {
        }

        protected Picture()
        {
        }
        
        public ClassifiedAdId ParentId { get; private set; }
        public PictureSize Size { get; private set; }
        public string Location { get; private set; }
        public int Order { get; private set; }

        public void Resize(PictureSize newSize)
        {
            Apply(new Events.ClassifiedAdPictureResized
            {
                PictureId = Id.Value,
                ClassifiedAdId = ParentId.Value,
                Height = newSize.Height,
                Width = newSize.Width
            });
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureAddedToAClassifiedAd e:
                    ParentId = new ClassifiedAdId(e.ClassifiedAdId);
                    PictureId = e.PictureId;
                    Id = new PictureId(e.PictureId);
                    Location = e.Url;
                    Size = new PictureSize
                    {
                        Height = e.Height,
                        Width = e.Width
                    };
                    Order = e.Order;
                    break;
                case Events.ClassifiedAdPictureResized e:
                    Size = new PictureSize
                    {
                        Height = e.Height,
                        Width = e.Width
                    };
                    break;
            }
        }
    }
}