using System.Text.Json;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using System.Globalization;
using CommunityToolkit.Diagnostics;
using Voting.Server.Tests.Utils;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Unit;
public partial class MappingsTests
{
    [Test]
    [Repeat(10)]
    public void SectionsListToDeployment_Should_Return_Correct_Deployment_When_Data_Is_Valid()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        VotingDbDeployment? expectedDeployment = seedData.Deployment.Clone() as VotingDbDeployment;
        Guard.IsNotNull(expectedDeployment);
        
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

    [Test]
    public void SectionsListToDeployment_Should_Fail_When_CandidateVotes_Is_Empty()
    {
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 5);
        List<Section> badSectionsList = seedData.Sections
            .Select(section => new Section(section.SectionID, new List<CandidateVotes>()))
            .ToList();

        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());
    }

    [Theory]
    public void SectionsListToDeployment_Should_Fail_When_Any_CandidateVotes_Is_Empty()
    {
        //Generate seed data.
        SeedData seedData1 = SeedDataBuilder.GenerateNew(30, 5);
        List<Section> badSectionsList = new(seedData1.Sections);

        //First CandidateVotes is empty.
        badSectionsList.First().CandidateVotes = new List<CandidateVotes>();
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //Last CandidateVotes is empty.
        badSectionsList = new List<Section>(seedData1.Sections);
        badSectionsList.Last().CandidateVotes = new List<CandidateVotes>();
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //5x Random between first and last is empty.
        for(int i = 0; i < 5; i++)
        {
            badSectionsList = new List<Section>(seedData1.Sections);
            badSectionsList[CurrentContext.Random.Next(1, badSectionsList.Count - 1)]
                .CandidateVotes = new List<CandidateVotes>();
            Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
                Throws.TypeOf<ArgumentException>());
        }
    }

    [Theory]
    public void SectionsListToDeployment_Should_Fail_When_Candidates_Count_Is_Inconsistent()
    {
        //Generate seed data.
        SeedData seedData1 = SeedDataBuilder.GenerateNew(30, 5);
        SeedData seedData2 = SeedDataBuilder.GenerateNew(1, 1);
        List<Section> badSectionsList = new(seedData1.Sections);

        //First have different size.
        badSectionsList.First().CandidateVotes = seedData2.Sections.First().CandidateVotes;
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //Last have different size.
        badSectionsList = seedData1.Sections;
        badSectionsList.Last().CandidateVotes = seedData2.Sections.First().CandidateVotes;
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //5x Random between first and last have different size.
        for(int i = 0; i < 5; i++)
        {
            badSectionsList = seedData1.Sections;
            badSectionsList[CurrentContext.Random.Next(1, badSectionsList.Count - 1)]
                .CandidateVotes = seedData2.Sections.First().CandidateVotes;
            Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
                Throws.TypeOf<ArgumentException>());
        }
    }
}