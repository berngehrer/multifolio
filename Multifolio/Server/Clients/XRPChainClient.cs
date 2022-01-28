using Multifolio.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public class XRPChainClient : IChainClient
    {
        //readonly string Data = "{ \"command\": \"account_info\", \"account\": \"{0}\", \"strict\": true, \"ledger_index\": \"validated\", \"api_version\": 1 }";

        readonly string _host;

        public ChainType ChainType => ChainType.XRP;

        public XRPChainClient(string host) => _host = host; 

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken stoppingToken)
        {
            const int digits = 6;

            using (var socket = new ClientWebSocket())
                try
                {
                    await socket.ConnectAsync(new Uri(_host), stoppingToken);

                    var data = "{ \"command\": \"account_info\", \"account\": \"" + address + "\", \"strict\": true, \"ledger_index\": \"validated\", \"api_version\": 1 }";

                    await Send(socket, data, stoppingToken);
                    var result = await Receive(socket, stoppingToken);

                    dynamic json = JsonConvert.DeserializeObject(result);
                    double value = json.result.account_data.Balance;

                    var balance = value / Math.Pow(10, digits);
                    return (decimal)balance;
                }
                catch (Exception)
                {
                    return (decimal)0;
                }
                finally
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", stoppingToken);
                }
        }


        public Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }


        async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        async Task<string> Receive(ClientWebSocket socket, CancellationToken stoppingToken)
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            while (!stoppingToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, stoppingToken);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        return await reader.ReadToEndAsync();
                }
            };
            return null;
        }
    }
}
