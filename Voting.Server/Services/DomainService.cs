using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Protos;
using Voting.Server.Utils.Mappings;

// using Voting.Server.Domain.Models;

[assembly: InternalsVisibleTo("Voting.Server.Tests.Unit")]
[assembly: InternalsVisibleTo("Voting.Server.Tests.Integration")]
[assembly: InternalsVisibleTo("Voting.Server.Tests.Utils")]
namespace Voting.Server.Services;

public class DomainService
{
    private IVotingDbRepository Repository { get; }

    public DomainService(IVotingDbRepository repository)
    {
        Repository = repository;
    }

    public async Task<Section> GetSectionAsync(uint sectionNumber = 0)
    {
        SectionEventDTO? result = await Repository.ReadSectionAsync(sectionNumber);
        return Mappings.SectionEventDTOToSection(result);
    }

    public async Task<List<Section>> GetSectionRangeAsync(uint[] sectionNumbers)
    {
        Guard.IsNotEmpty(sectionNumbers);
        
        List<Section> sectionVotesList = new();
        foreach (var sectionNumber in sectionNumbers)
        {
            SectionEventDTO? result = await Repository.ReadSectionAsync(sectionNumber);
            if(result != null) sectionVotesList.Add(Mappings.SectionEventDTOToSection(result));
        }

        Guard.IsNotEmpty(sectionVotesList);
        return sectionVotesList;
    }

    public async Task<List<Section>> GetVotesByCandidateAsync(uint candidateNumber = 0)
    {
        List<CandidateEventDTO> result = await Repository.ReadVotesByCandidateAsync(candidateNumber);
        List<Section> votesByCandidate = Mappings.CandidateEventDTOListToSectionList(result);
        return votesByCandidate;
    }
    
    
    public async Task<Section> GetVotesByCandidateAndSectionAsync(uint candidateNumber = 0, uint sectionNumber = 0)
    {
        CandidateEventDTO? result = await Repository.ReadVotesByCandidateAndSectionAsync(candidateNumber, sectionNumber);
        Section mappedResult = Mappings.CandidateEventDTOToSection(result);
        return mappedResult;
    }
    
    public async Task<long> GetTotalVotesByCandidateAsync(uint candidateNumber = 0)
    {
        List<Section> result = await GetVotesByCandidateAsync(candidateNumber);
        Guard.IsNotEmpty(result);
        return result.Select(section => section.CandidateVotes.Single()).Sum(cv => cv.Votes);
    }

    public async Task<long> GetTotalVotesBySectionAsync(uint sectionNumber = 0)
    {
        Section result = await GetSectionAsync(sectionNumber);
        return result.CandidateVotes.Sum(cv => cv.Votes);
    }
    
    public async Task InsertSectionsAsync(List<Section> sections)
    {
        List<Section> uniqueSections = await RemoveRedundantSectionsAsync(sections);
        Guard.IsNotEmpty(uniqueSections);
        VotingDbDeployment deployment = Mappings.SectionsListToDeployment(uniqueSections);
        TransactionReceipt receipt =  await Repository.CreateSectionRange(deployment);
        Guard.IsEqualTo(receipt.Status.ToLong(), 1);
    }

    internal async Task<List<Section>> RemoveRedundantSectionsAsync(List<Section> sections)
    {
        List<uint> sectionsToRemove = new();
        foreach (var section in sections)
        {
            bool sectionExists = await SectionExistsAsync(section.SectionID);
            if (sectionExists)
            {
                sectionsToRemove.Add(section.SectionID);
            }
        }

        return sections.Where(section => !sectionsToRemove.Contains(section.SectionID)).ToList();
    }
    
    internal async Task<bool> SectionExistsAsync(uint sectionNumber = 0)
    {
        SectionEventDTO? result = await Repository.ReadSectionAsync(sectionNumber);
        return result != null && result.Section == sectionNumber;
    }

}