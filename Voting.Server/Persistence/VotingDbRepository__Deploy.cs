using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Domain.Utils;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository
{
    private Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(
        VotingDbDeployment votingDbDeployment, IWeb3? web3 = null, CancellationTokenSource? cancellationTokenSource = null)
    {
        web3 ??= Web3;
        return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>()
            .SendRequestAndWaitForReceiptAsync(votingDbDeployment, cancellationTokenSource);
    }

    private Task<string> DeployContractAsync(VotingDbDeployment votingDbDeployment, IWeb3? web3 = null)
    {
        web3 ??= Web3;
        return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>().SendRequestAsync(votingDbDeployment);
    }

    // public async Task<VotingDbRepository> DeployContractAndGetRepositoryAsync(
    //     VotingDbDeployment votingDbDeployment, IWeb3? web3 = null, CancellationTokenSource? cancellationTokenSource = null)
    // {
    //     web3 ??= Web3;
    //     var receipt = await DeployContractAndWaitForReceiptAsync(web3, votingDbDeployment, cancellationTokenSource);
    //     return new VotingDbRepository(Web3, receipt.ContractAddress);
    // }
    
    public async Task<string> CreateSectionRange(VotingDbDeployment deployment)
    {
        return await DeployContractAsync(deployment);
    }
}