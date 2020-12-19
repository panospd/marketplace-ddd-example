using System;

namespace Marketplace.Domain
{
    public class Price : Money
    {
        public static Price FromDecimal(decimal amount, string currency,
            ICurrencyLookup currencyLookup) =>
            new Price(amount, currency, currencyLookup);

        internal Price(decimal amount, string currencyCode)
            : base(amount, new Currency { CurrencyCode = currencyCode })
        {
        }

        private Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
            : base(amount, currencyCode, currencyLookup)
        {
            if (amount < 0)
                throw new ArgumentException("Price cannot be negative", nameof(amount));
        }
    }
}