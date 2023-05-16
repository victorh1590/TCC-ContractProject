using System.Text.Json;
// using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence.ContractDefinition;
using System.Globalization;
using CommunityToolkit.Diagnostics;
using Voting.Server.Protos;
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
        VotingDbDeployment resultDeployment = Mappings.SectionsListToDeployment(seedData.Sections);
        
        //SectionsListToDeployment will make a new timestamp and it can be different from the seedData timestamp.
        //In that case, the JSONs will differ. So i'm forcing equality here.
        seedData.Deployment.Timestamp = resultDeployment.Timestamp;
        
        //Generate JSONs
        string resultJSON = JsonSerializer.Serialize(resultDeployment);
        string expectedDeploymentJSON = JsonSerializer.Serialize(expectedDeployment);

        //Assertions
        Assert.That(resultJSON, Is.EqualTo(expectedDeploymentJSON));
        Assert.That(resultDeployment, Is.Not.SameAs(expectedDeployment));
        Assert.That(resultDeployment.Sections, Is.Not.SameAs(expectedDeployment.Sections));
        Assert.That(resultDeployment.Candidates, Is.Not.SameAs(expectedDeployment.Candidates));
        Assert.That(resultDeployment.Votes, Is.Not.SameAs(expectedDeployment.Votes));
        Assert.That(resultDeployment.CompressedSectionData, Is.Not.SameAs(expectedDeployment.CompressedSectionData));
        Assert.That(resultDeployment.Candidates, Is.EqualTo(expectedDeployment.Candidates));
        Assert.That(resultDeployment.Sections, Is.EqualTo(expectedDeployment.Sections));
        Assert.That(resultDeployment.Votes, Is.EqualTo(expectedDeployment.Votes));
        Assert.That(resultDeployment.Timestamp, Is.Not.Empty);
        Assert.That(resultDeployment.Timestamp, Is.InstanceOf(typeof(string)));
        Assert.That(DateTime.TryParse(resultDeployment.Timestamp, out DateTime resultDate), Is.True);
        Assert.That(resultDeployment.Timestamp, Is.EqualTo(resultDate.ToString(CultureInfo.InvariantCulture)));
        Assert.That(resultDeployment.CompressedSectionData, Is.EqualTo(expectedDeployment.CompressedSectionData));
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
        //TODO not sure if this is correct.
        List<Section> badSectionsList = seedData.Sections
            .Select(section => new Section
            {
                SectionID = section.SectionID,
                CandidateVotes = { }
            })
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
        badSectionsList.First().CandidateVotes.Clear();
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //Last CandidateVotes is empty.
        badSectionsList = new List<Section>(seedData1.Sections);
        badSectionsList.Last().CandidateVotes.Clear();
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //5x Random between first and last is empty.
        for(int i = 0; i < 5; i++)
        {
            badSectionsList = new List<Section>(seedData1.Sections);
            badSectionsList[CurrentContext.Random.Next(1, badSectionsList.Count - 1)]
                .CandidateVotes.AddRange(new List<CandidateVotes>());
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
        badSectionsList.First().CandidateVotes.AddRange(seedData2.Sections.First().CandidateVotes);
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //Last have different size.
        badSectionsList = seedData1.Sections;
        badSectionsList.Last().CandidateVotes.AddRange(seedData2.Sections.First().CandidateVotes);
        Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
            Throws.TypeOf<ArgumentException>());

        //5x Random between first and last have different size.
        for(int i = 0; i < 5; i++)
        {
            badSectionsList = seedData1.Sections;
            badSectionsList[CurrentContext.Random.Next(1, badSectionsList.Count - 1)]
                .CandidateVotes.AddRange(seedData2.Sections.First().CandidateVotes);
            Assert.That(() => Mappings.SectionsListToDeployment(badSectionsList), 
                Throws.TypeOf<ArgumentException>());
        }
    }
}