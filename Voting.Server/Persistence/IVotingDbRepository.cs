using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public interface IVotingDbRepository
{
    // ContractHandler ContractHandler { get; }
    Task<string> GetCompressedDataQueryAsync(string contractAddress, GetCompressedDataFunction getCompressedDataFunction, BlockParameter? blockParameter = null);
    Task<string> GetCompressedDataQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(string contractAddress, GetOwnerFunction getOwnerFunction, BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(string contractAddress, GetTimestampFunction getTimestampFunction, BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<Section> GetSectionAsync(IWeb3 web3, uint sectionNumber = 0);
    Task<List<Section>> GetSectionRangeAsync(IWeb3 web3, uint[] sectionNumbers);
    Task<Section> GetVotesByCandidateForSection(IWeb3 web3, uint candidate = 0, uint sectionNumber = 0);
}