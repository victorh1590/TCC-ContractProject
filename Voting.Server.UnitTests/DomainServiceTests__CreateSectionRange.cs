using System.Reflection.Emit;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Voting.Server.Domain;
using Voting.Server.Domain.Models;
using Voting.Server.Domain.Models.Mappings;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestNet.Ganache;


namespace Voting.Server.UnitTests;

public partial class DomainServiceTests
{
    private TestChain<Ganache> TestChain { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    // public string URL { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = new ConfigurationBuilder()
            .AddUserSecrets<DomainServiceTests>()
            .Build();
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);
        TestChain = new TestChain<Ganache>(AccountManager);
        TestChain.SetUp();
        // URL = $"HTTP://{Options.GanacheOptions.Host}:{Options.GanacheOptions.Port}";
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestChain.TearDown();
    }

    [Theory]
    public async Task DeployContractTest()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(30); 
        var accountsTask = Repository.Web3.Personal.ListAccounts.SendRequestAsync();
        List<string> accounts = (await accountsTask.WaitAsync(timeSpan) ?? Array.Empty<string>()).ToList();
        Assert.That(accounts, Is.Not.Null);
        Assert.That(accounts, Is.Not.Empty);
        CollectionAssert.AllItemsAreNotNull(accounts);
        TestContext.WriteLine("Accounts in chain: ");
        accounts.ForEach(TestContext.WriteLine);

        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = SeedData.SeedData.Candidates,
            Votes = SeedData.SeedData.Votes,
            Sections = SeedData.SeedData.Sections,
            Timestamp = SeedData.SeedData.Timestamp,
            CompressedSectionData = SeedData.SeedData.CompressedSectionData
        };

        string contractAddress = await Repository.DeployContractAsync(deployment);
        TestContext.WriteLine("Contract Address: " + contractAddress);

        uint sectionNumber = 190U;
        TestContext.WriteLine($"Trying to access contract and getting section {sectionNumber}...");

        Section sectionData = await Repository.GetSectionAsync(sectionNumber);
        Assert.That(sectionData, Is.Not.Null);

        Section expectedSection = new Section(
            190U,
            new List<CandidateVotes>
            {
                new(15U, 1U),
                new(25U, 8U),
                new(35U, 7U),
                new(55U, 5U)
            }
        );

        string resultJSON = JsonSerializer.Serialize(sectionData);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);
        TestContext.WriteLine(resultJSON);
        TestContext.WriteLine("Expected: " + expectedJSON);

        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        CollectionAssert.AreEqual(expectedSection.CandidateVotes, sectionData.CandidateVotes);
    }
}