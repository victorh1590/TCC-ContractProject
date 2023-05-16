using Moq;
using System.Text.Json;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Utils;
using CommunityToolkit.Diagnostics;
using Voting.Server.Protos;
using Voting.Server.Utils.Mappings;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;

public partial class MappingsTests
{
    [Test, Sequential]
    public void CandidateEventDTOToSection_Should_Convert_CandidateEventDTO_to_Section_Correctly(
        [Random(1, 30, 10)] int randomSectionIndex, 
        [Random(0, 4, 10)] int randomCandidateIndex)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        Section? expectedSection = seedData.Sections[randomSectionIndex].Clone() as Section;
        Guard.IsNotNull(expectedSection);
        List<CandidateVotes> cvList = new List<CandidateVotes>(expectedSection.CandidateVotes
            .Where(candidateVotes => candidateVotes.Candidate == seedData.Deployment.Candidates[randomCandidateIndex])
            .ToList());
        expectedSection.CandidateVotes.Clear();
        expectedSection.CandidateVotes.AddRange(cvList);
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Section)
            .Returns(seedData.Deployment.Sections[randomSectionIndex]);
        candidateEventDTOMock.Setup(dto => dto.Candidate)
            .Returns(seedData.Deployment.Candidates[randomCandidateIndex]);
        candidateEventDTOMock.Setup(dto => dto.Votes)
            .Returns(seedData.Deployment.Votes[randomSectionIndex][randomCandidateIndex]);
        
        //Act
        Section resultSection = Mappings.CandidateEventDTOToSection(candidateEventDTOMock.Object);
        string resultJSON = JsonSerializer.Serialize(resultSection);
        string expectedSectionJSON = JsonSerializer.Serialize(expectedSection);
        
        //Assertions
        Assert.That(resultSection, Is.Not.SameAs(expectedSection));
        Assert.That(resultSection.CandidateVotes, Is.Not.SameAs(expectedSection.CandidateVotes));
        Assert.That(resultSection.SectionID, Is.EqualTo(expectedSection.SectionID));
        Assert.That(resultSection.CandidateVotes, Is.EqualTo(expectedSection.CandidateVotes));
        Assert.That(resultJSON, Is.EqualTo(expectedSectionJSON));
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Candidate_Number_Is_Zero()
    {
        //Arrange
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Candidate).Returns(0U);

        //Assertions
        Assert.That(() => Mappings.CandidateEventDTOToSection(candidateEventDTOMock.Object), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Section_Or_Candidate_Are_Zero()
    {
        //Arrange
        //Candidate is zero.
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Section)
            .Returns(CurrentContext.Random.NextUInt(1, 472500));
        candidateEventDTOMock.Setup(dto => dto.Candidate).Returns(0U);
        candidateEventDTOMock.Setup(dto => dto.Votes).Returns(0U);
        
        //Section is zero.
        Mock<CandidateEventDTO> candidateEventDTOMock2 = new Mock<CandidateEventDTO>();
        candidateEventDTOMock2.Setup(dto => dto.Section).Returns(0U);
        candidateEventDTOMock2.Setup(dto => dto.Candidate)
            .Returns(CurrentContext.Random.NextUInt(1, 99));
        candidateEventDTOMock2.Setup(dto => dto.Votes).Returns(0U);

        //Assertions
        Assert.That(() => Mappings.CandidateEventDTOToSection(candidateEventDTOMock.Object), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => Mappings.CandidateEventDTOToSection(candidateEventDTOMock2.Object), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Candidate_And_Section_Are_Both_Default()
    {
        //Arrange
        //Candidate is default(zero is default for uint).
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Section).Returns((uint)default!);
        candidateEventDTOMock.Setup(dto => dto.Candidate).Returns((uint)default!);
        candidateEventDTOMock.Setup(dto => dto.Votes)
            .Returns(CurrentContext.Random.NextUInt(1, 99));

        //Assertions
        Assert.That(() => Mappings.CandidateEventDTOToSection(candidateEventDTOMock.Object), 
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Argument_Null()
    {
        //Assertions
        Assert.That(() => Mappings.CandidateEventDTOToSection(), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => Mappings.CandidateEventDTOToSection(null), Throws.TypeOf<ArgumentNullException>());
    }
}