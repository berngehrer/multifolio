using System;

namespace Multifolio.Shared
{
    public class MarketHistory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string PublicKey { get; set; }
        public string ContractKey { get; set; }
        public double Balance { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double Change1h { get; set; }
        public double Change24h { get; set; }
        public double Change7d { get; set; }
        public DateTime Updated { get; set; }
    }
}
