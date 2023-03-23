using CommunityToolkit.Diagnostics;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public partial class VotingDbRepository
{
    private enum FilterRange
    {
        FromEarliestToLatest,
        FromLatestToLatest
    }

    private static void GetFilterRangeSettings(FilterRange range, out BlockParameter from, out BlockParameter to)
    {
        switch (range)
        {
            case FilterRange.FromLatestToLatest:
                from = BlockParameter.CreateLatest();
                to = BlockParameter.CreateLatest();
                break;
            case FilterRange.FromEarliestToLatest:
            default:
                from = BlockParameter.CreateEarliest();
                to = BlockParameter.CreateLatest();
                break;
        }
    }
    //
    // private Event<T> UseFilter<T>(IWeb3? web3 = null)
    //     where T : IEventDTO, new()
    // {
    //     Guard.IsNotNull(web3);
    //     return web3.Eth.GetEvent<T>();
    // }

    public async Task<Section> GetSectionAsync(uint sectionNumber = 0)
    {
        Guard.IsNotEqualTo(sectionNumber, 0);
        
        Event<SectionEventDTO> sectionEventHandler = Web3.Eth.GetEvent<SectionEventDTO>();
        GetFilterRangeSettings(FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput sectionEventFilter = sectionEventHandler.CreateFilterInput(sectionNumber, from, to);
        List<EventLog<SectionEventDTO>>? sectionLogs = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        Guard.IsNotNull(sectionLogs);
        Guard.IsNotEmpty(sectionLogs);
        return Mappings.SectionEventDTOToSection(sectionLogs.FirstOrDefault()!.Event);
    }
    
    public async Task<List<Section>> GetSectionRangeAsync(uint[] sectionNumbers)
    {
        Guard.IsNotEmpty(sectionNumbers);
        
        List<Section> sectionVotesList = new();
        foreach (var sectionNumber in sectionNumbers)
        {
            Section section = await GetSectionAsync(sectionNumber);
            sectionVotesList.Add(section);
        }
        return sectionVotesList;
    }
    
    public async Task<Section> GetVotesByCandidateForSection(uint candidate = 0, uint sectionNumber = 0)
    {
        Guard.IsNotEqualTo(candidate, 0);
        Guard.IsNotEqualTo(sectionNumber, 0);
        
        Event<CandidateEventDTO> candidateEventHandler = Web3.Eth.GetEvent<CandidateEventDTO>();
        GetFilterRangeSettings(FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput candidateEventFilter = candidateEventHandler.CreateFilterInput(sectionNumber, from, to);
        List<EventLog<CandidateEventDTO>>? candidateLogs = await candidateEventHandler.GetAllChangesAsync(candidateEventFilter);
        Guard.IsNotNull(candidateLogs);
        Guard.IsNotEmpty(candidateLogs);

        return Mappings.CandidateEventDTOToSection(candidateLogs.FirstOrDefault()!.Event);
    }
}