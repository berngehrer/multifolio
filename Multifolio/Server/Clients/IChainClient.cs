using Multifolio.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public interface IChainClient
    {
        // bool SupportToken

        ChainType ChainType { get; }

        Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken);

        Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken);
    }
}
