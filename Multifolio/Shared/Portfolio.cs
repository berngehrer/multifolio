using System;
using System.Linq;

namespace Multifolio.Shared
{
    public class Portfolio
    {
        public double Amount { get; set; }

        public double PercentChange { get; set; }

        public double AmountChange { get; set; }

        public DateRange Range { get; set; }

        public DateTime Updated { get; set; }

        public PortfolioBalance[] Balances { get; set; }

        public PortfolioAsset[] Assets { get; set; }

        public string Currency => "€"; // Assets?.Select(x => x.Currency).First();
    }
}
