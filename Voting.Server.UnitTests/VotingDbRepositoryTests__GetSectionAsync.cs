using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.UnitTests.SeedData;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

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
    public async Task GetSectionAsync_Should_Return_Correct_Data()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(30); 
        var accountsTask = Repository.Web3.Personal.ListAccounts.SendRequestAsync();
        List<string> accounts = (await accountsTask.WaitAsync(timeSpan)).ToList();
        Guard.IsNotNull(accounts);
        Guard.IsNotEmpty(accounts);
        CollectionAssert.AllItemsAreNotNull(accounts);
        
        TestContext.WriteLine("Accounts in chain: ");
        accounts.ForEach(TestContext.WriteLine);

        SeedData.SeedData seedData = SeedDataBuilder.GenerateNew(30, 4);

        TransactionReceipt transaction = await Repository.DeployContractAndWaitForReceiptAsync(seedData.Deployment);
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        
        //Check BYTECODE and transaction status.
        Guard.IsNotNullOrEmpty(await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress));
        Guard.IsEqualTo(transaction.Status.ToLong(), 1);

        uint sectionNumber = seedData.Deployment.Sections.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        TestContext.WriteLine($"Trying to access contract and getting section {sectionNumber}...");

        Section sectionData = await Repository.GetSectionAsync(sectionNumber);
        Section? expectedSection = seedData.Sections
            .Select(section => section)
            .FirstOrDefault(section => section.SectionID == sectionNumber);
        Guard.IsNotNull(expectedSection);

        string resultJSON = JsonSerializer.Serialize(sectionData);
        string expectedJSON = JsonSerializer.Serialize(expectedSection);
        TestContext.WriteLine(resultJSON);
        TestContext.WriteLine("Expected: " + expectedJSON);

        Assert.That(resultJSON, Is.EqualTo(expectedJSON));
        CollectionAssert.AreEqual(expectedSection.CandidateVotes, sectionData.CandidateVotes);
    }
}