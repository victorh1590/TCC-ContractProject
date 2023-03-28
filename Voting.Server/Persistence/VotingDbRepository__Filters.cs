using CommunityToolkit.Diagnostics;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
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
    
    public async Task<SectionEventDTO?> ReadSectionAsync(uint sectionNumber = 0)
    {
        Guard.IsNotEqualTo(sectionNumber, 0);
        
        Event<SectionEventDTO> sectionEventHandler = Web3.Eth.GetEvent<SectionEventDTO>();
        GetFilterRangeSettings(FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput sectionEventFilter = sectionEventHandler.CreateFilterInput(sectionNumber, from, to);
        List<EventLog<SectionEventDTO>>? sectionLogList = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        EventLog<SectionEventDTO>? sectionLog = sectionLogList.FirstOrDefault();
        return sectionLog?.Event;
    }

    public async Task<CandidateEventDTO?> ReadVotesByCandidateAndSection(uint candidate = 0, uint sectionNumber = 0)
    {
        Guard.IsNotEqualTo(candidate, 0);
        Guard.IsNotEqualTo(sectionNumber, 0);
        
        Event<CandidateEventDTO> candidateEventHandler = Web3.Eth.GetEvent<CandidateEventDTO>();
        GetFilterRangeSettings(FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput candidateEventFilter = candidateEventHandler.CreateFilterInput(sectionNumber, from, to);
        List<EventLog<CandidateEventDTO>>? candidateLogList = await candidateEventHandler.GetAllChangesAsync(candidateEventFilter);
        EventLog<CandidateEventDTO>? candidateLog = candidateLogList.FirstOrDefault();
        return candidateLog?.Event;
    }
    
    internal async Task<MetadataEventDTO?> ReadMetadata(string contractAddress)
    {
        Guard.IsNotEmpty(contractAddress);

        Event<MetadataEventDTO> metadataEventHandler = Web3.Eth.GetEvent<MetadataEventDTO>();
        GetFilterRangeSettings(FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput metadataEventFilter = metadataEventHandler.CreateFilterInput(contractAddress, from, to);
        List<EventLog<MetadataEventDTO>>? metadataLogList = await metadataEventHandler.GetAllChangesAsync(metadataEventFilter);
        EventLog<MetadataEventDTO>? metadataLog = metadataLogList.FirstOrDefault();
        return metadataLog?.Event;
    }
}