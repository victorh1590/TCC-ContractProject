using System.Diagnostics.Contracts;
using System.Numerics;
using System.Text.Json;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Voting.Server.Domain.Models;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.UnitTests.TestNet.Ganache;
using Contract = Nethereum.Contracts.Contract;

namespace Voting.Server.UnitTests;

public class VotingDbRepositoryTests__DeployContract
{
    private TestChain<Ganache> TestChain { get; set; } = default!;
    private IConfiguration Config { get; set; } = default!;
    private AccountManager AccountManager { get; set; } = default!;
    private IWeb3ClientsManager ClientsManager { get; set; } = default!;
    private IVotingDbRepository Repository { get; set; } = default!;
    private string Account { get; set; } = default!;
    private BlockParameter Latest { get; } = BlockParameter.CreateLatest();
    private BlockParameter Pending { get; } = BlockParameter.CreatePending();
    // public string URL { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Config = new ConfigurationBuilder()
            .AddUserSecrets<TestChain<Ganache>>()
            .Build();
        
        AccountManager = new AccountManager(Config);
        ClientsManager = new Web3ClientsManager(AccountManager);
        Repository = new VotingDbRepository(ClientsManager);
        TestChain = new TestChain<Ganache>(AccountManager);
        Account = AccountManager.Accounts.First().Address;
        TestChain.SetUp();
        // URL = $"HTTP://{Options.GanacheOptions.Host}:{Options.GanacheOptions.Port}";
        
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
        TestChain.TearDown();
    }

    [Test]
    public async Task Valid_Contract_Is_Successfully_Deployed()
    {
        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = SeedData.SeedData.Candidates,
            Votes = SeedData.SeedData.Votes,
            Sections = SeedData.SeedData.Sections,
            Timestamp = SeedData.SeedData.Timestamp,
            CompressedSectionData = SeedData.SeedData.CompressedSectionData
        };

        //Successfully returns TransactionReceipt with valid ContractAddress and correct BlockNumber.
        TransactionReceipt transaction = await Repository.DeployContractAndWaitForReceiptAsync(deployment);
        Assert.That(transaction, Is.Not.Null.Or.Empty);
        Assert.That(transaction.ContractAddress, Is.Not.Null.Or.Empty);
        Assert.That(transaction.BlockNumber.ToLong(), Is.EqualTo(1));
        
        //Check BYTECODE and transaction status.
        Assert.That(async() => await Repository.Web3.Eth.GetCode.SendRequestAsync(transaction.ContractAddress), 
            Is.Not.Null.Or.Empty);
        Assert.That(transaction.Status.ToLong(), Is.EqualTo(1));
        
        //Transaction Count is correct.
        long completedTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
                .SendRequestAsync(Account, Latest)).ToLong();
        long pendingTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
                .SendRequestAsync(Account, Pending)).ToLong();
        Assert.That(completedTransactionCount, Is.EqualTo(1));
        Assert.That(pendingTransactionCount, Is.EqualTo(1));
        
        //Successfully creates ContractHandler.
        TestContext.WriteLine("Contract Address: " + transaction.ContractAddress);
        ContractHandler handler = Repository.Web3.Eth.GetContractHandler(transaction.ContractAddress);
        Assert.That(handler, Is.Not.Null);

        //Contract returns data.
        var compressedDataResult = await handler.QueryAsync<GetCompressedDataFunction?, string>(null, Latest);
        Assert.That(compressedDataResult, Is.EqualTo(SeedData.SeedData.CompressedSectionData));
    }
    
    [Test]
    public async Task Invalid_Contract_Fails_To_Be_Deployed()
    {
        //All data Invalid.
        VotingDbDeployment deployment = new VotingDbDeployment
        {
            Candidates = new List<uint>(),
            Votes = new List<List<uint>>(),
            Sections = new List<uint>(),
            Timestamp = "",
            CompressedSectionData = ""
        };

        //Failed Deployment should throw Exception.
        Assert.That(async () => await Repository.DeployContractAsync(deployment), 
            Throws.TypeOf<RpcResponseException>());
        
        //Transaction count should be zero.
        long completedTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
            .SendRequestAsync(Account, Latest)).ToLong();
        long pendingTransactionCount = (await Repository.Web3.Eth.Transactions.GetTransactionCount
            .SendRequestAsync(Account, Pending)).ToLong();
        Assert.That(completedTransactionCount, Is.EqualTo(0));
        Assert.That(pendingTransactionCount, Is.EqualTo(0));
    }
}