using Voting.Server.Domain.Models;
using Voting.Server.UnitTests.TestData;

namespace Voting.Server.UnitTests;

public partial class DomainServiceTests
{
    [Test]
    [Repeat(10)]
    public void SectionExistsAsync_Should_Return_True_When_SectionID_Is_Valid()
    {
        //Select a valid expected section.
        uint expectedSectionID = _seedData.Sections
            .OrderBy(_ => Guid.NewGuid())
            .First()
            .SectionID;
        
        Assert.That(async () => await _domainService.SectionExistsAsync(expectedSectionID), Is.True);
    }
    
    [Test]
    [Repeat(10)]
    public void SectionExistsAsync_Should_Return_False_When_SectionID_Is_Invalid()
    {
        Assert.That(async () => await _domainService.SectionExistsAsync(
                TestContext.CurrentContext.Random.NextUInt(SeedDataBuilder.MaxSectionID, uint.MaxValue - 1)), 
            Is.False);
    }
}