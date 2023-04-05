using Voting.Server.UnitTests.TestData;
using Moq;
using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;

namespace Voting.Server.UnitTests;

public partial class MappingsTests
{
    [Test]
    // [Repeat(10)]
    public void DeploymentToSections_Should_Return_Correct_Sections_Given_A_Valid_Deployment()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 5);
        List<Section> expectedSections = seedData.Sections;
        Mock<VotingDbDeployment> deploymentMock = new Mock<VotingDbDeployment>();
        deploymentMock.Setup(dto => dto.Candidates).Returns(seedData.Deployment.Candidates);
        deploymentMock.Setup(dto => dto.Votes).Returns(seedData.Deployment.Votes);
        deploymentMock.Setup(dto => dto.Sections).Returns(seedData.Deployment.Sections);
        deploymentMock.Setup(dto => dto.Timestamp).Returns(seedData.Deployment.Timestamp);
        deploymentMock.Setup(dto => dto.CompressedSectionData)
            .Returns(seedData.Deployment.CompressedSectionData);

        //Act
        List<Section> result = Mappings.DeploymentToSections(deploymentMock.Object);
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedSectionsJSON = JsonSerializer.Serialize(expectedSections);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedSectionsJSON));
        Assert.That(result.Select(section => section.SectionID), 
            Is.EquivalentTo(expectedSections.Select(section => section.SectionID)));
        Assert.That(result.Select(section => section.CandidateVotes), 
            Is.EquivalentTo(expectedSections.Select(section => section.CandidateVotes)));
    }
}