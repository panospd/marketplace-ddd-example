﻿using System;
using System.Data;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Picture : Entity<PictureId>
    {
        public Picture(Action<object> applier)
            :base(applier)
        {
        }
        
        internal PictureSize Size { get; private set; }
        internal Uri Location { get; private set; }
        internal int Order { get; private set; }

        public void Resize(PictureSize newSize)
        {
            Apply(new Events.ClassifiedAdPictureResized
            {
                PictureId = Id.Value,
                Height = newSize.Height,
                Width = newSize.Width
            });
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureAddedToAClassifiedAd e:
                    Id = new PictureId(e.PictureId);
                    Location = new Uri(e.Url);
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

    public class PictureSize : Value<PictureSize>
    {
        public PictureSize(int width, int height)
        {
            if(width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Picture width must be a positive number");

            if(height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Picture height mist be a positive number");

            Width = width;
            Height = height;
        }

        public int Width { get; internal set; }
        public int Height { get; internal set; }

        internal PictureSize()
        {

        }
    }
}