using Grpc.Core;
using Voting.Server.Protos;
using Google.Protobuf.WellKnownTypes;
using Voting.Server.Domain;
using static Voting.Server.Protos.VotingService;

namespace Voting.Server.Services;

public class VotingService : VotingServiceBase
{
    private readonly DomainService _domainService;
    private readonly ILogger<VotingService> _logger;

    internal VotingService(DomainService domainService, ILogger<VotingService> logger)
    {
        _domainService = domainService;
        _logger = logger;
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task GetCandidateVotes(CandidateIdRequest request, IServerStreamWriter<Section> responseStream, ServerCallContext context)
    {
        List<Section> sections = await _domainService.GetVotesByCandidateAsync(request.Candidate);
        foreach (var section in sections)
        {
            await responseStream.WriteAsync(section);
        }
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<Section> GetSectionVotes(SectionIdRequest request, ServerCallContext context)
    {
        return await _domainService.GetSectionAsync(request.Section);
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<TotalVotesReply> GetTotalVotesBySection(SectionIdRequest request, ServerCallContext context)
    {
        long totalVotes = await _domainService.GetTotalVotesBySectionAsync(request.Section);
        return new TotalVotesReply { TotalVotes = totalVotes };
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<TotalVotesReply> GetTotalVotesByCandidate(CandidateIdRequest request, ServerCallContext context)
    {
        long totalVotes = await _domainService.GetTotalVotesByCandidateAsync(request.Candidate);
        return new TotalVotesReply { TotalVotes = totalVotes };
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<Empty> CreateSection(IAsyncStreamReader<Section> requestStream, ServerCallContext context)
    {
        await foreach (Section section in requestStream.ReadAllAsync())
        {
            List<Section> sectionList = new List<Section> { section };
            await _domainService.InsertSectionsAsync(sectionList);
        }

        return new Empty();
    }
}