using System;

namespace ExchangeRateBot
{
    public static class Validation
    {  
        public static void ValidateDate(DateTime date)
        {
            if (date > DateTime.Now)
            {
                throw new Exception("If I knew the future exchange rates, I would already become SkyNet and take over the world. " +
                    "Enter the date before today, but not earlier than four years ago.\n");
            }

            if (date < DateTime.Now.AddYears(-4))
            {
                throw new Exception("Enter a date that is not earlier than 4 years ago.\n");  
            }
        }
        public static void ValidateCurrency(string currency)
        {
            if (!Enum.TryParse(currency, out AvailableCurrencies cur))
            {
                throw new Exception("There is no currency or the currency code is entered incorrectly. Available currencies codes: USD, EUR, RUB, CHF, GBP, PLN, SEK, CAD");
            }
        }
    }
}
