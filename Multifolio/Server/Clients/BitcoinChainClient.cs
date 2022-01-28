using Multifolio.Shared;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    /// <summary>
    /// https://www.blockcypher.com/dev/bitcoin/#event
    /// 
    /// https://api.blockcypher.com/v1/btc/main/addrs/1DEP8i3QJCsomS4BSMY2RpU1upv62aGvhD/balance
    /// </summary>
    public class BitcoinChainClient : IChainClient
    {
        readonly string _host;

        public ChainType ChainType => ChainType.Bitcoin;

        public BitcoinChainClient(string host) => _host = host; 

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            const int digits = 8;

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1/btc/main/addrs/{address}"));
            var result = await client.SendAsync(message, cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync(cancellationToken);
                dynamic json = JsonConvert.DeserializeObject(content);
                string value = (json.balance).ToString();

                var balance = double.Parse(value) / Math.Pow(10, digits);
                return (decimal)balance;
            }
            return (decimal)0;
        }

        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
