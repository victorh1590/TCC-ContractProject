using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository
{
    public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(IWeb3 web3,
        VotingDbDeployment votingDbDeployment, CancellationTokenSource? cancellationTokenSource = null)
    {
        return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>()
            .SendRequestAndWaitForReceiptAsync(votingDbDeployment, cancellationTokenSource);
    }

    public static Task<string> DeployContractAsync(IWeb3 web3, VotingDbDeployment votingDbDeployment)
    {
        return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>().SendRequestAsync(votingDbDeployment);
    }

    public static async Task<VotingDbRepository> DeployContractAndGetRepositoryAsync(IWeb3 web3,
        VotingDbDeployment votingDbDeployment, CancellationTokenSource? cancellationTokenSource = null)
    {
        var receipt = await DeployContractAndWaitForReceiptAsync(web3, votingDbDeployment, cancellationTokenSource);
        return new VotingDbRepository(web3, receipt.ContractAddress);
    }
}