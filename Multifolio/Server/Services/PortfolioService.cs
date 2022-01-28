using Microsoft.EntityFrameworkCore;
using Multifolio.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Services
{
    public class PortfolioService
    {
        const double MaxBalanceDataPointCount = 80;    // TODO: Settings?

        readonly MultifolioDbContext _dbContext;

        public PortfolioService(MultifolioDbContext dbContext) => _dbContext = dbContext;

        public Task<Portfolio> GetPortfolio(CancellationToken cancellationToken)
        {
            return GetPortfolio(DateRange.Hour, grouped: false, includeEmpty: false, cancellationToken);
        }

        public async Task<Portfolio> GetPortfolio(DateRange range, bool grouped, bool includeEmpty, CancellationToken cancellationToken)
        {
            // TODO: settings
            const int minPortfolioAmount = 10;

            try
            {
                // ======================= Get latest entry ====================================
                var latestUpdate = _dbContext.History.Max(x => x.Updated);
                var latestSet = await _dbContext.History.Where(x => x.Updated == latestUpdate && (includeEmpty || (!includeEmpty && x.Amount > minPortfolioAmount))).ToListAsync(cancellationToken);
                var setSum = latestSet.Sum(x => x.Amount);

                // ======================= Get history within range ====================================
                var rangeHistory = _dbContext.History.Where(x => x.Updated > GetRangeDatetime(range)).GroupBy(x => x.Updated);
                if (!rangeHistory.Any())
                {
                    return new Portfolio
                    {
                        Range = range,
                        Amount = setSum,
                        Updated = latestUpdate,
                        Assets = Array.Empty<PortfolioAsset>(),
                        Balances = Array.Empty<PortfolioBalance>()
                    };
                }

                // ======================= Get first entry within history ====================================
                var firstUpdate = rangeHistory.Min(x => x.Key);
                var firstSet = await _dbContext.History.Where(x => x.Updated == firstUpdate && (includeEmpty || (!includeEmpty && x.Amount > minPortfolioAmount))).ToListAsync(cancellationToken);
                double getOldPrice(string symbol)
                {
                    var results = firstSet.Where(y => y.Symbol == symbol);
                    if (results.Any())
                        return results.First().Price;
                    else return 0d;
                }

                // ======================= Calculate set differences within history ====================================
                var oldSum = firstSet.Sum(x => x.Amount);
                var amountChange = setSum - oldSum;
                var percentChange = amountChange * 100 / oldSum;

                // ======================= Create portfolio assets within history ====================================
                var assets = latestSet.Distinct().Select(x => new PortfolioAsset(x, getOldPrice(x.Symbol))
                {
                    Percent = Math.Round(x.Amount * 100 / setSum, 1),
                    Icon = GetIcon(x.Symbol)
                })
                        .OrderByDescending(x => x.Amount)
                        .ToArray();

                // ======================= Group portfolio assets within history by on biggest one
                if (grouped)
                {
                    var groupedList = new List<PortfolioAsset>();
                    var groupdAssets = assets.GroupBy(x => x.Symbol);
                    foreach (var group in groupdAssets)
                    {
                        var biggest = group.OrderBy(x => x.Amount).Last();
                        foreach (var sub in group.Except(new[] { biggest }))
                        {
                            biggest.Amount += sub.Amount;
                            biggest.Balance += sub.Balance;
                            biggest.Percent += sub.Percent;
                        }
                        groupedList.Add(biggest);
                    }
                    assets = groupedList.ToArray();
                }

                // ======================= Get asset prices within history depending on grouping
                var assetRangeHistory = await _dbContext.History.Where(x => x.Updated > GetRangeDatetime(range)).ToListAsync(cancellationToken);
                foreach (var asset in assets)
                {
                    var assetHistory = assetRangeHistory.Where(x => x.Symbol == asset.Symbol);
                    var filteredHistory = (grouped)
                                         ? assetHistory
                                         : assetHistory.Where(x => x.PublicKey == asset.PublicKey);

                    double assetHistorySum(DateTime dt)
                    {
                        var value = assetRangeHistory.Where(x => x.Updated == dt).Sum(x => x.Amount);
                        return value == 0 ? 1 : value;
                    }

                    var workingList = (filteredHistory.GroupBy(x => x.Updated).Select(h => new PortfolioBalance
                    {
                        Date = h.Key,
                        Amount = h.FirstOrDefault().Price,
                        Percent = Math.Round(h.FirstOrDefault().Amount * 100 / assetHistorySum(h.FirstOrDefault().Updated), 1)
                    }))
                    .Reverse().ToArray();

                    var filteredAssetBalances = new List<PortfolioBalance>();
                    var assetReduceFactor = (int)Math.Floor(workingList.Length / MaxBalanceDataPointCount);
                    for (int i = 0; i < workingList.Length; i++)
                    {
                        if (assetReduceFactor > 0 && i % assetReduceFactor == 0)
                        {
                            filteredAssetBalances.Add(workingList[i]);
                        }
                        if (assetReduceFactor == 0)
                        {
                            filteredAssetBalances.Add(workingList[i]);
                        }
                    }
                    filteredAssetBalances.Reverse();
                    asset.PriceHistory = filteredAssetBalances.ToArray();
                }

                // ======================= Create portfolio blances within history ====================================
                var balances = (await rangeHistory.Select(history => new PortfolioBalance
                {
                    Date = history.Key,
                    Amount = history.Distinct().Sum(x => x.Amount)
                })
                    .ToListAsync(cancellationToken))
                    .ToArray();

                // ======================= Filter portfolio blances within history to max data points
                // To always have the latest values included
                balances.Reverse();
                var filteredBalances = new List<PortfolioBalance>();
                var reduceFactor = (int)Math.Floor(balances.Length / MaxBalanceDataPointCount);
                for (int i = 0; i < balances.Length; i++)
                {
                    if (reduceFactor > 0 && i % reduceFactor == 0)
                    {
                        filteredBalances.Add(balances[i]);
                    }
                    if (reduceFactor == 0)
                    {
                        filteredBalances.Add(balances[i]);
                    }
                }

                filteredBalances.Reverse();
                return new Portfolio
                {
                    Range = range,
                    Amount = setSum,
                    Assets = assets,                    
                    Updated = latestUpdate,
                    AmountChange = amountChange,
                    PercentChange = percentChange,
                    Balances = filteredBalances.ToArray()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Portfolio
                {
                    Assets = Array.Empty<PortfolioAsset>(),
                    Balances = Array.Empty<PortfolioBalance>()
                };
            }
        }

        DateTime GetRangeDatetime(DateRange range)
        {
            return range switch
            { 
                DateRange.Hour => DateTime.Now.Subtract(TimeSpan.FromHours(1)),
                DateRange.Day => DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                DateRange.Week => DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                DateRange.Month => DateTime.Now.Subtract(TimeSpan.FromDays(30)),
                DateRange.Year => DateTime.Now.Subtract(TimeSpan.FromDays(365)),
                DateRange.All => DateTime.MinValue,
                _ => DateTime.Now.Subtract(TimeSpan.FromHours(1))
            };
        }

        string GetIcon(string symbol)
        {
            Chain c = _dbContext.Chains.FirstOrDefault(x => x.Symbol == symbol);
            if (c != null)
                return c.Icon;
            AccountToken t = _dbContext.AccountTokens.FirstOrDefault(x => x.Symbol == symbol);
            return t?.Icon;
        }
    }
}
