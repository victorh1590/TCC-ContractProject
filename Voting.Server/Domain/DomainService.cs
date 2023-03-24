using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Nethereum.ABI;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Utils;

[assembly: InternalsVisibleTo("Voting.Server.UnitTests")]
namespace Voting.Server.Domain;

internal class DomainService
{
    // private string URL { get; }
    // private int ChainID { get; }
    // private string PrivateKey { get; }
    // public Account Acc { get; set; }
    // public IWeb3 Web3 { get; set; }
    private IVotingDbRepository Repository { get; }

    public DomainService(IVotingDbRepository repository)
    {
        // URL = "HTTP://localhost:7545";
        // ChainID = 5777;
        // PrivateKey = "d1d45d0629c5a5fe5ac563c89d759ca057cb7e2936f6c78351bb93bb2f58eb99";
        // Acc = new Account(PrivateKey, ChainID);
        // Web3 = new Web3(Acc, URL);
        Repository = repository;
    }
    
    // public DomainService(string url, int chainID, string privateKey, IVotingDbRepository repository)
    // {
    //     // URL = url;
    //     // ChainID = chainID;
    //     // PrivateKey = privateKey;
    //     // Acc = new Account(privateKey, chainID);
    //     // Web3 = new Web3(Acc, URL);
    //     Repository = repository;
    // }

    // public async Task<string> DeployContract(VotingDbDeployment votingDbDeployment)
    // {
    //     Web3.TransactionManager.UseLegacyAsDefault = true;
    //     TransactionReceipt transactionReceipt = await Web3.Eth
    //         .GetContractDeploymentHandler<VotingDbDeployment>()
    //         .SendRequestAndWaitForReceiptAsync(votingDbDeployment);
    //     return transactionReceipt.ContractAddress;
    // }

    // private VotingDbRepository GetVotingDbService(string contractAddress)
    // {
    //     return new VotingDbRepository(Web3, contractAddress);
    // }

    public async Task<Section> GetSectionAsync(uint sectionNumber = 0)
    {
        return await Repository.GetSectionAsync(sectionNumber);
    }

    public async Task<List<Section>> GetSectionRangeAsync(uint[] sectionNumbers)
    {
        return await Repository.GetSectionRangeAsync(sectionNumbers);
    }

    // public async Task GetVotesByCandidate(uint candidate = 0)
    // {
    //     //TODO return SectionVotes[i].Votes["Name"]
    // }
    //
    
    public async Task<dynamic> GetVotesByCandidateForSection(uint candidate = 0, uint sectionNumber = 0)
    {
        return await Repository.GetVotesByCandidateForSection(candidate, sectionNumber);
    }
    
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
    public async Task<string> CreateSection(Section section)
    {
        Guard.IsNotEqualTo(section.SectionID, 0);

        List<uint> candidates = section.CandidateVotes.Select(x => x.Candidate).ToList();
        Guard.IsNotEmpty(candidates);

        List<List<uint>> votes = new() { section.CandidateVotes.Select(x => x.Votes).ToList() };
        Guard.IsNotEmpty(votes.First());
        Guard.IsNotEqualTo(votes.First().Count, candidates.Count);
        
        string sectionJSON = JsonSerializer.Serialize(section);
        Guard.IsNotNullOrEmpty(sectionJSON);

        Compression compression = new();
        string compressedSection = compression.Compress(sectionJSON);
        Guard.IsNotNullOrEmpty(compressedSection);

        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString(CultureInfo.InvariantCulture);

        VotingDbDeployment deployment = new()
        {
            Votes = votes,
            Candidates = candidates,
            Sections = new List<uint> { section.SectionID },
            Timestamp = timestamp,
            CompressedSectionData = compressedSection
        };

        return await Repository.DeployContractAsync(deployment);
    }
    //
    public async Task<string> CreateSectionRange(List<Section> sections)
    {
        // Guard.IsNotEqualTo(sections.SectionID, 0);
        Guard.IsNotEmpty(sections);
        Guard.IsFalse(sections.Any(section => section.SectionID == 0));

        List<uint> candidates = sections.First().CandidateVotes.Select(x => x.Candidate).ToList();
        Guard.IsNotEmpty(candidates);

        List<List<uint>> votes = new();
        foreach (var section in sections)
        {
            votes.Add(section.CandidateVotes.Select(x => x.Votes).ToList());
            Guard.IsNotEmpty(votes.First());
            Guard.IsEqualTo(votes.First().Count, candidates.Count);
        }

        string sectionJSON = JsonSerializer.Serialize(sections);
        Guard.IsNotNullOrEmpty(sectionJSON);

        Compression compression = new();
        string compressedSection = compression.Compress(sectionJSON);
        Guard.IsNotNullOrEmpty(compressedSection);

        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString(CultureInfo.InvariantCulture);

        VotingDbDeployment deployment = new()
        {
            Votes = votes,
            Candidates = candidates,
            Sections = sections.Select(section => section.SectionID).ToList(),
            Timestamp = timestamp,
            CompressedSectionData = compressedSection
        };

        return await Repository.DeployContractAsync(deployment);
    }
}