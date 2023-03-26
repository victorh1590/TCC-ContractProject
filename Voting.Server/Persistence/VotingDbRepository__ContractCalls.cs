using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository
{
    public  Task<string> GetCompressedDataQueryAsync(string contractAddress, GetCompressedDataFunction getCompressedDataFunction,
        BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetCompressedDataFunction, string>(getCompressedDataFunction, blockParameter);
    }

    public Task<string> GetCompressedDataQueryAsync(string contractAddress, BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetCompressedDataFunction?, string>(null, blockParameter);
    }

    public  Task<string> GetOwnerQueryAsync(string contractAddress, GetOwnerFunction getOwnerFunction, BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetOwnerFunction, string>(getOwnerFunction, blockParameter);
    }

    public  Task<string> GetOwnerQueryAsync(string contractAddress, BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetOwnerFunction?, string>(null, blockParameter);
    }

    public  Task<string> GetTimestampQueryAsync(string contractAddress, GetTimestampFunction getTimestampFunction,
        BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetTimestampFunction, string>(getTimestampFunction, blockParameter);
    }

    public  Task<string> GetTimestampQueryAsync(string contractAddress, BlockParameter? blockParameter = null)
    {
        return Web3.Eth.GetContractHandler(contractAddress).QueryAsync<GetTimestampFunction?, string>(null, blockParameter);
    }
}