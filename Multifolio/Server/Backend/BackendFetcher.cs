using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Multifolio.Server.Services;
using Multifolio.Shared;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Backend
{
    public class BackendFetcher : BackgroundService
    {
        readonly IServiceScopeFactory _scopeFactory;
        DateTime _lastNotification = DateTime.MinValue;
        
        public BackendFetcher(IServiceScopeFactory scopeFactory) 
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<MultifolioDbContext>();

                var settings = dbContext.Settings.FirstOrDefault();
                if (settings != null)
                {
                    try
                    {
                        var balanceService = scope.ServiceProvider.GetService<BalanceService>();
                        var result = await balanceService.GetCoinmarketcapData(dbContext, stoppingToken);
                        var walletSum = await balanceService.UpdateDatabaseAsync(result, dbContext, stoppingToken);
                        await CheckNotifiationTrigger(walletSum, result, settings, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        bool success = await SendTelegramAsync(ex.Message + "\n" + ex.ToString(), settings, stoppingToken);
                        if (!success)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(settings.CoinmarketcapMinuteInterval), stoppingToken);
                }
                else
                {
                    break;
                }
            }
        }

        async Task CheckNotifiationTrigger(double sum, Currency[] result, BackendSettings settings, CancellationToken stoppingToken)
        {
            if (DateTime.Now.Subtract(_lastNotification) > TimeSpan.FromMinutes(settings.NotificationSleepInterval))
            {
                var winners = result.Where(x => Math.Abs(x.PercentChange1h) >= settings.NotificationPercentage);
                foreach (var asset in winners)
                {
                    var message = $"{asset.Name} ({asset.Symbol}): {asset.Price:n2} {asset.ConvertCurrency} ({asset.PercentChange1h:n2}%)";
                    await SendTelegramAsync(message, settings, stoppingToken);
                }
                if (winners.Any() || sum < 2000) // TODO: Put value to settings
                {
                    var message = $"Wallet: {sum:n2} EUR";
                    await SendTelegramAsync(message, settings, stoppingToken);
                }
                _lastNotification = DateTime.Now;
            }
        }

        async Task<bool> SendTelegramAsync(string message, BackendSettings settings, CancellationToken token)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var httpClientFactory = scope.ServiceProvider.GetService<IHttpClientFactory>();

                var text = WebUtility.HtmlEncode(message);
                var url = $"https://api.telegram.org/{settings.TelegramBotKey}/sendMessage?chat_id={settings.TelegramChatId}&text={text}";

                using var client = httpClientFactory.CreateClient();
                var res = await client.GetAsync(url, token);
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
