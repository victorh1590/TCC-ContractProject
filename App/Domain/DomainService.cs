using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Nethereum.ABI;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Voting.Server.Domain;

internal class DomainService
{
    private string URL { get; }
    private int ChainID { get; }
    private string PrivateKey { get; }

    public DomainService()
    {
        URL = "HTTP://localhost:7545";
        ChainID = 5777;
        PrivateKey = "d1d45d0629c5a5fe5ac563c89d759ca057cb7e2936f6c78351bb93bb2f58eb99";
    }

    private Web3 GetWeb3Client()
    {
        Account account = new Account(PrivateKey, 5777);
        return new Web3(account, URL);
    }

    public async Task<string> DeployContract(VotingDbDeployment votingDbDeployment)
    {
        Web3 web3 = GetWeb3Client();
        web3.TransactionManager.UseLegacyAsDefault = true;
        TransactionReceipt transactionReceipt = await web3.Eth
            .GetContractDeploymentHandler<VotingDbDeployment>()
            .SendRequestAndWaitForReceiptAsync(votingDbDeployment);
        return transactionReceipt.ContractAddress;
    }

    private VotingDbService GetVotingDbService(string contractAddress)
    {
        Web3 web3 = GetWeb3Client();
        return new VotingDbService(web3, contractAddress);
    }

    public async Task GetSectionAsync(uint sectionNumber = 0)
    {
        Web3 web3 = GetWeb3Client();
        Event<SectionEventDTO> sectionEventHandler = web3.Eth.GetEvent<SectionEventDTO>();
        var latest = BlockParameter.CreateLatest();
        var sectionEventFilter = sectionEventHandler.CreateFilterInput(sectionNumber, latest, latest);
        var logs = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        Console.WriteLine(logs);
        Console.WriteLine(logs.Count);
        foreach (var log in logs)
        {
            Console.WriteLine("Section: " + log.Event.Section);
            Console.WriteLine("Votes: " + log.Event.Votes);
            Console.WriteLine("Contract Address: " + log.Event.ContractAddress);
        }

        //TODO should return SectionVotes
    }

    //public async Task GetSectionRangeAsync(uint[]? sectionNumber = null)
    public async Task GetSectionRangeAsync(uint candidate = 0)
    {
        Web3 web3 = GetWeb3Client();
        Event<CandidateEventDTO> sectionEventHandler = web3.Eth.GetEvent<CandidateEventDTO>();
        var latest = BlockParameter.CreateLatest();
        var sectionEventFilter = sectionEventHandler.CreateFilterInput(candidate, latest, latest);
        var logs = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        Console.WriteLine(logs);
        Console.WriteLine(logs.Count);
        foreach (var log in logs)
        {
            Console.WriteLine("Candidate Number: " + log.Event.Candidate);
            Console.WriteLine("Section: " + log.Event.Section);
            Console.WriteLine("Votes: " + log.Event.Votes);
        }

        //TODO should return SectionVotes[]
    }

    // public async Task GetVotesByCandidate(string candidate)
    // {
    //     //TODO return SectionVotes[i].Votes["Name"]
    // }
    //
    // public async Task GetVotesByCandidateForSection(string candidate, uint sectionNumber)
    // {
    //     //TODO return SectionVotes.Votes["Name"]
    // }
    //
    // public async Task GetAllVotesByCandidate(string candidate)
    // {
    //     //TODO return Soma SectionVotes[i].Votes["Name"]
    // }
    //
    // public async Task GetAllVotes()
    // {
    //     //TODO return Soma SectionVotes[]
    // }
    //
    // public async Task CreateSection()
    // {
    //     //TODO call deploy a contract with the section.
    // }
    //
    // public async Task CreateSectionRange()
    // {
    //     //TODO call deploy a contract with sections.
    // }
}