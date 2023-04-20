using Voting.Server.Tests.Utils;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class DomainServiceTests
{
    [Test]
    [Repeat(10)]
    public async Task GetTotalVotesBySectionAsync_Should_Return_Sum_Of_Votes_For_Candidate_On_A_Given_Section()
    {
        //Select a valid section.
        uint expectedSectionNumber = _seedData.Deployment.Sections.MinBy(_ => Guid.NewGuid());
        
        //Sum votes for expected section.
        long expectedVoteCount = _sectionEventDTOs
            .Single(dto => dto.Section == expectedSectionNumber)
            .Votes
            .Sum(votes => votes);

        //Calls method and convert results to JSON.
        long resultVoteCount = await _domainService.GetTotalVotesBySectionAsync(expectedSectionNumber);
        
        //Assertions
        Assert.That(resultVoteCount, Is.Not.LessThan(0));
        Assert.That(resultVoteCount, Is.EqualTo(expectedVoteCount));
    }
    
    [Test]
    [Repeat(5)]
    public void GetTotalVotesBySectionAsync_Should_Fail_When_Candidate_Is_Invalid()
    {
        //Select a section number out of bounds.
        uint expectedSectionNumber = CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1);

        //Assertions
        //Invalid section number
        Assert.That(async() => await _domainService.GetTotalVotesBySectionAsync(expectedSectionNumber),
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        //No argument (default is zero).
        Assert.That(async() => await _domainService.GetTotalVotesBySectionAsync(),
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
    }
}