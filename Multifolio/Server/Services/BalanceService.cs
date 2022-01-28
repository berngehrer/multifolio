using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Multifolio.Shared;
using NoobsMuc.Coinmarketcap.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Services
{
    public class BalanceService
    {
        readonly IServiceScopeFactory _scopeFactory;

        public BalanceService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<AccountBalance[]> GetAccountBalancesFromDBAsync(MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var balances = new List<AccountBalance>();
            await foreach (var acc in dbContext.Accounts.AsAsyncEnumerable())
                balances.Add(await GetAccountBalanceFromDBAsync(acc, dbContext, cancellationToken));
            return balances.ToArray();
        }

        public async Task<AccountBalance> GetAccountBalanceFromDBAsync(Account account, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var chain = dbContext.Chains.Find(account.ChainId);
            var latestUpdate = dbContext.History.Max(x => x.Updated);
            var data = await dbContext.History.Where(x => x.PublicKey == account.PublicKey && x.Updated == latestUpdate && x.Symbol == chain.Symbol).ToListAsync(cancellationToken);
            return new AccountBalance { AccountId = account.Id, Balance = (decimal)data.FirstOrDefault()?.Balance };
        }

        public async Task<AccountTokenBalance[]> GetTokenBalancesFromDBAsync(MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var balances = new List<AccountTokenBalance>();
            await foreach (var token in dbContext.AccountTokens.AsAsyncEnumerable())
            {
                var account = dbContext.Accounts.Find(token.AccountId);
                balances.Add(await GetAccountTokenBalanceFromDBAsync(account, token, dbContext, cancellationToken));
            }
            return balances.ToArray();
        }

        public async Task<AccountTokenBalance> GetAccountTokenBalanceFromDBAsync(Account account, AccountToken token, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var latestUpdate = dbContext.History.Max(x => x.Updated);
            var data = await dbContext.History.Where(x => x.PublicKey == account.PublicKey && x.ContractKey == token.ContractKey && x.Updated == latestUpdate && x.Symbol == token.Symbol).ToListAsync(cancellationToken);
            return new AccountTokenBalance { AccountTokenId = token.Id, Balance = (decimal)data.FirstOrDefault()?.Balance };
        }




        //==== Backend Helpers


        async Task<AccountBalance> GetAccountBalanceAsync(Account account, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var chainService = scope.ServiceProvider.GetService<ChainProviderService>();

            var chain = dbContext.Chains.Find(account.ChainId);
            var client = await chainService.GetClientAsync(chain.Type, cancellationToken);
            var balance = await client.GetAccountBalanceAsync(account.PublicKey, cancellationToken);
            return new AccountBalance { AccountId = account.Id, Balance = balance };
        }

        async Task<AccountTokenBalance> GetAccountTokenBalanceAsync(Account account, AccountToken token, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var chainService = scope.ServiceProvider.GetService<ChainProviderService>();

            var chain = dbContext.Chains.Find(account.ChainId);
            var client = await chainService.GetClientAsync(chain.Type, cancellationToken);
            var balance = await client.GetTokenBalanceAsync(account.PublicKey, token.ContractKey, cancellationToken);
            return new AccountTokenBalance { AccountTokenId = token.Id, Balance = balance };
        }

        async Task<Balance[]> GetBalancesAsync(string symbol, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var results = new List<Balance>();
            var chains = dbContext.Chains.Where(x => x.Symbol == symbol);
            if (chains.Any())
            {
                foreach (Chain chain in chains)
                {
                    var accounts = dbContext.Accounts.Where(x => x.ChainId == chain.Id);
                    foreach (var acc in accounts)
                    {
                        var balance = await GetAccountBalanceAsync(acc, dbContext, cancellationToken);
                        results.Add(new Balance { Account = acc, Value = (double)balance.Balance });
                    }
                }
            }
            else
            {
                foreach (AccountToken token in dbContext.AccountTokens.Where(x => x.Symbol == symbol))
                {
                    var account = dbContext.Accounts.Find(token.AccountId);
                    var balance = await GetAccountTokenBalanceAsync(account, token, dbContext, cancellationToken);
                    results.Add(new Balance { Account = account, Token = token, Value = (double)balance.Balance });
                }
            }
            return results.ToArray();
        }


        //==== Backend Service Functions


        public async Task<Currency[]> GetCoinmarketcapData(MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            var symbols = dbContext.Chains.Select(x => x.Symbol).Concat(dbContext.AccountTokens.Select(x => x.Symbol)).Distinct().ToArray();
            var currency = dbContext.Settings.FirstOrDefault().Currency;

            var apiKey = dbContext.Settings.FirstOrDefault()?.CoinmarketcapApiKey;
            var client = new CoinmarketcapClient(apiKey);

            return (await Task.Factory.StartNew(() => client.GetCurrencyBySymbolList(symbols, currency), cancellationToken)).ToArray();
        }

        public async Task<double> UpdateDatabaseAsync(Currency[] currencies, MultifolioDbContext dbContext, CancellationToken cancellationToken)
        {
            double sum = 0d;
            var date = currencies.Select(x => x.LastUpdated.ToLocalTime()).First();
            var hasDate = dbContext.History.Any(x => x.Updated == date);
            if (!hasDate)
            {
                foreach (var c in currencies)
                {
                    foreach (var balance in await GetBalancesAsync(c.Symbol, dbContext, cancellationToken))
                    {
                        sum += c.Price * balance.Value;
                        dbContext.Add(new MarketHistory
                        {
                            Name = c.Name,
                            Symbol = c.Symbol,
                            Price = c.Price,
                            Balance = balance.Value,
                            Amount = c.Price * balance.Value,
                            PublicKey = balance.Account.PublicKey,
                            ContractKey = balance?.Token?.ContractKey,
                            Currency = c.ConvertCurrency,
                            Change1h = c.PercentChange1h,
                            Change24h = c.PercentChange24h,
                            Change7d = c.PercentChange7d,
                            Updated = c.LastUpdated.ToLocalTime()
                        });
                    }
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return sum;
        }
    }
}
