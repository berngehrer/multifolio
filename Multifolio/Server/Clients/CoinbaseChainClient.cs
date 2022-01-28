using Coinbase;
using Multifolio.Shared;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public class CoinbaseChainClient : IChainClient
    {
        readonly string _host;
        //readonly stellar_dotnet_sdk.Server _server;

        public ChainType ChainType => ChainType.Coinbase;

        public CoinbaseChainClient(string host, BackendSettings settings)
        {
            _host = host;
        }

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            // TODO: Get key and secret from settings !
            var client = new CoinbaseClient(new ApiKeyConfig { ApiKey = "", ApiSecret = "" });
            var account = await client.Accounts.GetAccountAsync(address, cancellationToken);
            return account.Data.Balance.Amount;
        }


        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
