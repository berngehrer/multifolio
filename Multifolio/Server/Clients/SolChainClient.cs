using Multifolio.Shared;
using Solnet.Extensions;
using Solnet.Rpc;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public class SolChainClient : IChainClient
    {
        readonly Cluster cluster;

        public ChainType ChainType => ChainType.Solana;

        public SolChainClient(Cluster cluster)
        {
            this.cluster = cluster;
        }

        public Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            var client = ClientFactory.GetClient(cluster);

            ITokenMintResolver tokens = TokenMintResolver.Load(); 

            TokenWallet tokenWallet = TokenWallet.Load(client, tokens, address);

            var solBalance = tokenWallet.Sol;
            return Task.FromResult(solBalance);
        }

        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            var client = ClientFactory.GetClient(Cluster.MainNet);

            ITokenMintResolver tokens = TokenMintResolver.Load();

            TokenWallet tokenWallet = TokenWallet.Load(client, tokens, address);

            var token = tokenWallet.TokenAccounts().WithPublicKey(contractAddress);

            return Task.FromResult(token.QuantityDecimal);


            //foreach (var token in tokenWallet.Balances())
            //{
            //    var dec = token.QuantityDecimal;
            //}
        }
    }
}
