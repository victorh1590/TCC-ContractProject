using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository
{
    public Task<string> GetCompressedDataQueryAsync(GetCompressedDataFunction getCompressedDataFunction,
        BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetCompressedDataFunction, string>(getCompressedDataFunction, blockParameter);
    }

    public Task<string> GetCompressedDataQueryAsync(BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetCompressedDataFunction?, string>(null, blockParameter);
    }

    public Task<string> GetOwnerQueryAsync(GetOwnerFunction getOwnerFunction, BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetOwnerFunction, string>(getOwnerFunction, blockParameter);
    }

    public Task<string> GetOwnerQueryAsync(BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetOwnerFunction?, string>(null, blockParameter);
    }

    public Task<string> GetTimestampQueryAsync(GetTimestampFunction getTimestampFunction,
        BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetTimestampFunction, string>(getTimestampFunction, blockParameter);
    }

    public Task<string> GetTimestampQueryAsync(BlockParameter? blockParameter = null)
    {
        return ContractHandler.QueryAsync<GetTimestampFunction?, string>(null, blockParameter);
    }
}