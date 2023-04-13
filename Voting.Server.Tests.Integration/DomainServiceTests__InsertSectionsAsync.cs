using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.UnitTests;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using Voting.Server.Tests.Utils;
using CommunityToolkit.Diagnostics;
using DotNet.Testcontainers.Builders;
using Nethereum.JsonRpc.Client;
using static NUnit.Framework.TestContext;

namespace Voting.Server.Tests.Integration;

[Order(5)]
[TestFixture]
internal class DomainServiceTests__InsertSectionsAsync : IUseBlockchainAndRepositoryProps
{
    public IGanache TestNet { get; set; } = default!;
    public IConfiguration Config { get; set; } = default!;
    public AccountManager AccountManager { get; set; } = default!;
    public IWeb3ClientsManager ClientsManager { get; set; } = default!;
    public IVotingDbRepository Repository { get; set; } = default!;
    public string Account { get; set; } = default!;
    private DomainService DomainService { get; set; } = default!;
    private SeedDataBuilder _seedDataBuilder = default!;
    
    [SetUp]
    public void SetUp()
    {
        DomainService = new DomainService(Repository);
        _seedDataBuilder = new SeedDataBuilder();
    }
    
    [Ignore("Debug")]
    [Order(1)]
    [Test]
    [Repeat(5)]
    public async Task InsertSectionAsync_Should_Insert_Data_Correctly()
    {
        //Arrange
        //Generate seed data.
        SeedData seedData1 = _seedDataBuilder.GenerateNew(30, 5);
        SeedData seedData2 = _seedDataBuilder.GenerateNew(30, 5);
        
        //Make expected sectionsList from seedData1 and 2.
        List<Section> expectedSections = seedData1.Sections;
        expectedSections.AddRange(seedData2.Sections);

        //Add sections from seedData1 to seedData2 to test if redundancy will be removed.
        seedData2.Sections
            .AddRange(seedData1.Sections.Take(CurrentContext.Random.Next(1, seedData1.Sections.Count)));

        //Act
        //Deploy Sections
        await DomainService.InsertSectionsAsync(seedData1.Sections);
        await DomainService.InsertSectionsAsync(seedData2.Sections);

        //Calls method and convert results to JSON.
        List<Section> resultSections = await DomainService.GetSectionRangeAsync(expectedSections
            .Select(s => s.SectionID)
            .ToArray());

        string resultJSON = JsonSerializer.Serialize(resultSections);
        string expectedJSON = JsonSerializer.Serialize(expectedSections);
        WriteLine(resultJSON);
        WriteLine("Expected: " + expectedJSON);

        //Assertions.
        Assert.That(resultSections, Is.Not.Null.Or.Empty);
        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        CollectionAssert.AllItemsAreUnique(resultSections);
        CollectionAssert.AllItemsAreNotNull(resultSections);
        CollectionAssert.AreEquivalent(
            resultSections.Select(r => r.SectionID), 
            expectedSections.Select(e => e.SectionID));
        CollectionAssert.AreEquivalent(
            resultSections.Select(r => r.CandidateVotes), 
            expectedSections.Select(e => e.CandidateVotes));
    }
    
    [Ignore("Debug")]
    [Order(2)]
    [Test]
    public async Task InsertSectionAsync_Should_Throw_Exception_If_Trying_To_Add_Only_Repeated_Sections()
    {
        //Generate seed data.
        SeedData seedData1 = _seedDataBuilder.GenerateNew(30, 5);

        //Deploy original Sections
        await DomainService.InsertSectionsAsync(seedData1.Sections);
        
        //Assertions.
        Assert.That(async () => await DomainService.InsertSectionsAsync(seedData1.Sections), 
            Throws.TypeOf<ArgumentException>());
    }
    
    [Order(3)]
    [Test]
    public async Task InsertSectionAsync_Should_Fail_When_Contract_Fails_To_Be_Deployed()
    {
        TestNet.Stop().Wait();
        
        //Generate seed data.
        SeedData seedData1 = _seedDataBuilder.GenerateNew(30, 5);

        //Assertions.
        Assert.That(async () => await DomainService.InsertSectionsAsync(seedData1.Sections), 
            Throws.TypeOf<RpcClientUnknownException>());
    }
}