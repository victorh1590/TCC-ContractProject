using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Domain.Models;
using Voting.Server.Tests.Utils;
using static NUnit.Framework.TestContext;

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
                .Take(CurrentContext.Random.Next(1, _seedData.Sections.Count))
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
        Assert.That(resultSections, Is.Not.SameAs(expectedSections));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.Not.SameAs(expectedSections.Select(section => section.CandidateVotes)));
        CollectionAssert.AreEquivalent(
            resultSections.Select(section => section.CandidateVotes).ToArray(),
            expectedSections.Select(section => section.CandidateVotes).ToArray());
        CollectionAssert.AreEquivalent(
            resultSections.Select(section => section.SectionID).ToArray(),
            expectedSections.Select(section => section.SectionID).ToArray());
    }
    
    [Test]
    [Repeat(5)]
    public void GetSectionRangeAsync_Should_Fail_When_All_SectionNums_Are_Invalid()
    {
        //Generate a list of sections to look for and print sectionNums.
        List<uint> sectionNumbers = new();
        for (int i = 0; i < 10; i++)
        {
            sectionNumbers.Add(CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1));
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
                .Take(CurrentContext.Random.Next(1, _seedData.Sections.Count - invalidDataVariance))
                .ToArray()
            );
        Guard.IsNotNull(expectedSectionsWithInvalids);
    
        //Copies Valid Data
        List<Section> expectedSectionsValidOnly = new(expectedSectionsWithInvalids);
    
        //Add Invalid Data
        for (int i = 0; i < invalidDataVariance; i++)
        {
            expectedSectionsWithInvalids.Add(new Section(
                CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1),
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
        Assert.That(resultSections, Is.Not.SameAs(expectedSectionsValidOnly));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.Not.SameAs(expectedSectionsValidOnly.Select(section => section.CandidateVotes)));
        Assert.That(resultSections.Select(section => section.SectionID), 
            Is.EquivalentTo(expectedSectionsValidOnly.Select(section => section.SectionID)));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.EquivalentTo(expectedSectionsValidOnly.Select(section => section.CandidateVotes)));
    }
}