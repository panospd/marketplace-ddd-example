using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class PictureId : Value<PictureId>
    {
        public PictureId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; internal set; }
    }
}