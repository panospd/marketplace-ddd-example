using System;
using System.Text.RegularExpressions;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class UserId : Value<UserId>
    {
        private readonly Guid _value;

        public UserId(Guid value)
        {
            _value = value;
        }
    }

    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        private readonly string _value;

        private ClassifiedAdTitle(string value)
        {
            if(value.Length > 100) 
                throw new ArgumentOutOfRangeException("Title cannot be longer than 100 characters", nameof(value));

            _value = value;
        }

        public static ClassifiedAdTitle FromString(string title) => 
            new ClassifiedAdTitle(title);

        private static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            return new ClassifiedAdTitle(Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty));
        }
    }
}
