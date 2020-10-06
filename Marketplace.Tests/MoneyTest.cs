﻿using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;
using Xunit;

namespace Marketplace.Tests
{
    public class MoneyTest
    {
        public static readonly ICurrencyLookup CurrencyLookup = new FakeCurrencyLookup();

        [Fact]
        public void Two_of_same_amount_should_be_equal()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", CurrencyLookup);
            var secondAmount = Money.FromDecimal(5, "EUR", CurrencyLookup);

            Assert.Equal(firstAmount, secondAmount);
        }

        [Fact]
        public void Two_of_same_amount_but_different_currency_should_not_be_equal()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", CurrencyLookup);
            var secondAmount = Money.FromDecimal(5, "USD", CurrencyLookup);

            Assert.NotEqual(firstAmount, secondAmount);
        }

        [Fact]
        public void FromString_andFromDecimal_should_be_equal()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", CurrencyLookup);
            var secondAmount = Money.FromString("5.00", "EUR", CurrencyLookup);

            Assert.Equal(firstAmount, secondAmount);
        }

        [Fact]
        public void Sum_of_money_gives_full_amount()
        {
            var coin1 = Money.FromDecimal(1, "EUR", CurrencyLookup);
            var coin2 = Money.FromDecimal(2, "EUR", CurrencyLookup);
            var coin3 = Money.FromDecimal(2, "EUR", CurrencyLookup);

            var banknote = Money.FromDecimal(5, "EUR", CurrencyLookup);

            Assert.Equal(banknote, coin1 + coin2 + coin3);
        }

        [Fact]
        public void Unused_currency_should_not_be_allowed()
        {
            Assert.Throws<ArgumentException>(() => Money.FromDecimal(100, "DEM", CurrencyLookup));
        }

        [Fact]
        public void Unknown_currency_should_not_be_allowed()
        {
            Assert.Throws<ArgumentException>(() => Money.FromDecimal(100, "What?", CurrencyLookup));
        }

        [Fact]
        public void Throw_when_too_many_decimal_places()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Money.FromDecimal(100.123m, "EUR", CurrencyLookup));
        }

        [Fact]
        public void Throw_on_adding_different_currencies()
        {
            var fistAmount = Money.FromDecimal(5, "USD", CurrencyLookup);
            var secondAmount = Money.FromDecimal(5, "EUR", CurrencyLookup);

            Assert.Throws<CurrencyMisMatchException>(() => fistAmount + secondAmount);
        }

        [Fact]
        public void Throw_on_subtracting_different_currencies()
        {
            var fistAmount = Money.FromDecimal(5, "USD", CurrencyLookup);
            var secondAmount = Money.FromDecimal(2, "EUR", CurrencyLookup);

            Assert.Throws<CurrencyMisMatchException>(() => fistAmount - secondAmount);
        }
    }

    public class FakeCurrencyLookup : ICurrencyLookup
    {
        private static readonly IEnumerable<CurrencyDetails> _currencies = new[]
        {
            new CurrencyDetails
            {
                CurrencyCode = "EUR",
                DecimalPlaces = 2,
                InUse = true
            },
            new CurrencyDetails
            {
                CurrencyCode = "USD",
                DecimalPlaces = 2,
                InUse = true
            },
            new CurrencyDetails
            {
                CurrencyCode = "JPY",
                DecimalPlaces = 0,
                InUse = true
            },
            new CurrencyDetails
            {
                CurrencyCode = "DEM",
                DecimalPlaces = 2,
                InUse = false
            }
        };

        public CurrencyDetails FindCurrency(string currencyCode)
        {
            var currency = _currencies.FirstOrDefault(x => x.CurrencyCode == currencyCode);

            return currency ?? CurrencyDetails.None;
        }
    }
}
