using System;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        private const string DefaultCurrency = "EUR";

        public static Money FromDecimal(decimal amount, string currency, ICurrencyLookup currencyLookup) => 
            new Money(amount, currency, currencyLookup);

        public static Money FromString(string amount, string currency, ICurrencyLookup currencyLookup) => 
            new Money(decimal.Parse(amount), currency, currencyLookup);

        public Money(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        {
            if(string.IsNullOrEmpty(currencyCode))
                throw new ArgumentNullException(nameof(currencyCode), "Currency code must be specified");

            var currency = currencyLookup.FindCurrency(currencyCode);

            if(!currency.InUse)
                throw new ArgumentException($"Currency {currencyCode} is not valid");

            if (decimal.Round(amount, currency.DecimalPlaces) != amount)
                throw new ArgumentOutOfRangeException(nameof(amount), $"Amount in {currencyCode} cannot have more that {currency.DecimalPlaces} decimals");

            

            Amount = amount;
            Currency = currency;
        }

        protected Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; }
        public Currency Currency{ get; }

        public Money Add(Money summand)
        {
            if (Currency != summand.Currency)
                throw new CurrencyMisMatchException("Cannot sum amounts with different currencies");

            return new Money(Amount + summand.Amount, Currency);
        }

        public Money Subtract(Money subtrahend)
        {
            if (Currency != subtrahend.Currency)
                throw new CurrencyMisMatchException("Cannot subtract amounts with different currencies");

            return new Money(Amount - subtrahend.Amount, Currency);
        }
           

        public static Money operator +(Money summand1, Money summand2) => 
            summand1.Add(summand2);

        public static Money operator -(Money subtrahend1, Money subtrahend2) =>
            subtrahend1.Subtract(subtrahend2);

        public override string ToString()
        {
            return $"{Currency.CurrencyCode} {Amount}";
        }
    }

    public class CurrencyMisMatchException : Exception
    {
        public CurrencyMisMatchException(string message)
            : base(message)
        {
        }
    }
}