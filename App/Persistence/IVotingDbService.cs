using App.Persistence.ContractDefinition;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;

namespace App.Persistence;

public interface IVotingDbService
{
    ContractHandler ContractHandler { get; }
    Task<string> GetCompressedDataQueryAsync(GetCompressedDataFunction getCompressedDataFunction, BlockParameter? blockParameter = null);
    Task<string> GetCompressedDataQueryAsync(BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(GetOwnerFunction getOwnerFunction, BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(GetTimestampFunction getTimestampFunction, BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(BlockParameter? blockParameter = null);
}