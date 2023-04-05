using Voting.Server.UnitTests.TestData;
using Moq;
using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests;

[TestFixture]
public class MappingsTest
{
    private SeedDataBuilder _seedDataBuilder { get; set; } = default!;

    [SetUp]
    public void SetUp()
    {
        _seedDataBuilder = new SeedDataBuilder();
    }

    [Test]
    public void SectionEventDTOToSection_Should_Convert_SectionEventDTO_to_Section_Correctly(
        [Random(1, 30, 10)] int randomSectionNum)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 5);
        Section expectedSection = seedData.Sections[randomSectionNum];
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section).Returns(seedData.Deployment.Sections[randomSectionNum]);
        sectionEventDTOMock.Setup(dto => dto.Candidates).Returns(seedData.Deployment.Candidates);
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(seedData.Deployment.Votes[randomSectionNum]);
        
        //Act
        Section result = Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object);
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedSectionJSON = JsonSerializer.Serialize(expectedSection);
        
        //Assertions
        Assert.That(result.SectionID, Is.EqualTo(expectedSection.SectionID));
        Assert.That(result.CandidateVotes, Is.EqualTo(expectedSection.CandidateVotes));
        Assert.That(resultJSON, Is.EqualTo(expectedSectionJSON));
    }
    
    [Test, Sequential]
    public void SectionEventDTOToSection_Should_Fail_When_Candidate_And_Votes_Arrays_Have_Different_Sizes(
        [Random(1, 30, 10)] int randomSectionNum,
        [Random(1U, 4U, 10)] uint randomCandidatesSize)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 5);
        SeedData seedData2 = _seedDataBuilder.GenerateNew(1, randomCandidatesSize);
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section).Returns(seedData.Deployment.Sections[randomSectionNum]);
        sectionEventDTOMock.Setup(dto => dto.Candidates).Returns(seedData2.Deployment.Candidates);
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(seedData.Deployment.Votes[randomSectionNum]);

        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object), Throws.TypeOf<ArgumentException>());
    }
    
    [Test, Sequential]
    public void SectionEventDTOToSection_Should_Fail_When_Argument_Null()
    {
        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => Mappings.SectionEventDTOToSection(null), Throws.TypeOf<ArgumentNullException>());
    }
}