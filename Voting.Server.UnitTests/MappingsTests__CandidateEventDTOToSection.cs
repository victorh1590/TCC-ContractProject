using Voting.Server.UnitTests.TestData;
using Moq;
using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests;

public partial class MappingsTests
{
    [Test, Sequential]
    public void CandidateEventDTOToSection_Should_Convert_CandidateEventDTO_to_Section_Correctly(
        [Random(1, 30, 10)] int randomSectionIndex, 
        [Random(0, 4, 10)] int randomCandidateIndex)
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 5);
        Section expectedSection = seedData.Sections[randomSectionIndex];
        expectedSection.CandidateVotes = expectedSection.CandidateVotes
            .Where(candidateVotes => candidateVotes.Candidate == seedData.Deployment.Candidates[randomCandidateIndex])
            .ToList();
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Section)
            .Returns(seedData.Deployment.Sections[randomSectionIndex]);
        candidateEventDTOMock.Setup(dto => dto.Candidate)
            .Returns(seedData.Deployment.Candidates[randomCandidateIndex]);
        candidateEventDTOMock.Setup(dto => dto.Votes)
            .Returns(seedData.Deployment.Votes[randomSectionIndex][randomCandidateIndex]);
        
        //Act
        Section result = Mappings.CandidateEventDTOToSection(candidateEventDTOMock.Object);
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedSectionJSON = JsonSerializer.Serialize(expectedSection);
        
        //Assertions
        Assert.That(result.SectionID, Is.EqualTo(expectedSection.SectionID));
        Assert.That(result.CandidateVotes, Is.EqualTo(expectedSection.CandidateVotes));
        Assert.That(resultJSON, Is.EqualTo(expectedSectionJSON));
    }
    
    [Test]
    public void CandidateEventDTOToSection_Should_Fail_When_Candidate_Number_Is_Zero()
    {
        //Arrange
        //Generate seed data.
        Mock<CandidateEventDTO> candidateEventDTOMock = new Mock<CandidateEventDTO>();
        candidateEventDTOMock.Setup(dto => dto.Candidate).Returns(0U);

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