using System;
using System.Text.RegularExpressions;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        private ClassifiedAdTitle(string value)
        {
            if(value.Length > 100) 
                throw new ArgumentOutOfRangeException("Title cannot be longer than 100 characters", nameof(value));

            Value = value;
        }

        public string Value { get; }

        public static ClassifiedAdTitle FromString(string title) => 
            new ClassifiedAdTitle(title);

        public static implicit operator string(ClassifiedAdTitle title) =>
            title.Value;

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