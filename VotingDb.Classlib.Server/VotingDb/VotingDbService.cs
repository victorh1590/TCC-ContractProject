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
using VotingDb.Classlib.VotingDb.ContractDefinition;

namespace VotingDb.Classlib.VotingDb
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

        public Task<bool> FindSectionQueryAsync(FindSectionFunction findSectionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FindSectionFunction, bool>(findSectionFunction, blockParameter);
        }

        
        public Task<bool> FindSectionQueryAsync(uint sectionID, BlockParameter blockParameter = null)
        {
            var findSectionFunction = new FindSectionFunction();
                findSectionFunction.SectionID = sectionID;
            
            return ContractHandler.QueryAsync<FindSectionFunction, bool>(findSectionFunction, blockParameter);
        }

        public Task<GetAllVotesOutputDTO> GetAllVotesQueryAsync(GetAllVotesFunction getAllVotesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllVotesFunction, GetAllVotesOutputDTO>(getAllVotesFunction, blockParameter);
        }

        public Task<GetAllVotesOutputDTO> GetAllVotesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllVotesFunction, GetAllVotesOutputDTO>(null, blockParameter);
        }

        public Task<uint> GetTotalVotesByCandidateQueryAsync(GetTotalVotesByCandidateFunction getTotalVotesByCandidateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalVotesByCandidateFunction, uint>(getTotalVotesByCandidateFunction, blockParameter);
        }

        
        public Task<uint> GetTotalVotesByCandidateQueryAsync(string candidate, BlockParameter blockParameter = null)
        {
            var getTotalVotesByCandidateFunction = new GetTotalVotesByCandidateFunction();
                getTotalVotesByCandidateFunction.Candidate = candidate;
            
            return ContractHandler.QueryAsync<GetTotalVotesByCandidateFunction, uint>(getTotalVotesByCandidateFunction, blockParameter);
        }

        public Task<uint> GetTotalVotesBySectionQueryAsync(GetTotalVotesBySectionFunction getTotalVotesBySectionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalVotesBySectionFunction, uint>(getTotalVotesBySectionFunction, blockParameter);
        }

        
        public Task<uint> GetTotalVotesBySectionQueryAsync(uint sectionID, BlockParameter blockParameter = null)
        {
            var getTotalVotesBySectionFunction = new GetTotalVotesBySectionFunction();
                getTotalVotesBySectionFunction.SectionID = sectionID;
            
            return ContractHandler.QueryAsync<GetTotalVotesBySectionFunction, uint>(getTotalVotesBySectionFunction, blockParameter);
        }

        public Task<uint> GetTotalVotesInBlockQueryAsync(GetTotalVotesInBlockFunction getTotalVotesInBlockFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalVotesInBlockFunction, uint>(getTotalVotesInBlockFunction, blockParameter);
        }

        
        public Task<uint> GetTotalVotesInBlockQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetTotalVotesInBlockFunction, uint>(null, blockParameter);
        }

        public Task<GetVotesByCandidateOutputDTO> GetVotesByCandidateQueryAsync(GetVotesByCandidateFunction getVotesByCandidateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesByCandidateFunction, GetVotesByCandidateOutputDTO>(getVotesByCandidateFunction, blockParameter);
        }

        public Task<GetVotesByCandidateOutputDTO> GetVotesByCandidateQueryAsync(string candidate, BlockParameter blockParameter = null)
        {
            var getVotesByCandidateFunction = new GetVotesByCandidateFunction();
                getVotesByCandidateFunction.Candidate = candidate;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesByCandidateFunction, GetVotesByCandidateOutputDTO>(getVotesByCandidateFunction, blockParameter);
        }

        public Task<GetVotesBySectionOutputDTO> GetVotesBySectionQueryAsync(GetVotesBySectionFunction getVotesBySectionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesBySectionFunction, GetVotesBySectionOutputDTO>(getVotesBySectionFunction, blockParameter);
        }

        public Task<GetVotesBySectionOutputDTO> GetVotesBySectionQueryAsync(uint sectionID, BlockParameter blockParameter = null)
        {
            var getVotesBySectionFunction = new GetVotesBySectionFunction();
                getVotesBySectionFunction.SectionID = sectionID;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesBySectionFunction, GetVotesBySectionOutputDTO>(getVotesBySectionFunction, blockParameter);
        }

        public Task<uint> GetVotesOnSectionByCandidateQueryAsync(GetVotesOnSectionByCandidateFunction getVotesOnSectionByCandidateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVotesOnSectionByCandidateFunction, uint>(getVotesOnSectionByCandidateFunction, blockParameter);
        }

        
        public Task<uint> GetVotesOnSectionByCandidateQueryAsync(uint sectionID, string candidate, BlockParameter blockParameter = null)
        {
            var getVotesOnSectionByCandidateFunction = new GetVotesOnSectionByCandidateFunction();
                getVotesOnSectionByCandidateFunction.SectionID = sectionID;
                getVotesOnSectionByCandidateFunction.Candidate = candidate;
            
            return ContractHandler.QueryAsync<GetVotesOnSectionByCandidateFunction, uint>(getVotesOnSectionByCandidateFunction, blockParameter);
        }
    }
}
