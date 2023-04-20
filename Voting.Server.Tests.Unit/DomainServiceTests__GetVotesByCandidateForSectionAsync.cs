using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Tests.Utils;
using CommunityToolkit.Diagnostics;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class DomainServiceTests
{
    [Test]
    [Repeat(10)]
    public async Task GetVotesByCandidateForSectionAsync_Should_Return_Correct_Data_When_All_CandidateNums_And_SectionNums_Are_Valid()
    {
        //Select a valid expected section.
        Section expectedSection = _seedData.Sections
            .OrderBy(_ => Guid.NewGuid())
            .First();
        expectedSection.CandidateVotes = new List<CandidateVotes>
        {
            expectedSection.CandidateVotes.MinBy(_ => Guid.NewGuid())
        };
        Guard.IsNotNull(expectedSection);
        Guard.IsNotEmpty(expectedSection.CandidateVotes);
        uint expectedCandidate = expectedSection.CandidateVotes.First().Candidate;

        //Calls method and convert results to JSON.
        Section resultSection = await _domainService.GetVotesByCandidateAndSectionAsync(
            expectedCandidate, expectedSection.SectionID);

        string resultJSON = JsonSerializer.Serialize(resultSection);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSection, Is.Not.SameAs(expectedSection));
        Assert.That(resultSection.CandidateVotes, Is.Not.SameAs(expectedSection.CandidateVotes));
        Assert.That(resultSection.CandidateVotes, Is.EquivalentTo(expectedSection.CandidateVotes));
        Assert.That(resultSection.SectionID, Is.EqualTo(expectedSection.SectionID));
    }
    
    [Test]
    [Repeat(5)]
    public void GetVotesByCandidateForSectionAsync_Should_Fail_When_CandidateNum_Is_Invalid()
    {
        uint validSectionNum = _seedData.Sections
            .OrderBy(_ => Guid.NewGuid())
            .First()
            .SectionID;
        Assert.That(async () => await _domainService.GetVotesByCandidateAndSectionAsync(
                CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1),
                validSectionNum), Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        Assert.That(async () => await _domainService.GetVotesByCandidateAndSectionAsync(0, validSectionNum), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        Assert.That(async () => await _domainService.GetVotesByCandidateAndSectionAsync(), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
    }
    
    [Test]
    [Repeat(5)]
    public void GetVotesByCandidateForSectionAsync_Should_Fail_When_SectionNum_Is_Invalid()
    {
        uint validCandidateNum = _seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());
        Assert.That(async () => await _domainService.GetVotesByCandidateAndSectionAsync(validCandidateNum,
            CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1)), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        Assert.That(async () => await _domainService.GetVotesByCandidateAndSectionAsync(validCandidateNum), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
    }
}