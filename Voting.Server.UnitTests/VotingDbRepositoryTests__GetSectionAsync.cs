using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[Ignore("Testing blockchain creation.")]
[TestFixture]
public class VotingDbRepositoryTests__GetSectionAsync
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
            .AddUserSecrets<TestChain<Ganache>>()
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

        TransactionReceipt transaction = await Repository.DeployContractAndWaitForReceiptAsync(deployment);
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        
        //Check BYTECODE and transaction status.
        Assert.That(async() => await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress), 
            Is.Not.Null.Or.Empty);
        Assert.That(transaction.Status.ToLong(), Is.EqualTo(1));

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