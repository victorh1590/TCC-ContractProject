using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Domain.Models;
using Voting.Server.Tests.Utils;

namespace Voting.Server.Tests.Unit;

public partial class DomainServiceTests
{
    [Test]
    [Repeat(10)]
    public async Task GetSectionRangeAsync_Should_Return_Correct_Data_When_All_SectionNums_Are_Valid()
    {
        //Generate a list of sections to look for.
        List<Section> expectedSections = new();
        expectedSections
            .AddRange(_seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, _seedData.Sections.Count))
                .ToArray()
            );
        Guard.IsNotNull(expectedSections);

        uint[] sectionNumbers = expectedSections.Select(section => section.SectionID).ToArray();

        //Calls method and convert results to JSON.
        List<Section> resultSections = await _domainService.GetSectionRangeAsync(sectionNumbers);

        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSections);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSections.Count, Is.EqualTo(expectedSections.Count));
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.CandidateVotes).ToArray(),
            expectedSections.Select(item => item.CandidateVotes).ToArray());
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.SectionID).ToArray(),
            expectedSections.Select(item => item.SectionID).ToArray());
    }
    
    [Test]
    [Repeat(5)]
    public void GetSectionRangeAsync_Should_Fail_When_All_SectionNums_Are_Invalid()
    {
        //Generate a list of sections to look for and print sectionNums.
        List<uint> sectionNumbers = new();
        for (int i = 0; i < 10; i++)
        {
            sectionNumbers.Add(TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1));
        }
    
        //Assertions.
        Assert.That(
            async () => await _domainService.GetSectionRangeAsync(sectionNumbers.ToArray()),
            Throws.InstanceOf<ArgumentException>());
    }
    
    [Test, Sequential]
    public async Task GetSectionRangeAsync_Should_Return_Partial_Data_When_Part_Of_The_Sections_Are_Invalid(
        [Range(1, 5, 1)] int invalidDataVariance)
    {
        //Generate a list of sections to look for.
        List<Section> expectedSectionsWithInvalids = new();
        expectedSectionsWithInvalids
            .AddRange(_seedData.Sections
                .OrderBy(_ => Guid.NewGuid())
                .Take(TestContext.CurrentContext.Random.Next(1, _seedData.Sections.Count - invalidDataVariance))
                .ToArray()
            );
        Guard.IsNotNull(expectedSectionsWithInvalids);
    
        //Copies Valid Data
        List<Section> expectedSectionsValidOnly = new(expectedSectionsWithInvalids);
    
        //Add Invalid Data
        for (int i = 0; i < invalidDataVariance; i++)
        {
            expectedSectionsWithInvalids.Add(new Section(
                TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1),
                new List<CandidateVotes>()));
        }
    
        //Calls method and convert results to JSON.
        uint[] sectionNumbers = expectedSectionsWithInvalids.Select(section => section.SectionID).ToArray();
        List<Section> resultSections = await _domainService.GetSectionRangeAsync(sectionNumbers);
    
        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSectionsValidOnly);
    
        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        Assert.That(resultSections.Count, Is.EqualTo(expectedSectionsValidOnly.Count));
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.CandidateVotes).ToArray(),
            expectedSectionsValidOnly.Select(item => item.CandidateVotes).ToArray());
        CollectionAssert.AreEquivalent(
            resultSections.Select(item => item.SectionID).ToArray(),
            expectedSectionsValidOnly.Select(item => item.SectionID).ToArray());
    }
}