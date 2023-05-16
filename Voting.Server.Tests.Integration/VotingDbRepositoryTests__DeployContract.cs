using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Voting.Server.Persistence;
using Voting.Server.Persistence.Accounts;
using Voting.Server.Persistence.Clients;
using Voting.Server.Persistence.ContractDefinition;
using Voting.Server.Tests.Integration.TestNet.Ganache;
using Voting.Server.Tests.Utils;
using Voting.Server.UnitTests;

namespace Voting.Server.Tests.Integration;

// [Ignore("Debug")]
[Order(1)]
[TestFixture]
public class VotingDbRepositoryTests__DeployContract : IUseBlockchainAndRepositoryProps
{
    public IGanache TestNet { get; set; } = default!;
    public IConfiguration Config { get; set; } = default!;
    public AccountManager AccountManager { get; set; } = default!;
    public IWeb3ClientsManager ClientsManager { get; set; } = default!;
    public IVotingDbRepository Repository { get; set; } = default!;
    public string Account { get; set; } = default!;
    private BlockParameter Latest { get; } = BlockParameter.CreateLatest();
    private BlockParameter Pending { get; } = BlockParameter.CreatePending();
    private BlockParameter Ealiest { get; } = BlockParameter.CreateEarliest();
    private readonly SeedDataBuilder _seedDataBuilder = new();
    
    [Order(1)]
    [Test]
    public async Task CreateSectionRange_Should_Deploy_Valid_Contract()
    {
        Thread.Sleep(5000);
        //Generate seed data.
        SeedData seedData = _seedDataBuilder.GenerateNew(30, 4);
        
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
        ContractHandler handler = Repository.Web3.Eth.GetContractHandler(transaction.ContractAddress);
        Assert.That(handler, Is.Not.Null);
        
        //Contract returns data.
        var compressedDataResult = 
            await handler.QueryAsync<GetCompressedDataFunction?, string>(null, Latest);
        Assert.That(compressedDataResult, Is.Not.SameAs(seedData.Deployment.CompressedSectionData));
        Assert.That(compressedDataResult, Is.EqualTo(seedData.Deployment.CompressedSectionData));
    }
    
    [Order(2)]
    [Test]
    public async Task CreateSectionRange_Should_Fail_When_Invalid_Contract()
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