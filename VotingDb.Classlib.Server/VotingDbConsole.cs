using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;
using VotingDb.Classlib.VotingDb;
using VotingDb.Classlib.VotingDb.ContractDefinition;

namespace VotingDb.Classlib.Server;
public class VotingDbConsole
{
    public static async Task Main()
    {
        var url = "HTTP://localhost:7545";
        //var url = "https://mainnet.infura.io";
        var privateKey = "d1d45d0629c5a5fe5ac563c89d759ca057cb7e2936f6c78351bb93bb2f58eb99";
        var account = new Nethereum.Web3.Accounts.Account(privateKey, 5777);
        var web3 = new Web3(account, url);
        
        Console.WriteLine(account.PublicKey);
        Console.WriteLine(account.Address);
        
        // Deployment 
        web3.TransactionManager.UseLegacyAsDefault = true;
        var votingDbDeployment = new VotingDbDeployment();
            votingDbDeployment.Votes = SeedData.Votes;
            votingDbDeployment.Candidates = SeedData.Candidates;
            votingDbDeployment.Sections = SeedData.Sections;
            votingDbDeployment.Timestamp = SeedData.Timestamp;

        var clock = new Stopwatch();
        clock.Start();
        var transactionReceiptDeployment = await web3.Eth.GetContractDeploymentHandler<VotingDbDeployment>().SendRequestAndWaitForReceiptAsync(votingDbDeployment);
        clock.Stop();
        Console.WriteLine("Transaction Time: " + clock.Elapsed);
        var contractAddress = transactionReceiptDeployment.ContractAddress;
        var contractService = new VotingDbService(web3, contractAddress);
        // var contractHandler = web3.Eth.GetContractHandler(contractAddress);
        clock.Reset();
        clock.Start();
        var result = await contractService.GetAllVotesQueryAsync();
        clock.Stop();
        Console.WriteLine("Transaction Time: " + clock.Elapsed);
        Console.WriteLine(result.ReturnValue3.Count);

        foreach (var element in result.ReturnValue3)
        {
            foreach (var votes in element)
            {
                Console.Write(votes + ",");
            }
            Console.WriteLine();
        }
        
        /** Function: findSection**/
        /*
        var findSectionFunction = new FindSectionFunction();
        findSectionFunction.SectionID = sectionID;
        var findSectionFunctionReturn = await contractHandler.QueryAsync<FindSectionFunction, bool>(findSectionFunction);
        */


        /** Function: getAllVotes**/
        /*
        var getAllVotesOutputDTO = await contractHandler.QueryDeserializingToObjectAsync<GetAllVotesFunction, GetAllVotesOutputDTO>();
        */


        /** Function: getTotalVotesByCandidate**/
        /*
        var getTotalVotesByCandidateFunction = new GetTotalVotesByCandidateFunction();
        getTotalVotesByCandidateFunction.Candidate = candidate;
        var getTotalVotesByCandidateFunctionReturn = await contractHandler.QueryAsync<GetTotalVotesByCandidateFunction, uint>(getTotalVotesByCandidateFunction);
        */


        /** Function: getTotalVotesBySection**/
        /*
        var getTotalVotesBySectionFunction = new GetTotalVotesBySectionFunction();
        getTotalVotesBySectionFunction.SectionID = sectionID;
        var getTotalVotesBySectionFunctionReturn = await contractHandler.QueryAsync<GetTotalVotesBySectionFunction, uint>(getTotalVotesBySectionFunction);
        */


        /** Function: getTotalVotesInBlock**/
        /*
        var getTotalVotesInBlockFunctionReturn = await contractHandler.QueryAsync<GetTotalVotesInBlockFunction, uint>();
        */


        /** Function: getVotesByCandidate**/
        /*
        var getVotesByCandidateFunction = new GetVotesByCandidateFunction(); 
        getVotesByCandidateFunction.Candidate = candidate;
        var getVotesByCandidateOutputDTO = await contractHandler.QueryDeserializingToObjectAsync<GetVotesByCandidateFunction, GetVotesByCandidateOutputDTO>(getVotesByCandidateFunction);
        */


        /** Function: getVotesBySection**/
        /*
        var getVotesBySectionFunction = new GetVotesBySectionFunction(); 
        getVotesBySectionFunction.SectionID = sectionID;
        var getVotesBySectionOutputDTO = await contractHandler.QueryDeserializingToObjectAsync<GetVotesBySectionFunction, GetVotesBySectionOutputDTO>(getVotesBySectionFunction);
        */


        /** Function: getVotesOnSectionByCandidate**/
        /*
        var getVotesOnSectionByCandidateFunction = new GetVotesOnSectionByCandidateFunction();
        getVotesOnSectionByCandidateFunction.SectionID = sectionID;
        getVotesOnSectionByCandidateFunction.Candidate = candidate;
        var getVotesOnSectionByCandidateFunctionReturn = await contractHandler.QueryAsync<GetVotesOnSectionByCandidateFunction, uint>(getVotesOnSectionByCandidateFunction);
        */
    }

}