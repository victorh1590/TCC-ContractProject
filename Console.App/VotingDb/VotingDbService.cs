using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Console.VotingDb.ContractDefinition;

namespace Console.VotingDb
{
    public partial class VotingDbService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, VotingDbDeployment votingDbDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>().SendRequestAndWaitForReceiptAsync(votingDbDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, VotingDbDeployment votingDbDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>().SendRequestAsync(votingDbDeployment);
        }

        public static async Task<VotingDbService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, VotingDbDeployment votingDbDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, votingDbDeployment, cancellationTokenSource);
            return new VotingDbService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public VotingDbService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public VotingDbService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> GetCompressedDataQueryAsync(GetCompressedDataFunction getCompressedDataFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCompressedDataFunction, string>(getCompressedDataFunction, blockParameter);
        }

        
        public Task<string> GetCompressedDataQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCompressedDataFunction, string>(null, blockParameter);
        }

        public Task<string> GetOwnerQueryAsync(GetOwnerFunction getOwnerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetOwnerFunction, string>(getOwnerFunction, blockParameter);
        }

        
        public Task<string> GetOwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetOwnerFunction, string>(null, blockParameter);
        }

        public Task<string> GetTimestampQueryAsync(GetTimestampFunction getTimestampFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTimestampFunction, string>(getTimestampFunction, blockParameter);
        }

        
        public Task<string> GetTimestampQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTimestampFunction, string>(null, blockParameter);
        }
    }
}
