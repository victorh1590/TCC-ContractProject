using Grpc.Core;
using Voting.Server.Protos;
using Google.Protobuf.WellKnownTypes;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using System.Linq;
using CommunityToolkit.Diagnostics;
using static Voting.Server.Protos.VotingService;
using CandidateVotes = Voting.Server.Domain.Models.CandidateVotes;

namespace Voting.Server.Services;

public class VotingService : VotingServiceBase
{
    private readonly DomainService _domainService;


    internal VotingService(DomainService domainService)
    {
        _domainService = domainService;
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<SectionListReply> GetCandidateVotes(CandidateIdRequest request, ServerCallContext context)
    {
        List<Section> sections = await _domainService.GetVotesByCandidateAsync(request.Candidate);
        SectionListReply reply = new SectionListReply();
        foreach(Section section in sections)
        {
            List<Protos.CandidateVotes> cvList = new List<Protos.CandidateVotes>();
            foreach(Domain.Models.CandidateVotes cv in section.CandidateVotes)
            {
                Protos.CandidateVotes cvReply = new Protos.CandidateVotes
                {
                    Candidate = cv.Candidate,
                    Votes = cv.Votes
                };
                cvList.Add(cvReply);
            }

            SectionReply sectionReply = new SectionReply
            {
                SectionID = section.SectionID
            };
            sectionReply.CandidateVotes.AddRange(cvList);
            reply.Sections.Add(sectionReply);
        }
        return await Task.FromResult(reply);
    }

    //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public override async Task<SectionReply> GetSectionVotes(SectionIdRequest request, ServerCallContext context)
    {
        Section section = await _domainService.GetSectionAsync(request.Section);
        List<Protos.CandidateVotes> cvList = new List<Protos.CandidateVotes>();
        foreach(Domain.Models.CandidateVotes cv in section.CandidateVotes)
        {
            Protos.CandidateVotes cvReply = new Protos.CandidateVotes
            {
                Candidate = cv.Candidate,
                Votes = cv.Votes
            };
            cvList.Add(cvReply);
        }

        SectionReply sectionReply = new SectionReply
        {
            SectionID = section.SectionID
        };
        sectionReply.CandidateVotes.AddRange(cvList);

        return await Task.FromResult(sectionReply);
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
    public override async Task CreateSection(IAsyncStreamReader<SectionCreationRequest> requestStream, IServerStreamWriter<SectionsCreationReply> responseStream, ServerCallContext context)
    {
        await foreach (SectionCreationRequest sectionCreationRequest in requestStream.ReadAllAsync())
        {
            List<Section> sectionList = new();
            List<CandidateVotes> cv = new();
            foreach (var candidateVotes in sectionCreationRequest.Section.CandidateVotes)
            {
                Guard.IsNotNull(candidateVotes);
                cv.Add(new CandidateVotes
                {
                    Candidate = candidateVotes.Candidate,
                    Votes = candidateVotes.Votes
                });
            }
            Section section = new Section(sectionCreationRequest.Section.SectionID, cv);

            sectionList.Add(section);
            await _domainService.InsertSectionsAsync(sectionList);
        }
    }
}