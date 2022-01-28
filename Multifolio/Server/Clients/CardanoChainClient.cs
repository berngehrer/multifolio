using Multifolio.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    /// <summary>
    /// https://developers.cardano.org/docs/get-started/dandelion-apis/
    /// 
    /// https://input-output-hk.github.io/cardano-rest/explorer-api/
    /// 
    /// https://nownodes.medium.com/how-to-connect-to-cardano-block-explorer-9440e8dc8879
    /// 
    /// https://documenter.getpostman.com/view/13630829/TVmFkLwy#c0836be6-3f07-45e9-974f-33a66fe0a150
    /// https://documenter.getpostman.com/view/13630829/TVmFkLwy#how-to-get-an-api-key
    /// </summary>
    public class CardanoChainClient : IChainClient
    {
        public ChainType ChainType => ChainType.Cardano;

        public CardanoChainClient(string host) { }

        public Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            return Task.FromResult((decimal)0);
        }

        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
