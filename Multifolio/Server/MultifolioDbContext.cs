using Microsoft.EntityFrameworkCore;
using Multifolio.Shared;
using System.Diagnostics.CodeAnalysis;

namespace Multifolio.Server
{
    public class MultifolioDbContext : DbContext
    {
        public MultifolioDbContext()
        {
        }

        public MultifolioDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }


        public DbSet<Chain> Chains { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountToken> AccountTokens { get; set; }

        public DbSet<BackendSettings> Settings { get; set; }

        public DbSet<MarketHistory> History { get; set; }
    }
}
