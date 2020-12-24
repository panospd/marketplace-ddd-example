using System;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using MarketPlace.Framework;

namespace Marketplace.Domain.UserProfile
{
    public class DisplayName : Value<DisplayName>
    {
        public DisplayName(string value)
        {
            Value = value;
        }

        public string Value { get; internal set; }

        public static DisplayName FromString(string displayName, ContentModeration.CheckTextForProfanity hasProfanity)
        {
            if(displayName.IsEmpty())
                throw new ArgumentNullException(nameof(displayName));
            
            if(hasProfanity(displayName))
                throw new DomainExceptions.ProfanityFound(displayName);
            
            return new DisplayName(displayName);
        }

        public static implicit operator string(DisplayName displayName) => displayName.Value;
        
        protected DisplayName() {}
    }
}