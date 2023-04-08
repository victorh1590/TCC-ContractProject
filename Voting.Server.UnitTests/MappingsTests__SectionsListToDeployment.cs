using Voting.Server.UnitTests.TestData;
using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using System.Globalization;

namespace Voting.Server.UnitTests;
public partial class MappingsTests
{
    [Test]
    [Repeat(10)]
    public void SectionsListToDeployment_Should_Return_Correct_Deployment_When_Data_Is_Valid()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 5);
        List<Section> expectedSections = seedData.Sections;
        VotingDbDeployment expectedDeployment = seedData.Deployment;

        //Act
        VotingDbDeployment result = Mappings.SectionsListToDeployment(seedData.Sections);
        string resultJSON = JsonSerializer.Serialize(result);
        string expectedDeploymentJSON = JsonSerializer.Serialize(expectedDeployment);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedDeploymentJSON));
        Assert.That(result.Candidates, Is.EqualTo(expectedDeployment.Candidates));
        Assert.That(result.Sections, Is.EqualTo(expectedDeployment.Sections));
        Assert.That(result.Votes, Is.EqualTo(expectedDeployment.Votes));
        Assert.That(result.Timestamp, Is.Not.Empty);
        Assert.That(result.Timestamp, Is.InstanceOf(typeof(string)));
        Assert.That(DateTime.TryParse(result.Timestamp, out DateTime resultDate), Is.True);
        Assert.That(result.Timestamp, Is.EqualTo(resultDate.ToString(CultureInfo.InvariantCulture)));
        Assert.That(result.CompressedSectionData, Is.EqualTo(expectedDeployment.CompressedSectionData));
    }
    
    [Test]
    public void SectionsListToDeployment_Should_Fail_If_Sections_Is_Empty()
    {
        Assert.That(() => Mappings.SectionsListToDeployment(new List<Section>()), 
            Throws.TypeOf<ArgumentException>());
    }
}