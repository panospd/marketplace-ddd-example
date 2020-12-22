﻿using System;
using System.Text.RegularExpressions;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        // Satisfy the serialization requirements
        protected ClassifiedAdTitle()
        {
        }
        
        public static ClassifiedAdTitle FromString(string title)
        {
            CheckValidity(title);
            return new ClassifiedAdTitle(title);
        }

        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);
            CheckValidity(value);

            return new ClassifiedAdTitle(value);
        }

        public string Value { get; internal set; }

        public static implicit operator string(ClassifiedAdTitle title) =>
            title.Value;

        internal ClassifiedAdTitle(string value)
        {
            Value = value;
        }

        private static void CheckValidity(string value)
        {
            if(value.Length > 100)
                throw new ArgumentOutOfRangeException("Title cannot be longer than 100 characters", nameof(value));
        }
        
        public static ClassifiedAdTitle NoTitle = new ClassifiedAdTitle();
    }
}