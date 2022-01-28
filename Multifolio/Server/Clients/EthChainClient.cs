using Multifolio.Shared;
using Nethereum.Web3;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Multifolio.Server.Clients
{
    public class EthChainClient : IChainClient
    {
        readonly Web3 _web3;

        public EthChainClient(string host) => _web3 = new Web3(host);

        public ChainType ChainType => ChainType.Ethereum;

        public async Task<decimal> GetAccountBalanceAsync(string address, CancellationToken cancellationToken)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance);
        }


        public async Task<decimal> GetTokenBalanceAsync(string address, string contractAddress, CancellationToken cancellationToken)
        {
            string abiStandardToken = @"[{""constant"":false,""inputs"":[{""name"":""_spender"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":""supply"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_from"",""type"":""address""},{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":""success"",""type"":""bool""}],""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""},{""name"":""_spender"",""type"":""address""}],""name"":""allowance"",""outputs"":[{""name"":""remaining"",""type"":""uint256""}],""type"":""function""},{""inputs"":[{""name"":""_initialAmount"",""type"":""uint256""}],""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_from"",""type"":""address""},{""indexed"":true,""name"":""_to"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_owner"",""type"":""address""},{""indexed"":true,""name"":""_spender"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""}]";
            var contract = _web3.Eth.GetContract(abiStandardToken, contractAddress);

            //var contract = _web3.Eth.GetContract(Properties.Resources.EthERC20Abi, contractAddress);
            var balanceFunction = contract.GetFunction("balanceOf");
            var balance = await balanceFunction.CallAsync<BigInteger>(address);
            return Web3.Convert.FromWei(balance);
        }
    }
}
