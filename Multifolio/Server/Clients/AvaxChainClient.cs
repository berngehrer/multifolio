
using Multifolio.Shared;

namespace Multifolio.Server.Clients
{
    public class AvaxChainClient : EthChainClient
    {
        public new ChainType ChainType => ChainType.Avalanche;

        public AvaxChainClient(string host) : base(host) { }
    }
}
