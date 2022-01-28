
namespace Multifolio.Shared
{
    public class BackendSettings
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string CoinmarketcapApiKey { get; set; }
        public double CoinmarketcapMinuteInterval { get; set; }
        public string PurestakeAlgorandApiKey { get; set; }
        public string TelegramBotKey { get; set; }
        public string TelegramChatId { get; set; }
        public double NotificationPercentage { get; set; }
        public double NotificationMinBalance { get; set; }
        public double NotificationSleepInterval { get; set; }
    }
}
