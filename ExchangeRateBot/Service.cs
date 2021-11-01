using System;

namespace ExchangeRateBot
{
    public static class Service
    {
        public static (DateTime, string) ParseInputMessage(string input)
        {
            DateTime date;
            string currency;

            var splitedInput = input.Split(' ');
            if (splitedInput.Length != 2) 
            { 
                throw new Exception(@"Request formatting error. The request must be in the format ""dd.MM.yyyy currency code"". For example: ""11.02.2020 USD"""); 
            }

            string dateString = splitedInput[0];

            if (!DateTime.TryParse(dateString, out date))
            {
                throw new Exception("Incorrect date or date formatting. Specify the date in the format dd.MM.yyyy. For example : 01.11.2020\n");
            }

            currency = splitedInput[1].ToUpper();

            return (date, currency);
        }
        public static string CreateOutputMessage(ExchangeRate exchange, DateTime date)
        {
            string message = $"Date: *{date.ToString("d")}*\n";

            if (exchange.baseCurrency != null)
            {
                message += $"Base currency: *{exchange.baseCurrency}*\n";
            }
            if (exchange.currency != null)
            {
                message += $"Chosen currency: *{exchange.currency}*\n";
            }
            if (exchange.saleRateNB != 0)
            {
                message+= $"National Bank sale rate: *{exchange.saleRateNB}*\n";
            }
            if (exchange.purchaseRateNB != 0)
            {
                message += $"National Bank purchase rate: *{exchange.purchaseRateNB}*\n";
            }
            if (exchange.saleRate != 0)
            {
                message += $"Privat Bank sale rate: *{exchange.saleRate}*\n";
            }
            if (exchange.purchaseRate != 0)
            {
                message += $"Privat Bank purchase rate: *{exchange.purchaseRate}*\n";
            }

            return message;
        }
    }
}
