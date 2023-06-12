using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
// using Nethereum.JsonRpc.UnityClient;
using Nethereum.JsonRpc.Client;
using UnityEngine.Networking;

public class QueryNFT
{
    public Web3 web3 = new Nethereum.Web3.Web3(BlockchainConstants.INFURA_GATEWAY);

    // Function Message for tokenURI
    [Function("tokenURI", "string")]
    public class TokenURIFunctionMessage : FunctionMessage
    {
        [Parameter("uint256","tokenId")]
        public BigInteger tokenId { get; set; }
    }

    // Function to get tokenURI
    public async Task<string> GetTokenURI(BigInteger tokenId, string contractAddress)
    {
        var tokenURIMessage = new TokenURIFunctionMessage() { tokenId = tokenId };
        var queryHandler = web3.Eth.GetContractQueryHandler<TokenURIFunctionMessage>();
        var tokenURI = await queryHandler
            .QueryAsync<string>(contractAddress, tokenURIMessage)
            .ConfigureAwait(false);

        return tokenURI;
    }
}