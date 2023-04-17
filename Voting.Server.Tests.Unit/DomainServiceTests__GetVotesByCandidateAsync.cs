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
    public async Task GetVotesByCandidateAsync_Should_Return_Correct_Data_When_All_CandidateNums_And_SectionNums_Are_Valid()
    {
        //Select a valid expected section.
        uint expectedCandidate = _seedData.Deployment.Candidates.MinBy(_ => Guid.NewGuid());
        List<Section> expectedSectionList = new();
        _candidateEventDTOs
            .Where(dto => dto.Candidate == expectedCandidate)
            .ToList()
            .ForEach(dto => expectedSectionList.Add(new Section(
                dto.Section, new List<CandidateVotes> { new(dto.Candidate, dto.Votes) })
            ));
        Guard.IsNotNull(expectedSectionList);
        Guard.IsNotEmpty(expectedSectionList);

        //Calls method and convert results to JSON.
        List<Section> resultSectionList = await _domainService.GetVotesByCandidateAsync(expectedCandidate);

        string resultJSON = JsonSerializer.Serialize(resultSectionList);
        string expectedJSON = JsonSerializer.Serialize(expectedSectionList);

        //Assertions
        Assert.That(resultSectionList, Is.Not.Null.Or.Empty);
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSectionList.Select(s => s.SectionID), 
            Is.EquivalentTo(expectedSectionList.Select(s => s.SectionID)));
        Assert.That(resultSectionList.Select(s => s.CandidateVotes), 
            Is.EquivalentTo(expectedSectionList.Select(s => s.CandidateVotes)));
    }
    
    [Test]
    [Repeat(5)]
    public void GetVotesByCandidateAsync_Should_Fail_When_CandidateNum_Is_Invalid()
    {
        Assert.That(async () => await _domainService.GetVotesByCandidateAsync(
                CurrentContext.Random.NextUInt(SeedDataBuilder.MaxCandidateNumber, uint.MaxValue - 1)), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        Assert.That(async () => await _domainService.GetVotesByCandidateAsync(0), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
        Assert.That(async () => await _domainService.GetVotesByCandidateAsync(), 
            Throws.TypeOf<ArgumentException>().Or.TypeOf<ArgumentNullException>());
    }
}