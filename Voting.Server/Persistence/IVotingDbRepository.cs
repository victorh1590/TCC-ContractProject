using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public interface IVotingDbRepository
{
    IWeb3 Web3 { get; }
    // private Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(
    //     VotingDbDeployment votingDbDeployment, IWeb3? web3 = null,
    //     CancellationTokenSource? cancellationTokenSource = null);
    //
    // private Task<string> DeployContractAsync(VotingDbDeployment votingDbDeployment, IWeb3? web3 = null);
    Task<TransactionReceipt> CreateSectionRange(VotingDbDeployment deployment);
    Task<string> GetCompressedDataQueryAsync(string contractAddress, GetCompressedDataFunction getCompressedDataFunction, BlockParameter? blockParameter = null);
    Task<string> GetCompressedDataQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(string contractAddress, GetOwnerFunction getOwnerFunction, BlockParameter? blockParameter = null);
    Task<string> GetOwnerQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(string contractAddress, GetTimestampFunction getTimestampFunction, BlockParameter? blockParameter = null);
    Task<string> GetTimestampQueryAsync(string contractAddress, BlockParameter? blockParameter = null);
    Task<SectionEventDTO?> ReadSectionAsync(uint sectionNumber = 0);
    Task<CandidateEventDTO?> ReadVotesByCandidateAndSectionAsync(uint candidate = 0, uint sectionNumber = 0);
    Task<MetadataEventDTO?> ReadMetadataAsync(string contractAddress);
}