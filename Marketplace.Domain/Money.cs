using System;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        public static Money FromDecimal(decimal amount) => 
            new Money(amount);

        public static Money FromString(string amount) => 
            new Money(decimal.Parse(amount));

        public Money(decimal amount)
        {
            if(decimal.Round(amount, 2) != amount)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot have more that two decimals");

            Amount = amount;
        }

        public decimal Amount { get; }

        public Money Add(Money summand) => new Money(Amount + summand.Amount);

        public  Money Subtract(Money subtrahend) => 
            new Money(Amount - subtrahend.Amount);

        public static Money operator +(Money summand1, Money summand2) => 
            summand1.Add(summand2);

        public static Money operator -(Money subtrahend1, Money subtrahend2) =>
            subtrahend1.Subtract(subtrahend2);
    }
}