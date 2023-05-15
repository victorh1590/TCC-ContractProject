using Grpc.Core;
using Voting.Server.Protos;
using Google.Protobuf.WellKnownTypes;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using static Voting.Server.Protos.VotingService;

namespace Voting.Server.Services
{
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
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
        public override async Task<TotalVotesReply> GetTotalVotes(Empty request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
        public override async Task<TotalVotesReply> GetTotalVotesByCandidate(CandidateIdRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        //[global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
        public override async Task CreateSection(IAsyncStreamReader<SectionsCreationRequest> requestStream, IServerStreamWriter<SectionsCreationReply> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }
    }
}
