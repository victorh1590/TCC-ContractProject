using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestData;
using Voting.Server.UnitTests.TestNet.Ganache;

namespace Voting.Server.UnitTests;

[Ignore("Debugging tests.")]
[Order(1)]
[TestFixture]
public class VotingDbRepositoryTests__DeployContract
{
    private TestNet<Ganache> TestNet { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    private string Account { get; set; } = default!;
    private BlockParameter Latest { get; } = BlockParameter.CreateLatest();
    private BlockParameter Pending { get; } = BlockParameter.CreatePending();
    private BlockParameter Ealiest { get; } = BlockParameter.CreateEarliest();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Config = new ConfigurationBuilder()
            .AddUserSecrets<TestNet<Ganache>>()
            .Build();
        
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);
        TestNet = new TestNet<Ganache>(AccountManager);
        Account = AccountManager.Accounts.First().Address;
        TestNet.SetUp();
        
        TimeSpan timeSpan = TimeSpan.FromSeconds(30); 
        var accountsTask = Repository.Web3.Personal.ListAccounts.SendRequestAsync();
        List<string> accounts = (await accountsTask.WaitAsync(timeSpan) ?? Array.Empty<string>()).ToList();
        Guard.IsNotNull(accounts);
        Guard.IsNotEmpty(accounts);
        accounts.ForEach(acc => Guard.IsNotNullOrEmpty(acc));
        
        TestContext.WriteLine("Accounts in chain: ");
        accounts.ForEach(TestContext.WriteLine);
        
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestNet.TearDown();
    }

    [Theory]
    [Order(1)]
    public async Task CreateSectionRange_Should_Deploy_Valid_Contract()
    {
        //Generate seed data.
        SeedData seedData = SeedDataBuilder.GenerateNew(30, 4);

        //Successfully returns TransactionReceipt with valid ContractAddress and correct BlockNumber.
        TransactionReceipt transaction = await Repository.CreateSectionRange(seedData.Deployment);
        Assert.That(transaction, Is.Not.Null.Or.Empty);
        Assert.That(transaction.ContractAddress, Is.Not.Null.Or.Empty);
        Assert.That(transaction.BlockNumber.ToLong(), Is.EqualTo(1));
        
        //Check BYTECODE and transaction status.
        Assert.That(async() => await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress), 
            Is.Not.Null.Or.Empty);
        Assert.That(transaction.Status.ToLong(), Is.EqualTo(1));
        
        //Transaction Count is correct.
        long completedTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
                .SendRequestAsync(Account)).ToLong();
        Assert.That(completedTransactionCount, Is.EqualTo(1));
        
        //Successfully creates ContractHandler.
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        ContractHandler handler = Repository.Web3.Eth.GetContractHandler(transaction.ContractAddress);
        Assert.That(handler, Is.Not.Null);
        
        //Contract returns data.
        var compressedDataResult = await handler.QueryAsync<GetCompressedDataFunction?, string>(null, Latest);
        Assert.That(compressedDataResult, Is.EqualTo(seedData.Deployment.CompressedSectionData));
    }
    
    [Test]
    [Order(2)]
    public async Task CreateSetionRange_Should_Fail_When_Invalid_Contract()
    {
        //Generate invalid seed data.
        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = new List<uint>(),
            Votes = new List<List<uint>>(),
            Sections = new List<uint>(),
            Timestamp = "",
            CompressedSectionData = ""
        };

        //Failed Deployment should throw Exception.
        Assert.That(async () => await Repository.CreateSectionRange(deployment), 
            Throws.TypeOf<RpcResponseException>());
        
        //Transaction count should be zero.
        long completedTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
            .SendRequestAsync(Account)).ToLong();
        Assert.That(completedTransactionCount, Is.EqualTo(1));
    }
}