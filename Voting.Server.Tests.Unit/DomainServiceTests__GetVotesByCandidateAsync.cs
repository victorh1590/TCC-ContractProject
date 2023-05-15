using System.Text.Json;
// using Voting.Server.Domain.Models;
using Voting.Server.Tests.Utils;
using CommunityToolkit.Diagnostics;
using Voting.Server.Protos;
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
        List<Section> expectedSections = new();
        _candidateEventDTOs
            .Where(dto => dto.Candidate == expectedCandidate)
            .ToList()
            .ForEach(dto =>
            {
                Section section = new Section();
                CandidateVotes cv = new CandidateVotes
                {
                    Candidate = dto.Candidate,
                    Votes = dto.Votes
                };
                section.SectionID = dto.Section;
                section.CandidateVotes.Add(cv);

                expectedSections.Add(section);
            });
        Guard.IsNotNull(expectedSections);
        Guard.IsNotEmpty(expectedSections);

        //Calls method and convert results to JSON.
        List<Section> resultSections = await _domainService.GetVotesByCandidateAsync(expectedCandidate);

        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSections);

        //Assertions
        Assert.That(resultSections, Is.Not.Null.Or.Empty);
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSections, Is.Not.SameAs(expectedSections));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.Not.SameAs(expectedSections.Select(section => section.CandidateVotes)));
        Assert.That(resultSections.Select(section => section.SectionID), 
            Is.EquivalentTo(expectedSections.Select(section => section.SectionID)));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.EquivalentTo(expectedSections.Select(section => section.CandidateVotes)));
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