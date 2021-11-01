using System.Collections.Generic;

namespace ExchangeRateBot
{
    enum AvailableCurrencies
    {
        USD, EUR, RUB, CHF, GBP, PLN, SEK, CAD
    }
    public class ExchangeRate
    {
        public string baseCurrency { get; set; }
        public string currency { get; set; }
        public double saleRateNB { get; set; }
        public double purchaseRateNB { get; set; }
        public double saleRate { get; set; }
        public double purchaseRate { get; set; }
    }
    public class AllData
    {
        public string date { get; set; }
        public string bank { get; set; }
        public long baseCurrency { get; set; }
        public string baseCurrencyLit { get; set; }
        public List<ExchangeRate> exchangeRate { get; set; }
    }
}
