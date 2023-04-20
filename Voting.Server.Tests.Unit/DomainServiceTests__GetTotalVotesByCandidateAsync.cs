using Voting.Server.Tests.Utils;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class DomainServiceTests
{
    [Test]
    [Repeat(10)]
    public async Task GetTotalVotesByCandidateAsync_Should_Return_Sum_Of_Votes_For_Candidate()
    {
        //Select a valid candidate.
        uint expectedCandidate = _seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());
        
        //Sum votes for expected candidate.
        long expectedVoteCount = _candidateEventDTOs
            .Where(dto => dto.Candidate == expectedCandidate)
            .Sum(section => section.Votes);

        //Calls method and convert results to JSON.
        long resultVoteCount = await _domainService.GetTotalVotesByCandidateAsync(expectedCandidate);
        
        //Assertions
        Assert.That(resultVoteCount, Is.Not.LessThan(0));
        Assert.That(resultVoteCount, Is.EqualTo(expectedVoteCount));
    }
    
    [Test]
    [Repeat(5)]
    public void GetTotalVotesByCandidateAsync_Should_Fail_When_Candidate_Is_Invalid()
    {
        //Select a candidate number out of bounds.
        uint expectedCandidate = CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1);

        //Assertions
        //Invalid candidate number
        Assert.That(async() => await _domainService.GetTotalVotesByCandidateAsync(expectedCandidate),
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        //No argument (default is zero).
        Assert.That(async() => await _domainService.GetTotalVotesByCandidateAsync(),
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
    }
}