using Moq;
using System.Text.Json;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Protos;
using Voting.Server.Tests.Utils;
using Voting.Server.Utils.Mappings;

namespace Voting.Server.Tests.Unit;

public partial class MappingsTests
{
    [Test]
    [Repeat(10)]
    public void DeploymentToSections_Should_Return_Correct_Sections_Given_A_Valid_Deployment()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        List<Section> expectedSections = new(seedData.Sections);
        Mock<VotingDbDeployment> deploymentMock = new Mock<VotingDbDeployment>();
        deploymentMock.Setup(deployment => deployment.Candidates)
            .Returns(seedData.Deployment.Candidates);
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(seedData.Deployment.Votes);
        deploymentMock.Setup(deployment => deployment.Sections)
            .Returns(seedData.Deployment.Sections);
        deploymentMock.Setup(deployment => deployment.Timestamp)
            .Returns(seedData.Deployment.Timestamp);
        deploymentMock.Setup(deployment => deployment.CompressedSectionData)
            .Returns(seedData.Deployment.CompressedSectionData);

        //Act
        List<Section> resultSections = Mappings.DeploymentToSections(deploymentMock.Object);
        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedSectionsJSON = JsonSerializer.Serialize(expectedSections);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedSectionsJSON));
        Assert.That(resultSections.Count, Is.EqualTo(expectedSections.Count));
        Assert.That(resultSections, Is.Not.SameAs(expectedSections));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.Not.SameAs(expectedSections.Select(section => section.CandidateVotes)));
        Assert.That(resultSections.Select(section => section.SectionID), 
            Is.EquivalentTo(expectedSections.Select(section => section.SectionID)));
        Assert.That(resultSections.Select(section => section.CandidateVotes), 
            Is.EquivalentTo(expectedSections.Select(section => section.CandidateVotes)));
    }

    [Theory]
    public void DeploymentToSections_Should_Fail_When_Candidates_Sections_Or_Votes_Are_Empty()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);

        //Candidates empty mock
        Mock<VotingDbDeployment> deploymentMock = new Mock<VotingDbDeployment>();
        deploymentMock.Setup(deployment => deployment.Candidates)
            .Returns(new List<uint>());
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(seedData.Deployment.Votes);
        deploymentMock.Setup(deployment => deployment.Sections)
            .Returns(seedData.Deployment.Sections);
        deploymentMock.Setup(deployment => deployment.Timestamp)
            .Returns(seedData.Deployment.Timestamp);
        deploymentMock.Setup(deployment => deployment.CompressedSectionData)
            .Returns(seedData.Deployment.CompressedSectionData);

        //Empty Candidates
        Assert.That(() => Mappings.DeploymentToSections(deploymentMock.Object), 
            Throws.TypeOf<ArgumentException>());

        //Empty Votes.
        deploymentMock.Setup(deployment => deployment.Candidates)
            .Returns(seedData.Deployment.Candidates);
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(new List<List<uint>>());
        Assert.That(() => Mappings.DeploymentToSections(deploymentMock.Object), 
            Throws.TypeOf<ArgumentException>());

        //Empty Sections.
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(seedData.Deployment.Votes);
        deploymentMock.Setup(deployment => deployment.Sections)
            .Returns(new List<uint>());   
        Assert.That(() => Mappings.DeploymentToSections(deploymentMock.Object), 
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void DeploymentToSections_Should_Fail_When_Votes_And_Sections_Count_Is_Different()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData1 = SeedDataBuilder.GenerateNew(30, 5); 
        SeedData seedData2 = SeedDataBuilder.GenerateNew(1, 5);

        //Candidates empty mock
        Mock<VotingDbDeployment> deploymentMock = new Mock<VotingDbDeployment>();

        deploymentMock.Setup(deployment => deployment.Timestamp)
            .Returns(seedData1.Deployment.Timestamp);
        deploymentMock.Setup(deployment => deployment.Candidates)
            .Returns(seedData1.Deployment.Candidates);
        deploymentMock.Setup(deployment => deployment.CompressedSectionData)
            .Returns(seedData1.Deployment.CompressedSectionData);
        //Votes.Count = 1
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(seedData2.Deployment.Votes);
        //Sections.Count = 30
        deploymentMock.Setup(deployment => deployment.Sections)
            .Returns(seedData1.Deployment.Sections);

        //Empty Candidates
        Assert.That(() => Mappings.DeploymentToSections(deploymentMock.Object), 
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void DeploymentToSections_Should_Fail_When_Candidates_And_Votes_Items_Count_Are_Different()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData1 = SeedDataBuilder.GenerateNew(30, 5); 
        SeedData seedData2 = SeedDataBuilder.GenerateNew(30, 1);

        //Candidates empty mock
        Mock<VotingDbDeployment> deploymentMock = new Mock<VotingDbDeployment>();

        deploymentMock.Setup(deployment => deployment.Sections)
            .Returns(seedData1.Deployment.Sections);
        deploymentMock.Setup(deployment => deployment.Timestamp)
            .Returns(seedData1.Deployment.Timestamp);
        deploymentMock.Setup(deployment => deployment.CompressedSectionData)
            .Returns(seedData1.Deployment.CompressedSectionData); 
        //Votes[0].Count = 1
        deploymentMock.Setup(deployment => deployment.Votes)
            .Returns(seedData2.Deployment.Votes);
        //Candidates.Count = 5
        deploymentMock.Setup(deployment => deployment.Candidates)
            .Returns(seedData1.Deployment.Candidates);

        //Empty Candidates
        Assert.That(() => Mappings.DeploymentToSections(deploymentMock.Object), 
            Throws.TypeOf<ArgumentException>());
    }
}