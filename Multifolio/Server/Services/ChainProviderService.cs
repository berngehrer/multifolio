using Multifolio.Server.Clients;
using Multifolio.Shared;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Services
{
    public class ChainProviderService
    {
        readonly MultifolioDbContext _dbContext;

        public ChainProviderService(MultifolioDbContext dbContext) => _dbContext = dbContext;

        public Task<IChainClient> GetClientAsync(ChainType type, CancellationToken cancellationToken)
        {
            var settings = _dbContext.Settings.FirstOrDefault();

            var chain = _dbContext.Chains.FirstOrDefault(x => x.Type == type);
            var host = chain?.Host;

            IChainClient client = null;
            switch (type)
            {
                case ChainType.Bitcoin:
                    client = new BitcoinChainClient(host);
                    break;
                case ChainType.Avalanche:
                    client = new AvaxChainClient(host);
                    break;
                case ChainType.Ethereum:
                    client = new EthChainClient(host);
                    break;
                case ChainType.Solana:
                    client = new SolChainClient(Solnet.Rpc.Cluster.MainNet);
                    break;
                case ChainType.Stellar:
                    client = new StellarChainClient(host);
                    break;
                case ChainType.Algorand:
                    client = new AlgorandChainClient(host, settings);
                    break;
                case ChainType.Coinbase:
                    client = new CoinbaseChainClient(host, settings);
                    break;
                case ChainType.XRP:
                    client = new XRPChainClient(host);
                    break;
                case ChainType.Cardano:
                    client = new CardanoChainClient(host);
                    break;
                default:
                    return null;
            }
            return Task.FromResult(client);
        }
    }
}
