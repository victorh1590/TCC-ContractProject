using Moq;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Utils;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class MappingsTests
{
    [Test]
    public void SectionEventDTOToSection_Should_Convert_SectionEventDTO_to_Section_Correctly(
        [Random(1, 30, 10)] int randomSectionIndex)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        Section? expectedSection = seedData.Sections[randomSectionIndex].Clone() as Section;
        Guard.IsNotNull(expectedSection);
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section).Returns(seedData.Deployment.Sections[randomSectionIndex]);
        sectionEventDTOMock.Setup(dto => dto.Candidates).Returns(seedData.Deployment.Candidates);
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(seedData.Deployment.Votes[randomSectionIndex]);
        
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
        [Random(1, 30, 10)] int randomSectionIndex,
        [Random(1U, 4U, 10)] uint randomCandidatesSize)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        SeedData seedData2 = SeedDataBuilder.GenerateNew(1, randomCandidatesSize);
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section).Returns(seedData.Deployment.Sections[randomSectionIndex]);
        sectionEventDTOMock.Setup(dto => dto.Candidates).Returns(seedData2.Deployment.Candidates);
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(seedData.Deployment.Votes[randomSectionIndex]);

        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object), 
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void SectionEventDTOToSection_Should_Fail_When_Candidate_And_Votes_Arrays_Are_Empty()
    {
        //Arrange
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section)
            .Returns(CurrentContext.Random.NextUInt(1, 472500));
        sectionEventDTOMock.Setup(dto => dto.Candidates).Returns(new List<uint>());
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(new List<uint>());

        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object), 
            Throws.TypeOf<ArgumentException>());
    }
    
    [Test]
    public void SectionEventDTOToSection_Should_Fail_When_Candidate_Or_Votes_Arrays_Are_Empty()
    {
        //Arrange
        //Votes is empty.
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section)
            .Returns(CurrentContext.Random.NextUInt(1, 472500));
        sectionEventDTOMock.Setup(dto => dto.Candidates)
            .Returns(new List<uint> { CurrentContext.Random.NextUInt(1, 99) } );
        sectionEventDTOMock.Setup(dto => dto.Votes).Returns(new List<uint>());

        //Candidates is empty.
        Mock<SectionEventDTO> sectionEventDTOMock2 = new Mock<SectionEventDTO>();
        sectionEventDTOMock2.Setup(dto => dto.Section)
            .Returns(CurrentContext.Random.NextUInt(1, 472500));
        sectionEventDTOMock2.Setup(dto => dto.Candidates).Returns(new List<uint>() );
        sectionEventDTOMock2.Setup(dto => dto.Votes)
            .Returns(new List<uint> { 0U });
        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object), 
            Throws.TypeOf<ArgumentException>());
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock2.Object), 
            Throws.TypeOf<ArgumentException>());
    }
    
    [Test]
    public void SectionEventDTOToSection_Should_Fail_When_Section_Is_Zero()
    {
        //Arrange
        Mock<SectionEventDTO> sectionEventDTOMock = new Mock<SectionEventDTO>();
        sectionEventDTOMock.Setup(dto => dto.Section).Returns(0);
        sectionEventDTOMock.Setup(dto => dto.Candidates)
            .Returns(new List<uint> { CurrentContext.Random.NextUInt(1, 99) } );
        sectionEventDTOMock.Setup(dto => dto.Votes)
            .Returns(new List<uint> { 0U });

        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(sectionEventDTOMock.Object), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void SectionEventDTOToSection_Should_Fail_When_Argument_Null()
    {
        //Assertions
        Assert.That(() => Mappings.SectionEventDTOToSection(), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => Mappings.SectionEventDTOToSection(null), Throws.TypeOf<ArgumentNullException>());
    }
}