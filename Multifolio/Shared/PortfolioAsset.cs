
using System;

namespace Multifolio.Shared
{
    public class PortfolioAsset
    {
        public PortfolioAsset()
        {
        }

        public PortfolioAsset(MarketHistory history, double oldPrice)
        {
            Name = history.Name;
            Symbol = history.Symbol;
            PublicKey = history.PublicKey;
            ContractKey = history.ContractKey;
            Balance = history.Balance;
            Price = history.Price;
            Amount = history.Amount;
            Currency = history.Currency;
            //Change = history.Change1h;
            if (oldPrice == 0d)
                Change = 0d;
            else
                Change = (Price - oldPrice) * 100 / oldPrice;
        }

        public string Name { get; set; }
        public string Symbol { get; set; }
        public string PublicKey { get; set; }
        public string ContractKey { get; set; }
        public double Balance { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double Percent { get; set; }
        public string Currency { get => "€"; set { } }
        public double Change { get; set; }
        public string Icon { get; set; }

        public PortfolioBalance[] PriceHistory { get; set; }

        public double ChangeAmount
        {
            get {
                var diff = Amount - (Amount * 100d / (100d + Math.Abs(Change)));
                return diff >= 0 ? diff : diff * -1d;
            }
        }
    }
}
