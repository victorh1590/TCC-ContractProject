using CommunityToolkit.Diagnostics;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.Persistence;

public enum FilterRange
{
    FromEarliestToLatest,
    FromLatestToLatest
}

public partial class VotingDbRepository
{

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

    public async Task<SectionEventDTO?> ReadSectionAsync(uint sectionNumber = 0, FilterRange? range = null)
    {
        Guard.IsNotEqualTo(sectionNumber, 0);

        Event<SectionEventDTO> sectionEventHandler = Web3.Eth.GetEvent<SectionEventDTO>();
        GetFilterRangeSettings(range ?? FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput sectionEventFilter = sectionEventHandler.CreateFilterInput(sectionNumber, from, to);
        List<EventLog<SectionEventDTO>>? sectionLogList = await sectionEventHandler.GetAllChangesAsync(sectionEventFilter);
        EventLog<SectionEventDTO>? sectionLog = sectionLogList.FirstOrDefault();
        return sectionLog?.Event;
    }

    public async Task<CandidateEventDTO?> ReadVotesByCandidateAndSectionAsync(
        uint candidateNumber = 0, uint sectionNumber = 0, FilterRange? range = null)
    {
        Guard.IsNotEqualTo(candidateNumber, 0);
        Guard.IsNotEqualTo(sectionNumber, 0);

        Event<CandidateEventDTO> candidateEventHandler = Web3.Eth.GetEvent<CandidateEventDTO>();
        GetFilterRangeSettings(range ?? FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput candidateEventFilter = candidateEventHandler
            .CreateFilterInput(candidateNumber, sectionNumber, from, to);
        List<EventLog<CandidateEventDTO>>? candidateLogList = await candidateEventHandler.GetAllChangesAsync(candidateEventFilter);
        EventLog<CandidateEventDTO>? candidateLog = candidateLogList.FirstOrDefault();
        return candidateLog?.Event;
    }
    
    public async Task<List<CandidateEventDTO>> ReadVotesByCandidateAsync(uint candidateNumber = 0, FilterRange? range = null)
    {
        Guard.IsNotEqualTo(candidateNumber, 0);

        Event<CandidateEventDTO> candidateEventHandler = Web3.Eth.GetEvent<CandidateEventDTO>();
        GetFilterRangeSettings(range ?? FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput candidateEventFilter = candidateEventHandler.CreateFilterInput(candidateNumber, from, to);
        List<EventLog<CandidateEventDTO>>? candidateLogList = await candidateEventHandler.GetAllChangesAsync(candidateEventFilter);
        List<CandidateEventDTO> eventLogList = candidateLogList.Select(eventLog => eventLog.Event).ToList();
        return eventLogList;
    }

    public async Task<MetadataEventDTO?> ReadMetadataAsync(string contractAddress, FilterRange? range = null)
    {
        Guard.IsNotEmpty(contractAddress);

        Event<MetadataEventDTO> metadataEventHandler = Web3.Eth.GetEvent<MetadataEventDTO>();
        GetFilterRangeSettings(range ?? FilterRange.FromEarliestToLatest, out BlockParameter from, out BlockParameter to);
        NewFilterInput metadataEventFilter = metadataEventHandler.CreateFilterInput(contractAddress, from, to);
        List<EventLog<MetadataEventDTO>>? metadataLogList = await metadataEventHandler.GetAllChangesAsync(metadataEventFilter);
        EventLog<MetadataEventDTO>? metadataLog = metadataLogList.SingleOrDefault();
        return metadataLog?.Event;
    }
}