using Multifolio.Shared;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    /// <summary>
    /// https://developer.purestake.io/home
    /// </summary>
    public class AlgorandChainClient : IChainClient
    {
        readonly string _host;
        readonly BackendSettings _settings;

        public AlgorandChainClient(string host, BackendSettings settings)
        {
            _host = host;
            _settings = settings;
        }

        public ChainType ChainType => ChainType.Algorand;

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            double digits = 6;

            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/ps2/v2/accounts/{address}"));
            message.Headers.Add("X-API-Key", _settings.PurestakeAlgorandApiKey);
            var result = await client.SendAsync(message, cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync(cancellationToken);
                dynamic value = JsonConvert.DeserializeObject(json);
                var balance = value.amount;
                return decimal.Parse(balance.ToString()) / (decimal)Math.Pow(10, digits);
            }
            return (decimal)0;

            //try
            //{
            //    AlgodApi algodApiInstance = new AlgodApi(ALGOD_API_ADDR, ALGOD_API_TOKEN);
            //    var accountInfo = algodApiInstance.AccountInformation(ADDR);
            //    return (decimal)accountInfo.Amount;

            //    var status = await algodApiInstance.GetStatusAsync();

            //    var supply = algodApiInstance.GetSupply();
            //}
            //catch (ApiException e)
            //{
            //    Console.WriteLine("Exception when calling algod#getSupply: " + e.Message);
            //}
        }


        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            //string ALGOD_API_ADDR = "your algod api address";
            //string ALGOD_API_TOKEN = "your algod api token";

            //IndexerApi indexer = new IndexerApi(ALGOD_API_ADDR, ALGOD_API_TOKEN);
            ////AlgodApi algodApiInstance = new AlgodApi(ALGOD_API_ADDR, ALGOD_API_TOKEN);
            //var health = indexer.MakeHealthCheck();
            //Console.WriteLine("Make Health Check: " + health.ToJson());

            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var address = "KV2XGKMXGYJ6PWYQA5374BYIQBL3ONRMSIARPCFCJEAMAHQEVYPB7PL3KU";
            //var acctInfo = indexer.LookupAccountByID(address);
            //Console.WriteLine("Look up account by id: " + acctInfo.ToJson());

            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var transInfos = indexer.LookupAccountTransactions(address, 10);
            //Console.WriteLine("Look up account transactions(limit 10): " + transInfos.ToJson());

            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var appsInfo = indexer.SearchForApplications(limit: 10);
            //Console.WriteLine("Search for application(limit 10): " + appsInfo.ToJson());

            //var appIndex = appsInfo.Applications[0].Id;
            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var appInfo = indexer.LookupApplicationByID(appIndex);
            //Console.WriteLine("Look up application by id: " + appInfo.ToJson());

            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var assetsInfo = indexer.SearchForAssets(limit: 10, unit: "LAT");
            //Console.WriteLine("Search for assets" + assetsInfo.ToJson());

            //var assetIndex = assetsInfo.Assets[0].Index;
            //System.Threading.Thread.Sleep(1200); //test in purestake, imit 1 req/sec
            //var assetInfo = indexer.LookupAssetByID(assetIndex);
            //Console.WriteLine("Look up asset by id:" + assetInfo.ToJson());

            throw new System.NotImplementedException();
        }
    }
}
