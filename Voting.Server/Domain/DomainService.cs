using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Voting.Server.Persistence;
using Voting.Server.Persistence.ContractDefinition;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;

[assembly: InternalsVisibleTo("Voting.Server.UnitTests")]
namespace Voting.Server.Domain;

internal class DomainService
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

    // public async Task GetVotesByCandidate(uint candidate = 0)
    // {
    //     //TODO return SectionVotes[i].Votes["Name"]
    // }
    //
    
    public async Task<Section> GetVotesByCandidateForSection(uint candidate = 0, uint sectionNumber = 0)
    {
        CandidateEventDTO? result = await Repository.ReadVotesByCandidateAndSectionAsync(candidate, sectionNumber);
        Section mappedResult = Mappings.CandidateEventDTOToSection(result);
        return mappedResult;
    }
    
    public async void InsertSections(List<Section> sections)
    {
        List<Section> uniqueSections = await RemoveRedundantSections(sections);
        Guard.IsNotEmpty(uniqueSections);
        VotingDbDeployment deployment = Mappings.SectionsListToDeployment(uniqueSections);
        TransactionReceipt receipt =  await Repository.CreateSectionRange(deployment);
        Guard.IsEqualTo(receipt.Status.ToLong(), 1);
    }

    private async Task<List<Section>> RemoveRedundantSections(List<Section> sections)
    {
        foreach (var section in sections)
        {
            bool sectionExists = await SectionExists(section.SectionID);
            if (!sectionExists)
            {
                sections.Remove(section);
            }
        }

        return sections;
    }
    
    private async Task<bool> SectionExists(uint sectionNumber = 0)
    {
        SectionEventDTO? result = await Repository.ReadSectionAsync(sectionNumber);
        return result != null && result.Section == sectionNumber;
    }

}