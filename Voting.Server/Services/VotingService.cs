using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Voting.Server.Protos.v1;
using static Voting.Server.Protos.v1.VotingService;

namespace Voting.Server.Services;

public class VotingService : VotingServiceBase
{
    private readonly DomainService _domainService;
    private readonly ILogger<VotingService> _logger;

    private int MaxRepeats => 5;
    private int MaxListSize => 30;

    public VotingService(DomainService domainService, ILogger<VotingService> logger)
    {
        _domainService = domainService;
        _logger = logger;
    }

    public override async Task GetCandidateVotes(CandidateIdRequest request, IServerStreamWriter<Section> responseStream, ServerCallContext context)
    {
        List<Section> sections = await _domainService.GetVotesByCandidateAsync(request.Candidate);
        foreach (var section in sections)
        {
            await responseStream.WriteAsync(section);
        }
    }

    public override async Task<Section> GetSectionVotes(SectionIdRequest request, ServerCallContext context)
    {
        return await _domainService.GetSectionAsync(request.Section);
    }

    public override async Task<TotalVotesReply> GetTotalVotesBySection(SectionIdRequest request, ServerCallContext context)
    {
        long totalVotes = await _domainService.GetTotalVotesBySectionAsync(request.Section);
        return new TotalVotesReply { TotalVotes = totalVotes };
    }

    public override async Task<TotalVotesReply> GetTotalVotesByCandidate(CandidateIdRequest request, ServerCallContext context)
    {
        long totalVotes = await _domainService.GetTotalVotesByCandidateAsync(request.Candidate);
        return new TotalVotesReply { TotalVotes = totalVotes };
    }

    public override async Task<Empty> CreateSection(IAsyncStreamReader<Section> requestStream, ServerCallContext context)
    {
        List<Section> sectionList = new List<Section>();
        await foreach (Section section in requestStream.ReadAllAsync())
        {
            if (sectionList.Count < MaxListSize)
            {
                sectionList.Add(section);
                continue;
            }
            else
            {
                int repeatCount = 0;
                while (repeatCount < MaxRepeats)
                {
                    try
                    {
                        _logger.LogInformation($"Trying to create section ");
                        sectionList.ForEach(s => _logger.LogInformation($"{s.SectionID}"));
                        await _domainService.InsertSectionsAsync(sectionList);
                        break;
                    }
                    catch (RpcException e)
                    {
                        _logger.LogInformation($"Failed to create sections. \n{e.Status}");
                        repeatCount++;
                    }
                }
                _logger.LogInformation($"Success creating sections ");
                sectionList.ForEach(s => _logger.LogInformation($"{s.SectionID}"));
                sectionList = new List<Section>();
            }
        }

        return new Empty();
    }
}