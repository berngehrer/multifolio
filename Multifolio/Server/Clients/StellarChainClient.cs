using Multifolio.Shared;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public class StellarChainClient : IChainClient
    {
        readonly string _host;
        //readonly stellar_dotnet_sdk.Server _server;

        public ChainType ChainType => ChainType.Stellar;

        public StellarChainClient(string host) => _host = host; // => _server = new stellar_dotnet_sdk.Server(host);

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/accounts/{address}"));
            var result = await client.SendAsync(message, cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync(cancellationToken);
                dynamic value = JsonConvert.DeserializeObject(json);
                var balance = value.balances[0].balance;
                string balanceString = balance.ToString();
#if DEBUG
                return decimal.Parse(balanceString.Replace(".", ","));
#else
                return decimal.Parse(balance.ToString());
#endif
            }
            return (decimal)0;


            //stellar_dotnet_sdk.KeyPair keypair = stellar_dotnet_sdk.KeyPair.FromAccountId(address);

            //stellar_dotnet_sdk.responses.AccountResponse accountResponse = await _server.Accounts.Account(keypair.AccountId);

            //stellar_dotnet_sdk.responses.Balance[] balances = accountResponse.Balances;

            ////return decimal.Parse(balances.FirstOrDefault()?.BalanceString?.Replace(".", ",") ?? "0");
            //return decimal.Parse(balances.FirstOrDefault()?.BalanceString ?? "0");

            //return Task.FromResult((decimal)416.8777687);
        }


        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
