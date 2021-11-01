using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ExchangeRateBot
{
    public class Program
    {
        private static Config json;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();
            json = config.Get<Config>();

            Console.WriteLine($"Bot name: {json.BotName}");

            var botClient = new TelegramBotClient(json.Token);
            using var cts = new CancellationTokenSource();
            botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cts.Token);

            Console.ReadLine();

            cts.Cancel();
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            DateTime startTime = DateTime.Now;

            if (update.Message.Date > startTime) return;
            if (update.Type != UpdateType.Message) return;
            if (update.Message.Type != MessageType.Text) return;
            if (update.Message.Text == "/start") { return; }

            var chatId = update.Message.Chat.Id;
            Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

            string message = null;
            DateTime date = default;
            string currency = null;

            try
            {
                var parsedMessage = Service.ParseInputMessage(update.Message.Text);
                date = parsedMessage.Item1;
                currency = parsedMessage.Item2;
                Validation.ValidateDate(date);
                Validation.ValidateCurrency(currency);
                var exRate = await GetExchangeRate(currency, date);
                message = Service.CreateOutputMessage(exRate, date);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Markdown,
                replyToMessageId: update.Message.MessageId
            );
        }

        static async Task<ExchangeRate> GetExchangeRate(string curr, DateTime date)
        {
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                string connectionPath = json.PrivatBankAPI;
                string path = connectionPath + date;
                response = await client.GetAsync(path);
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var allData = JsonSerializer.Deserialize<AllData>(responseBody);
            return allData.exchangeRate.FirstOrDefault(exchangeRate => exchangeRate.currency == curr);
        }
    }
}
